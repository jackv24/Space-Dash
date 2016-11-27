using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public Vector2 mobileUIResolution = new Vector2(640, 480);
    public GameObject quitAppButton;
    public GameObject servicesButtons;

    [Space()]
    public GameObject backgroundPanel;
    public GameObject optionsPanel;
    public GameObject pauseMenu;

    [Header("Resolution")]
    public Text resolutionText;
    public Dropdown resolutionDropdown;
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
    private bool hasResolutionChanged = false;
    private Resolution[] resolutions;
#endif

    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;

    [Header("Image Effects")]
    public Dropdown qualityDropdown;
    public Toggle bloomToggle;
    public Toggle vignetteToggle;
    public Toggle antialiasingToggle;

    [Header("Sound")]
    public Text musicText;
    private string musicTextString;

    public Slider musicSlider;

    public Text soundText;
    private string soundTextString;

    public Slider soundSlider;

    [Header("Finish Buttons")]
    public Button applyButton;
    public Button okayButton;
    public Button cancelButton;

    void Start()
    {
        //Add listeners to events
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
        resolutionDropdown.onValueChanged.AddListener(delegate { hasResolutionChanged = true; });
#endif

        musicSlider.onValueChanged.AddListener(delegate { UpdateSliders(); });
        soundSlider.onValueChanged.AddListener(delegate { UpdateSliders(); });

        applyButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ToggleOptions(); });

        //Cancel button should hide the UI, reset options, and reset options in UI
        cancelButton.onClick.AddListener(delegate { ToggleOptions(); OptionsManager.instance.ApplyOptions(); LoadOptions(); });

        //Load resolutions
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
        resolutions = Screen.resolutions;

        resolutionDropdown.options.Clear();

        //Fill dropdown with available resolutions
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolutions[i].ToString()));
        }

        servicesButtons.SetActive(false);
#endif
#if UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
        resolutionText.gameObject.SetActive(false);
        resolutionDropdown.gameObject.SetActive(false);
        fullscreenToggle.gameObject.SetActive(false);
        vSyncToggle.gameObject.SetActive(false);
#endif
#if UNITY_ANDROID || UNITY_IOS
        GetComponent<CanvasScaler>().referenceResolution = mobileUIResolution;
#endif
#if UNITY_WEBGL
        quitAppButton.SetActive(false);
        servicesButtons.SetActive(false);
#endif

        qualityDropdown.options.Clear();

        foreach (string name in QualitySettings.names)
            qualityDropdown.options.Add(new Dropdown.OptionData(name));

        //Cache text for formatting
        soundTextString = soundText.text;
        musicTextString = musicText.text;

        OptionsManager options = OptionsManager.instance;

        //Update UI at start
        LoadOptions();
        UpdateSliders();

        //Hide options menu to start with
        //Disable all children (hides UI)
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && pauseMenu)
        {
            TogglePauseMenu();
        }
    }

    public void ApplyChanges()
    {
        OptionsManager options = OptionsManager.instance;

        //Only change resolution if dropdown value has changed, otherwise it may be wrong
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
        if(hasResolutionChanged)
            options.currentOptions.screenResolution = resolutions[resolutionDropdown.value];
        options.currentOptions.isFullScreen = fullscreenToggle.isOn;
        options.currentOptions.vSyncOn = vSyncToggle.isOn;
#endif

        //Image effects
        options.currentOptions.hasBloom = bloomToggle.isOn;
        options.currentOptions.hasVignette = vignetteToggle.isOn;
        options.currentOptions.hasAntialiasing = antialiasingToggle.isOn;

        options.currentOptions.qualityLevel = qualityDropdown.value;

        //Sounds
        options.currentOptions.musicVolume = musicSlider.value;
        options.currentOptions.gameVolume = soundSlider.value;

        //Save options to file
        options.SaveOptions();
        //Apply options to game
        options.ApplyOptions();
    }

    void UpdateSliders()
    {
        musicText.text = string.Format(musicTextString, (musicSlider.value * 100));
        soundText.text = string.Format(soundTextString, (soundSlider.value * 100));

        //Preview sound level before saving
        OptionsManager.instance.SetMusicVolume(musicSlider.value);
        OptionsManager.instance.SetGameVolume(soundSlider.value);
    }

    public void ToggleOptions()
    {
        bool isVisible = !optionsPanel.activeSelf;

        optionsPanel.SetActive(isVisible);

        if (GameObject.FindWithTag("Player") == null)
            backgroundPanel.SetActive(isVisible);
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu && GameManager.instance)
        {
            bool isVisible = !pauseMenu.activeSelf;

            GameManager.instance.IsGamePaused = isVisible;
            SoundManager.instance.playerLoopSource.enabled = !isVisible;

            pauseMenu.SetActive(isVisible);
            backgroundPanel.SetActive(isVisible);

            //Make sure all options panels are hidden when needed
            if (!isVisible)
            {
                //Disable all children (hides UI)
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).gameObject.SetActive(false);
            }
        }
#if UNITY_ANDROID || UNITY_IOS
        else
            Application.Quit();
#endif
    }

    public void ResetPrefs()
    {
        PlayerPrefs.DeleteAll();

        if (HUDControl.instance)
            HUDControl.instance.bestScore = 0;
    }

    public void LoadOptions()
    {
        Options options = OptionsManager.instance.currentOptions;

        //Load options into UI elements
#if !UNITY_ANDROID && !UNITY_IOS && !UNITY_WEBGL
        resolutionDropdown.captionText.text = options.screenResolution.ToString();
        fullscreenToggle.isOn = options.isFullScreen;
        vSyncToggle.isOn = options.vSyncOn;
#endif

        bloomToggle.isOn = options.hasBloom;
        vignetteToggle.isOn = options.hasVignette;
        antialiasingToggle.isOn = options.hasAntialiasing;

        qualityDropdown.value = options.qualityLevel;
        qualityDropdown.captionText.text = QualitySettings.names[options.qualityLevel];

        musicSlider.value = options.musicVolume;
        soundSlider.value = options.gameVolume;
    }
}
