using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    TerrainGenerator _generator;
    private void OnEnable()
    {
        if (target == null) { return; }
        _generator = (TerrainGenerator)target;
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        if (GUILayout.Button("NewSeed"))
        {
            _generator.NewSeed();
        }
        if (GUILayout.Button("Build"))
        {
            _generator.Build();

        }
        //if (GUILayout.Button("Save"))
        //{
        //    AssetDatabase.CreateAsset(_generator.GetComponent<MeshFilter>().sharedMesh, "Assets/Meshes/mesh.asset");
        //    AssetDatabase.SaveAssets();
        //}
        if (GUILayout.Button("New Colorset"))
        {
            _generator.NewColorSet();
            Repaint();
        }
        if (GUILayout.Button("Destroy"))
        {
            _generator.Destroy();
        }
    }
}
