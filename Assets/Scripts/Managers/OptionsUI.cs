using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionsUI : MonoBehaviour
{
    [Header("Resolution")]
    public Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    public Toggle fullscreenToggle;

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

    private bool isVisible = false;

    void Start()
    {
        //Add listeners to events
        musicSlider.onValueChanged.AddListener(delegate { UpdateUI(); });
        soundSlider.onValueChanged.AddListener(delegate { UpdateUI(); });

        applyButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ApplyChanges(); });
        okayButton.onClick.AddListener(delegate { ShowOrHide(); });

        //Load resolutions
        resolutions = Screen.resolutions;

        resolutionDropdown.options.Clear();

        for (int i = 0; i < resolutions.Length; i++)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolutions[i].ToString()));
            resolutionDropdown.value = i;
        }

        //Cache text for formatting
        soundTextString = soundText.text;
        musicTextString = musicText.text;

        //Update UI at start
        UpdateUI();

        if (transform.GetChild(0).gameObject.activeSelf)
        {
            isVisible = true;
            ShowOrHide();
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            ShowOrHide();
    }

    public void ApplyChanges()
    {
        Resolution res = resolutions[resolutionDropdown.value];

        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);
    }

    public void UpdateUI()
    {
        musicText.text = string.Format(musicTextString, (musicSlider.value * 100));
        soundText.text = string.Format(soundTextString, (soundSlider.value * 100));
    }

    public void ShowOrHide()
    {
        isVisible = !isVisible;

        transform.GetChild(0).gameObject.SetActive(isVisible);

        Time.timeScale = isVisible ? 0 : 1;
    }
}
