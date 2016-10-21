using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    //Static instance for easy access
    public static BackgroundManager instance;

    [Header("Renderers")]
    public MeshRenderer backgroundRenderer;
    public MeshRenderer starsRenderer;

    [Header("Backgrounds")]
    public float backgroundMoveX = 0.1f;
    private float backgroundOffsetX = 0;

    public float starsMoveX = 0.25f;
    private float starsOffsetX = 0;

    [Space()]
    public List<ParallaxBackground> backgrounds = new List<ParallaxBackground>();

    void Awake()
    {
        if (instance)
            Debug.LogWarning("A Background Manager already exists!");
        else
            instance = this;
    }

    void Start()
    {
        //Randomise background on start
        Randomise();
    }

    void Update()
    {
        //Check if there is a renderer
        if (backgroundRenderer)
        {
            //Move offset over time
            backgroundOffsetX += backgroundMoveX * Time.deltaTime;

            //Set material property
            backgroundRenderer.material.SetTextureOffset("_MainTex", new Vector2(backgroundOffsetX, 0));
        }

        //Same as backgroundRenderer
        if (starsRenderer)
        {
            starsOffsetX += starsMoveX * Time.deltaTime;

            starsRenderer.material.SetTextureOffset("_MainTex", new Vector2(starsOffsetX, 0));
        }
    }

    public void Randomise()
    {
        //Set random background from list
        SetBackground(backgrounds[Random.Range(0, backgrounds.Count)]);
    }

    void SetBackground(ParallaxBackground bg)
    {
        //Set maintex on material as background
        if (backgroundRenderer)
            backgroundRenderer.sharedMaterial.SetTexture("_MainTex", bg.background);

        //Set maintex on material as stars
        if (starsRenderer)
            starsRenderer.sharedMaterial.SetTexture("_MainTex", bg.stars);
    }
}

[System.Serializable]
public class ParallaxBackground
{
    public Texture2D background;
    public Texture2D stars;
}