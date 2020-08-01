using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnObjects))]
public class SpawnerEditor : Editor
{
    SpawnObjects _spawner;
    private void OnEnable()
    {
        if (target == null) { return; }
        _spawner = (SpawnObjects)target;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        if (GUILayout.Button("Spawn"))
        {
            _spawner.Spawn();
        }
        if (GUILayout.Button("Clear"))
        {
            _spawner.Clear();
        }
    }
}
