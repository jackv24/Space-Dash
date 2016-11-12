using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ItemSpawn))]
public class ItemSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Editor Preview", EditorStyles.boldLabel);

        if (GUILayout.Button("Preview"))
        {
            ((ItemSpawn)target).Spawn(false);
        }
    }
}
