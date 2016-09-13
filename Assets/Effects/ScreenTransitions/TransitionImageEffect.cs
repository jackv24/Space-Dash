using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TransitionImageEffect : MonoBehaviour
{
    public Material effectMaterial;

    [Space()]
    [Range(0f, 1f)]
    public float cutoffAmount = 0f;
    [Range(0f, 1f)]
    public float lerpSpeed = 0.25f;

    [Space()]
    public float waitTime = 2f;

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
        while (cutoffAmount < 0.98f)
        {
            cutoffAmount = Mathf.Lerp(cutoffAmount, 1, lerpSpeed);

            yield return new WaitForEndOfFrame();
        }

        //Wait for a bit
        yield return new WaitForSeconds(waitTime);

        //Play transition backward
        while (cutoffAmount > 0.02f)
        {
            cutoffAmount = Mathf.Lerp(cutoffAmount, 0, lerpSpeed);

            yield return new WaitForEndOfFrame();
        }

        //Make sure cutoff is at zero to avoid small artifacts
        cutoffAmount = 0;
    }
}
