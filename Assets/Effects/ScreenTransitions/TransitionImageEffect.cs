using UnityEngine;
using System.Collections;

public class TransitionImageEffect : MonoBehaviour
{
    public Material effectMaterial;

    public Texture2D[] transitionTextures;

    [Space()]
    [Range(0f, 1f)]
    public float cutoffAmount = 0f;
    public float transitionTime = 0.5f;
    public AnimationCurve transitionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    [Space()]
    public float waitTime = 0f;

    [Space()]
    public bool playStart = true;
    public bool playEnd = true;

    void OnDisable()
    {
        effectMaterial.SetFloat("_Cutoff", 0);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Update material cutoff amount to match script
        effectMaterial.SetFloat("_Cutoff", cutoffAmount);

        //Replace screen with imageeffect
        Graphics.Blit(src, dst, effectMaterial);
    }

    public void PlayTransition()
    {
        effectMaterial.SetTexture("_TransitionTexture", transitionTextures[Random.Range(0, transitionTextures.Length)]);

        StartCoroutine("Transition");
    }

    //Plays the transition
    IEnumerator Transition()
    {
        //Play transition forward
        if (playStart)
        {
            float time = 0;
            while (time <= transitionTime)
            {
                time += Time.deltaTime;

                cutoffAmount = transitionCurve.Evaluate(time / transitionTime);

                yield return new WaitForEndOfFrame();
            }

            //Wait for a bit
            yield return new WaitForSeconds(waitTime);
        }

        if (playEnd)
        {
            //Play transition backward
            float time = 1;
            while (time >= 0)
            {
                time -= Time.deltaTime;

                cutoffAmount = transitionCurve.Evaluate(time / transitionTime);

                yield return new WaitForEndOfFrame();
            }

            //Make sure cutoff is at zero to avoid small artifacts
            cutoffAmount = 0;
        }
    }
}
