using UnityEngine;
using System.Collections;

public class LavaEffect : MonoBehaviour
{
    public AnimationCurve rocksMovementX;
    public AnimationCurve rocksMovementZ;

    public float rocksSpeed = 1f;

    [Space()]
    public AnimationCurve lavaMovementX;
    public AnimationCurve lavaMovementZ;

    public float lavaSpeed = 1f;

    private float rockTime = 0;
    private float lavaTime = 0;

    private Material mat;

    void Awake()
    {
        //Each lava flow might move differently, so the material will be instanced
        mat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        //Make timing loop back to 1, and scale with speed
        rockTime += Time.deltaTime * rocksSpeed;
        if (rockTime > 1f)
            rockTime = 0;

        lavaTime += Time.deltaTime * lavaSpeed;
        if (lavaTime > 1f)
            lavaTime = 0;

        //Set the texture offsets
        mat.SetTextureOffset("_TopTex", new Vector2(rocksMovementX.Evaluate(rockTime), rocksMovementZ.Evaluate(rockTime)));
        mat.SetTextureOffset("_MainTex", new Vector2(lavaMovementX.Evaluate(lavaTime), lavaMovementZ.Evaluate(lavaTime)));
    }
}
