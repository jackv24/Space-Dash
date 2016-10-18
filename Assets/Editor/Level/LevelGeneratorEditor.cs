using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    int previewLength = 20;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Preview", EditorStyles.boldLabel);

        previewLength = EditorGUILayout.IntField("Preview Length", previewLength);

        if (GUILayout.Button("Generate Preview"))
        {
            ((LevelGenerator)target).GeneratePreview(previewLength);
        }
        if (GUILayout.Button("Clear Preview"))
        {
            ((LevelGenerator)target).Reset();
        }
    }
}
