using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityStandardAssets.ImageEffects;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    public Options currentOptions;

    public string fileName = "game-options";
    private string dataPath;

    void Awake()
    {
        //If an options manager already exists, this one is not needed
        if (instance)
            Destroy(gameObject);
        //If there is no options manager, use this one
        else
        {
            instance = this;

            //Add filename onto datapath
            dataPath = Application.persistentDataPath + "/" + fileName + ".xml";

            LoadOptions();
        }
    }

    void Start()
    {
        ApplyOptions();
    }

    public void ApplyOptions()
    {
        //Apply screen options
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
        Resolution res = currentOptions.screenResolution;
        Screen.SetResolution(res.width, res.height, currentOptions.isFullScreen);
        QualitySettings.vSyncCount = currentOptions.vSyncOn ? 1 : 0;
#endif

        //Enable or disable components on camera for imageeffects
        Camera.main.GetComponent<Bloom>().enabled = currentOptions.hasBloom;
        Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = currentOptions.hasVignette;

        //Set aa on all cameras
        Antialiasing[] aaObjects = FindObjectsOfType<Antialiasing>();
        foreach (var aa in aaObjects)
            aa.enabled = currentOptions.hasAntialiasing;

        QualitySettings.SetQualityLevel(currentOptions.qualityLevel, true);

        //If there is a soundmanager
        if (SoundManager.instance)
        {
            //Set volumes
            SoundManager.instance.SetMusicVolume(currentOptions.musicVolume);
            SoundManager.instance.SetGameVolume(currentOptions.gameVolume);
        }
    }

    public void SaveOptions()
    {
        //Dont save in editor
        if (!Application.isEditor)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Options));

            //Save options to xml file
            using (FileStream stream = new FileStream(dataPath, FileMode.Create))
            {
                serializer.Serialize(stream, currentOptions);
            }
        }
    }

    public void LoadOptions()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Options));

        //Check if file exists
        if (!Application.isEditor && File.Exists(dataPath))
        {
            //Load options from xml
            using (FileStream stream = new FileStream(dataPath, FileMode.Open))
                currentOptions = (Options)serializer.Deserialize(stream);
        }
        else
            //If file does not exist, create a new one with default options
            currentOptions = new Options();
    }
}

public class Options
{
    //Default screen resolution is the max available resolution

#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
    public Resolution screenResolution = Screen.resolutions[Screen.resolutions.Length - 1];
    public bool isFullScreen = true;
    public bool vSyncOn = true;

    //By default all image effects are on
    public bool hasBloom = true;
    public bool hasVignette = true;
    public bool hasAntialiasing = true;

    //Max quality by default
    public int qualityLevel = QualitySettings.names.Length - 1;
#endif

#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
    //By default all image effects are off for mobile
    public bool hasBloom = false;
    public bool hasVignette = false;
    public bool hasAntialiasing = false;

    //Worse quality by default on mobile
    public int qualityLevel = 0;
#endif

    //Volume is max by default
    public float musicVolume = 1f;
    public float gameVolume = 1f;
}