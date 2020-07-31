using System.Collections;
using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Topology;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [Header("General Settings")]
    public int seed;
    [Range(1, 1000000), SerializeField] private int _sizeX;
    [Range(1, 1000000), SerializeField] private int _sizeY;

    [Header("Point distribution")]
    [Range(4, 1000000), SerializeField] private int _pointDensity;

    [Header("Color")]
    [SerializeField] private Gradient _heightGradient;

    [Header("Main noise settings")]
    [Range(1, 50000), SerializeField] private float _heightScale;
    [Range(1, 10000), SerializeField] private float _scale;
    [Range(0, 1), SerializeField] private float _dampening;

    [Header("Layer noise settings")]
    [Range(1, 20), SerializeField] private int _octaves;
    [Range(0, 1), SerializeField] private float _persistance;
    [Range(0, 5), SerializeField] private float _lacunarity;

    [Header("Position")]
    public Vector2 Offset;






    private Polygon polygon;
    private TriangleNet.Mesh mesh;
    private UnityEngine.Mesh terrainMesh;
    private List<float> heights = new List<float>();

    private float minNoiseHeight;
    private float maxNoiseHeight;
    private System.Random rand;

    public void Build()
    {
        System.DateTime startTime = System.DateTime.Now;
        polygon = new Polygon();
        rand = new System.Random(seed);

        for (int i = 0; i < _pointDensity; i++)
        {
            float x = (float)rand.NextDouble() * _sizeX;
            float y = (float)rand.NextDouble() * _sizeY;

            polygon.Add(new Vertex(x, y));
        }

        ConstraintOptions constraints = new ConstraintOptions();
        constraints.ConformingDelaunay = true;


        mesh = polygon.Triangulate(constraints) as TriangleNet.Mesh;
        Debug.Log(mesh.vertices.Count);
        SetHeight();
        CreateMesh();
        Debug.Log(System.DateTime.Now - startTime);
    }

    private void CreateMesh()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();
        List<Color> colors = new List<Color>();
        List<Color32> colors32 = new List<Color32>();

        IEnumerator<Triangle> triangleEnum = mesh.Triangles.GetEnumerator();

        for (int i = 0; i < mesh.Triangles.Count; i++)
        {
            if (!triangleEnum.MoveNext())
            {
                break;
            }

            Triangle currentTriangle = triangleEnum.Current;

            var color = EvaluateColor(currentTriangle);
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);

            colors32.Add(color);
            colors32.Add(color);
            colors32.Add(color);

            Vector3 v0 = new Vector3((float)currentTriangle.vertices[2].x, heights[currentTriangle.vertices[2].id], (float)currentTriangle.vertices[2].y);
            Vector3 v1 = new Vector3((float)currentTriangle.vertices[1].x, heights[currentTriangle.vertices[1].id], (float)currentTriangle.vertices[1].y);
            Vector3 v2 = new Vector3((float)currentTriangle.vertices[0].x, heights[currentTriangle.vertices[0].id], (float)currentTriangle.vertices[0].y);

            triangles.Add(vertices.Count);
            triangles.Add(vertices.Count + 1);
            triangles.Add(vertices.Count + 2);

            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);

            var normal = Vector3.Cross(v1 - v0, v2 - v0);

            for (int x = 0; x < 3; x++)
            {
                normals.Add(normal);
                uvs.Add(Vector3.zero);
            }
        }

        terrainMesh = new Mesh();
        terrainMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        terrainMesh.vertices = vertices.ToArray();
        terrainMesh.triangles = triangles.ToArray();
        terrainMesh.uv = uvs.ToArray();
        terrainMesh.normals = normals.ToArray();
        terrainMesh.colors = colors.ToArray();

        GetComponent<MeshFilter>().sharedMesh = terrainMesh;
        GetComponent<MeshFilter>().sharedMesh.colors = colors.ToArray();
        GetComponent<MeshCollider>().sharedMesh = terrainMesh;
        Debug.Log(GetComponent<MeshFilter>().sharedMesh.vertices.Length);

    }

    private void SetHeight()
    {
        minNoiseHeight = float.PositiveInfinity;
        maxNoiseHeight = float.NegativeInfinity;
        heights = new List<float>();

        for (int i = 0; i < mesh.vertices.Count; i++)
        {

            float amplitude = 1f;
            float frequency = 1f;
            float noiseHeight = 0f;


            for (int o = 0; o < _octaves; o++)
            {
                float xValue = (float)mesh.vertices[i].x / _scale * frequency;
                float yValue = (float)mesh.vertices[i].y / _scale * frequency;

                float perlinValue = Mathf.PerlinNoise(xValue + Offset.x + seed, yValue + Offset.y + seed) * 2 - 1;
                perlinValue *= _dampening;

                noiseHeight += perlinValue * amplitude;

                amplitude *= _persistance;
                frequency *= _lacunarity;
            }

            if (noiseHeight > maxNoiseHeight)
            {
                maxNoiseHeight = noiseHeight;
            }
            else if (noiseHeight < minNoiseHeight)
            {
                minNoiseHeight = noiseHeight;
            }

            noiseHeight = (noiseHeight < 0f) ? noiseHeight * _heightScale / 10f : noiseHeight * _heightScale;
            heights.Add(noiseHeight);

        }
    }

    private void Start()
    {

    }
    private Color EvaluateColor(Triangle triangle)
    {
        var currentHeight = heights[triangle.vertices[0].id] + heights[triangle.vertices[1].id] + heights[triangle.vertices[2].id];
        currentHeight /= 3;
        currentHeight = (currentHeight < 0f) ? currentHeight / _heightScale * 10f : currentHeight / _heightScale;
        var gradientVal = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, currentHeight);
        return _heightGradient.Evaluate(gradientVal);
    }
    public void Destroy()
    {
        GetComponent<MeshFilter>().mesh = null;
    }
    public void NewSeed()
    {
        seed = Random.Range(0, 10000);
    }
    public void NewColorSet()
    {
        var sand = new GradientColorKey(RandomColor(), 0.3f);
        var grass = new GradientColorKey(RandomColor(), 0.55f);
        var mountains = new GradientColorKey(RandomColor(), 0.75f);
        var snow = new GradientColorKey(RandomColor(), 1f);
        List<GradientColorKey> keys = new List<GradientColorKey> { sand, grass, mountains, snow };
        _heightGradient.colorKeys = keys.ToArray();
    }
    private Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }
}
