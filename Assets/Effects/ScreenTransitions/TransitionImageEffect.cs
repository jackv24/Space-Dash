using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TransitionImageEffect : MonoBehaviour
{
    public Material effectMaterial;

    [Space()]
    [Range(0f, 1f)]
    public float cutoffAmount = 0f;

    public float transitionTime = 0.5f;

    [Space()]
    public float waitTime = 0f;

    [Space()]
    public bool playStart = true;
    public bool playEnd = true;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Update material cutoff amount to match script
        effectMaterial.SetFloat("_Cutoff", cutoffAmount);

        //Replace screen with imageeffect
        Graphics.Blit(src, dst, effectMaterial);
    }

    //Usually Called via sendmessage on the main camera
    public void PlayTransition()
    {
        StartCoroutine("Transition");
    }

    //Plays the transition
    IEnumerator Transition()
    {
        //Play transition forward
        if (playStart)
        {
            while (cutoffAmount < 0.98f)
            {
                cutoffAmount = Mathf.Lerp(cutoffAmount, 1, (1 / transitionTime) * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            //Wait for a bit
            yield return new WaitForSeconds(waitTime);
        }

        if (playEnd)
        {
            //Play transition backward
            while (cutoffAmount > 0.02f)
            {
                cutoffAmount = Mathf.Lerp(cutoffAmount, 0, (1 / transitionTime) * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }

            //Make sure cutoff is at zero to avoid small artifacts
            cutoffAmount = 0;
        }
    }
}
