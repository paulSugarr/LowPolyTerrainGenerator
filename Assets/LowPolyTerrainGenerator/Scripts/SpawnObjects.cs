using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private Transform _prefabsParent;

    [Header("Prefab Settings")]
    [Range(0, 1), SerializeField] private float _probability;
    [Range(1, 1000), SerializeField] private int _spawnStep;
    [SerializeField] private GameObject _prefab;
    [Range(0, 1), SerializeField] private float _maxHeightPercentage;
    [Range(0, 1), SerializeField] private float _minHeightPercentage;

    private System.Random rand;

    public void Spawn()
    {
        Clear();

        rand = new System.Random(_terrainGenerator.Seed);
        var terrainSize = _terrainGenerator.GetSize();
        long pointCount = terrainSize.x * terrainSize.y;


        Renderer renderer = _terrainGenerator.GetComponent<Renderer>();
        float maxHeight = float.NegativeInfinity;
        float minHeight = float.PositiveInfinity;
        var vertecies = _terrainGenerator.GetComponent<MeshFilter>().sharedMesh.vertices;

        foreach (var vertex in vertecies)
        {
            if (vertex.y > maxHeight) maxHeight = vertex.y;
            if (vertex.y < minHeight) minHeight = vertex.y;
        }

        for (long i = 0; i < terrainSize.x; i += _spawnStep)
        {
            for (int j = 0; j < terrainSize.y; j+= _spawnStep)
            {
                if (rand.NextDouble() <= _probability)
                {
                    float randomX = Random.Range(renderer.bounds.min.x, renderer.bounds.max.x);
                    float randomZ = Random.Range(renderer.bounds.min.z, renderer.bounds.max.z);
                    RaycastHit hit;
                    if (Physics.Raycast(new Vector3(randomX, renderer.bounds.max.y + 5f, randomZ), -Vector3.up, out hit))
                    {
                        float currentHeightPercentage = (hit.point.y - minHeight) / (maxHeight - minHeight);
                        if (currentHeightPercentage <= _maxHeightPercentage && currentHeightPercentage >= _minHeightPercentage)
                        {
                            var prefab = Instantiate(_prefab, hit.point, Quaternion.identity);
                            prefab.transform.SetParent(_prefabsParent);
                        }
                    }
                }
            }

        }


    }

    public void Clear()
    {
        var children = _prefabsParent.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child != _prefabsParent) { DestroyImmediate(child.gameObject); }
        }
    }

}
