using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ItemPickup))]
public class ItemPickupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ItemPickup t = (ItemPickup)target;

        t.type = (ItemPickup.Type)EditorGUILayout.EnumPopup("Type", t.type);

        EditorGUILayout.Space();
        switch (t.type)
        {
            case ItemPickup.Type.Health:
            case ItemPickup.Type.Oxygen:
                t.value = EditorGUILayout.IntField("Value", t.value);
                break;
            case ItemPickup.Type.ExtraJump:
                t.reset = EditorGUILayout.Toggle("Reset Jumps", t.reset);
                break;
        }

        EditorGUILayout.Space();
        t.pointsValue = EditorGUILayout.IntField("Points Value", t.pointsValue);
    }
}
