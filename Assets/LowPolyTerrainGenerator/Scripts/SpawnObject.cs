using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public GameObject Prefab;

    [Range(0, 1)] public float Probability;
    [Range(1, 1000)] public int SpawnStep;
    [Range(0, 1)] public float MaxHeightPercentage;
    [Range(0, 1)] public float MinHeightPercentage;

    public SpawnObject(float probability, int spawnStep, GameObject prefab, float maxHeightPercentage,
                       float minHeightPercentage)
    {
        Probability = probability;
        SpawnStep = spawnStep;
        Prefab = prefab;
        MaxHeightPercentage = maxHeightPercentage;
        MinHeightPercentage = minHeightPercentage;
    }
}
