using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
    void OnSceneGUI()
    {
        //Can only move end position handle if the game is not playing
        if (!Application.isPlaying)
        {
            MovingPlatform t = target as MovingPlatform;

            EditorGUI.BeginChangeCheck();

            //Draw handle to move end position of platform
            Vector2 endPosition = Handles.PositionHandle(t.offset + (Vector2)t.transform.position, Quaternion.identity);

            //If was edited
            if (EditorGUI.EndChangeCheck())
            {
                //Record undo step
                Undo.RecordObject(target, "Changed Moving Platform Offset");

                //Update platform with new value
                t.offset = endPosition - (Vector2)t.transform.position;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        //Draw original fields, with additional below
        base.OnInspectorGUI();

        MovingPlatform t = target as MovingPlatform;

        //The time it takes the platform to move to the end
        float travelTime = t.offset.magnitude / t.moveSpeed;

        EditorGUILayout.Space();
        //Display travel time as a read-only label
        EditorGUILayout.LabelField(new GUIContent("Travel Time", "How long the platform takes to move from start to the end position."), new GUIContent(travelTime.ToString() + " seconds"));
    }

}
