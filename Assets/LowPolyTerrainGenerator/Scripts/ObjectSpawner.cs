using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private TerrainGenerator _terrainGenerator;
    [SerializeField] private Transform _prefabsParent;

    [HideInInspector] public List<SpawnObject> SpawningObjects;

    private System.Random rand;

    public void Spawn()
    {
        Clear();
        if (SpawningObjects == null) { SpawningObjects = new List<SpawnObject>(); }
        foreach (var spawnObject in SpawningObjects)
        {
            if (spawnObject.Prefab == null) { continue; }
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

            for (long i = 0; i < terrainSize.x; i += spawnObject.SpawnStep)
            {
                for (int j = 0; j < terrainSize.y; j += spawnObject.SpawnStep)
                {
                    if (rand.NextDouble() <= spawnObject.Probability)
                    {
                        float randomX = Random.Range(renderer.bounds.min.x, renderer.bounds.max.x);
                        float randomZ = Random.Range(renderer.bounds.min.z, renderer.bounds.max.z);
                        RaycastHit hit;
                        if (Physics.Raycast(new Vector3(randomX, renderer.bounds.max.y + 5f, randomZ), -Vector3.up, out hit))
                        {
                            float currentHeightPercentage = (hit.point.y - minHeight) / (maxHeight - minHeight);
                            if (currentHeightPercentage <= spawnObject.MaxHeightPercentage && currentHeightPercentage >= spawnObject.MinHeightPercentage)
                            {
                                var prefab = Instantiate(spawnObject.Prefab, hit.point, Quaternion.identity);
                                prefab.transform.SetParent(_prefabsParent);
                            }
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
