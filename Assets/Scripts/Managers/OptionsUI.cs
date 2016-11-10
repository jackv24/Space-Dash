using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    public GameObject backgroundPanel;
    public GameObject optionsPanel;
    public GameObject pauseMenu;

    [Header("Resolution")]
    public Dropdown resolutionDropdown;
    private bool hasResolutionChanged = false;
    private Resolution[] resolutions;

    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;

    [Header("Image Effects")]
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
        resolutionDropdown.onValueChanged.AddListener(delegate { hasResolutionChanged = true; });

        musicSlider.onValueChanged.AddListener(delegate { UpdateSliders(); });
        soundSlider.onValueChanged.AddListener(delegate { UpdateSliders(); });

        applyButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ToggleOptions(); });

        //Cancel button should hide the UI, reset options, and reset options in UI
        cancelButton.onClick.AddListener(delegate { ToggleOptions(); OptionsManager.instance.ApplyOptions(); LoadOptions(); });

        //Load resolutions
        resolutions = Screen.resolutions;

        resolutionDropdown.options.Clear();

        //Fill dropdown with available resolutions
        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolutions[i].ToString()));
        }

        //Cache text for formatting
        soundTextString = soundText.text;
        musicTextString = musicText.text;

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
        if(hasResolutionChanged)
            options.currentOptions.screenResolution = resolutions[resolutionDropdown.value];
        options.currentOptions.isFullScreen = fullscreenToggle.isOn;
        options.currentOptions.vSyncOn = vSyncToggle.isOn;

        //Image effects
        options.currentOptions.hasBloom = bloomToggle.isOn;
        options.currentOptions.hasVignette = vignetteToggle.isOn;
        options.currentOptions.hasAntialiasing = antialiasingToggle.isOn;

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
        SoundManager.instance.SetMusicVolume(musicSlider.value);
        SoundManager.instance.SetGameVolume(soundSlider.value);
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
        if (pauseMenu)
        {
            bool isVisible = !pauseMenu.activeSelf;

            GameManager.instance.isGamePaused = isVisible;

            pauseMenu.SetActive(isVisible);

            backgroundPanel.SetActive(isVisible);
        }
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
        resolutionDropdown.captionText.text = options.screenResolution.ToString();
        fullscreenToggle.isOn = options.isFullScreen;
        vSyncToggle.isOn = options.vSyncOn;

        bloomToggle.isOn = options.hasBloom;
        vignetteToggle.isOn = options.hasVignette;
        antialiasingToggle.isOn = options.hasAntialiasing;

        musicSlider.value = options.musicVolume;
        soundSlider.value = options.gameVolume;
    }
}
