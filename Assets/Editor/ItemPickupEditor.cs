using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ItemPickup))]
public class ItemPickupEditor : Editor
{
    SerializedProperty typeProp;
    SerializedProperty valueProp;
    SerializedProperty resetProp;
    SerializedProperty pointsProp;
    SerializedProperty colorProp;
    SerializedProperty scaleProp;
    SerializedProperty prefabProp;

    void OnEnable()
    {
        typeProp = serializedObject.FindProperty("type");
        valueProp = serializedObject.FindProperty("value");
        resetProp = serializedObject.FindProperty("reset");
        pointsProp = serializedObject.FindProperty("pointsValue");
        colorProp = serializedObject.FindProperty("pickupTextColor");
        scaleProp = serializedObject.FindProperty("pickupTextScale");
        prefabProp = serializedObject.FindProperty("pickupIconPrefab");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(typeProp, new GUIContent("Type"));

        EditorGUILayout.Space();
        switch ((ItemPickup.Type)typeProp.enumValueIndex)
        {
            case ItemPickup.Type.Health:
            case ItemPickup.Type.Oxygen:
                EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"));
                break;
            case ItemPickup.Type.ExtraJump:
                EditorGUILayout.PropertyField(resetProp, new GUIContent("Reset Jumps"));
                break;
            case ItemPickup.Type.ExtraOxygen:
                EditorGUILayout.PropertyField(valueProp, new GUIContent("Value"));
                EditorGUILayout.PropertyField(resetProp, new GUIContent("Reset Oxygen"));
                break;
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(pointsProp, new GUIContent("Points Value"));
        EditorGUILayout.PropertyField(colorProp, new GUIContent("Pickup Text Color"));
        EditorGUILayout.PropertyField(scaleProp, new GUIContent("Pickup Text Scale"));

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(prefabProp, new GUIContent("Pickup Icon Prefab"));

        serializedObject.ApplyModifiedProperties();
    }
}
