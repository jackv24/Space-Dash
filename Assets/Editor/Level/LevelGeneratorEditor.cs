using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelGenerator t = target as LevelGenerator;

        base.OnInspectorGUI();

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Preview"))
        {
            t.Generate();
        }
    }
}
