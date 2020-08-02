using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(ObjectSpawner))]
public class SpawnerEditor : Editor
{
    ReorderableList reordableObjects;
    ObjectSpawner _spawner;
    bool ShowList = false;
    float lineHeight;
    float lineHeightSpace;
    private void OnEnable()
    {
        if (target == null) { return; }
        _spawner = (ObjectSpawner)target;
        if (_spawner.SpawningObjects == null) { _spawner.SpawningObjects = new List<SpawnObject>(); }
        lineHeight = EditorGUIUtility.singleLineHeight;
        lineHeightSpace = lineHeight + 5;
        CellsListSetup();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(lineHeightSpace);
        var rect = GUILayoutUtility.GetRect(10, 25);


        if (EditorGUI.DropdownButton(rect, new GUIContent("Show spawn objects"), FocusType.Keyboard))
        {
            ShowList = !ShowList;
        }
        if (ShowList)
        {
            reordableObjects.DoLayoutList();
        }
        GUILayout.Space(5);

        if (GUILayout.Button("Spawn"))
        {
            _spawner.Spawn();
        }
        if (GUILayout.Button("Clear"))
        {
            _spawner.Clear();
        }

    }

    private void CellsListSetup()
    {
        var objects = _spawner.SpawningObjects;
        if (objects == null) { objects = new List<SpawnObject>(); }
        reordableObjects = new ReorderableList(serializedObject, serializedObject.FindProperty("SpawningObjects"), true, true, true, true);
        reordableObjects.elementHeight = 180;
        
        reordableObjects.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Spawn Object Types");
        };
        reordableObjects.onAddCallback = (ReorderableList list) =>
        {
            SpawnObject newObject = new SpawnObject(0.1f, 1, null, 0.5f, 0.1f);
            objects.Add(newObject);
        };
        reordableObjects.onRemoveCallback = (ReorderableList list) =>
        {
            objects.RemoveAt(list.index);
        };

        reordableObjects.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            rect.y += 5;
            var newObject = EditorGUI.ObjectField(new Rect(rect.x + 60, rect.y, rect.width / 2, lineHeight), objects[index].Prefab, typeof(GameObject), false) as GameObject;

            objects[index].Prefab = newObject;
            Texture2D icon;
            SerializedProperty element = reordableObjects.serializedProperty.GetArrayElementAtIndex(index);
            
            if (newObject != null)
            {
                
                icon = AssetPreview.GetAssetPreview(objects[index].Prefab);
            }
            else
            {
                icon = Texture2D.grayTexture;
            }
            if (icon != null)
            {
                EditorGUI.DrawPreviewTexture(new Rect(rect.x, rect.y, 50, 50), icon);
            }
            rect.y += lineHeightSpace;

            EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, rect.width / 4, lineHeight), string.Format("Probability  ", index));
            objects[index].Probability = EditorGUI.Slider(new Rect(rect.x + 160, rect.y, EditorGUIUtility.currentViewWidth / 3, lineHeight),
                objects[index].Probability, 0f, 1f);
            rect.y += lineHeightSpace;

            EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, rect.width / 4, lineHeight), string.Format("Spawn Step ", index));
            objects[index].SpawnStep = EditorGUI.IntSlider(new Rect(rect.x + 160, rect.y, EditorGUIUtility.currentViewWidth / 3, lineHeight),
                objects[index].SpawnStep, 0, 1000);
            rect.y += lineHeightSpace;

            EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, rect.width / 4, lineHeight), string.Format("Max Height ", index));
            objects[index].MaxHeightPercentage = EditorGUI.Slider(new Rect(rect.x + 160, rect.y, EditorGUIUtility.currentViewWidth / 3, lineHeight),
                objects[index].MaxHeightPercentage, 0f, 1f);
            rect.y += lineHeightSpace;

            EditorGUI.LabelField(new Rect(rect.x + 60, rect.y, rect.width / 4, lineHeight), string.Format("Min Height ", index));
            objects[index].MinHeightPercentage = EditorGUI.Slider(new Rect(rect.x + 160, rect.y, EditorGUIUtility.currentViewWidth / 3, lineHeight),
                objects[index].MinHeightPercentage, 0f, 1f);
            rect.y += lineHeightSpace;

            objects[index].Offset = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, lineHeight), "Offset", objects[index].Offset);
            rect.y += lineHeightSpace;

            objects[index].Rotation = EditorGUI.Vector3Field(new Rect(rect.x, rect.y, rect.width, lineHeight), "Rotation", objects[index].Rotation);
            rect.y += lineHeightSpace;

        };


    }
}
