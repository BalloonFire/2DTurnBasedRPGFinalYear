using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Required for audio control

public class SettingsManager : MonoBehaviour
{
    [Header("Volume Settings")]
    //public AudioMixer audioMixer; // Assign your Audio Mixer in the Inspector
    public Slider volumeSlider;

    [Header("Brightness Settings")]
    public Image brightnessOverlay; // Assign a semi-transparent black UI Image
    public Slider brightnessSlider;

    [Header("Graphic Settings")]
    public Dropdown GraphicDropDown;

    [Header("Resolution Settings")]
    public Dropdown ResolutionDropdown;

    void Start()
    {
        // Load saved settings (if any)
        LoadSettings();
    }

    // VOLUME CONTROL
    public void SetVolume(float volume)
    {
        float minVolume = 0.0001f; // Avoid log(0)
        float adjustedVolume = Mathf.Max(volume, minVolume);
        //audioMixer.SetFloat("MasterVolume", Mathf.Log10(adjustedVolume) * 20);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    //GRAPHIC CONTROL
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    //RESOLUTION CONTROL
    public void SetResolution(int resolutionIndex)
    {
        // Get all available screen resolutions
        Resolution[] resolutions = Screen.resolutions;

        // Check if index is valid
        if (resolutionIndex < 0 || resolutionIndex >= resolutions.Length)
        {
            Debug.LogWarning($"Invalid resolution index: {resolutionIndex}");
            resolutionIndex = resolutions.Length - 1; // Default to highest resolution
        }

        // Get the selected resolution
        Resolution selectedResolution = resolutions[resolutionIndex];

        // Apply the resolution
        Screen.SetResolution(
            selectedResolution.width,
            selectedResolution.height,
            Screen.fullScreen
        );
    }

    // BRIGHTNESS CONTROL
    public void SetBrightness(float brightness)
    {
        if (brightnessOverlay == null) return; // Add this line
        Color overlayColor = brightnessOverlay.color;
        overlayColor.a = 1 - brightness;
        brightnessOverlay.color = overlayColor;
        PlayerPrefs.SetFloat("Brightness", brightness);
    }

    // LOAD SAVED SETTINGS
    private void LoadSettings()
    {
        //Graphic 
        QualitySettings.SetQualityLevel(0);

        //Resolution
        

        // Volume
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f); // Default 75%
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);

        // Brightness
        float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0.8f); // Default 80%
        brightnessSlider.value = savedBrightness;
        SetBrightness(savedBrightness);
    }

    // RESET TO DEFAULT (Optional: Call this from a button)
    public void ResetToDefault()
    {
        volumeSlider.value = 0.75f;
        brightnessSlider.value = 0.8f;
        // Apply the values immediately
        SetVolume(0.75f);
        SetBrightness(0.8f);
    }
}