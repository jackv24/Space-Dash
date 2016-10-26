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

            //Persist between scenes
            DontDestroyOnLoad(gameObject);

            //Add filename onto datapath
            dataPath = Application.persistentDataPath + "/" + fileName + ".xml";

            LoadOptions();
        }
    }

    public void ApplyOptions()
    {
        //Apply screen options
        Resolution res = currentOptions.screenResolution;
        Screen.SetResolution(res.width, res.height, currentOptions.isFullScreen);

        //Enable or disable components on camera for imageeffects
        Camera.main.GetComponent<Bloom>().enabled = currentOptions.hasBloom;
        Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = currentOptions.hasVignette;
        Camera.main.GetComponent<Antialiasing>().enabled = currentOptions.hasAntialiasing;

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
    public Resolution screenResolution = Screen.resolutions[Screen.resolutions.Length - 1];
    public bool isFullScreen = true;
    
    //By default all image effects are on
    public bool hasBloom = true;
    public bool hasVignette = true;
    public bool hasAntialiasing = true;

    //Volume is max by default
    public float musicVolume = 1f;
    public float gameVolume = 1f;
}