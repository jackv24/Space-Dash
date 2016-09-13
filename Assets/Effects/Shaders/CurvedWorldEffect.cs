using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CurvedWorldEffect : MonoBehaviour
{
	[Tooltip("The degree of curvature of all meshes with a curvature shader")]
	public float  worldCurvature = 0.005f;

	[Tooltip("Should the curvature show while in the editor?\n(should be disabled for level editing")]
	public bool showInEditor = false;

	[Tooltip("The target that everything curves based on. Usually the player.")]
	public Transform target;

	void Update()
	{
        //Only curve if desired, or if game is playing
		if (Application.isPlaying || showInEditor)
		{
			Shader.SetGlobalFloat ("_Curvature", worldCurvature);

			Shader.SetGlobalVector ("_TargetPos", target.position);
		}
        //Otherwise set curvature to 0
		else
			Shader.SetGlobalFloat ("_Curvature", 0);
	}
}
