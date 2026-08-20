using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
//Mainly a Leyton script, Luke helped fix a few bugs
public class SettingsMenu : MonoBehaviour
{
    //declare the values used for the components
    private const int minimumVolume = 0;
    private const int maximumVolume = 100;
    private const int startingVolume = 50;
    private int defaultResolutionIndex;
    private List<int> resolutionsX;
    private List<int> resolutionsY;
    private List<string> displayResolutions;
    private Resolution defaultResolution;

    FullScreenMode currentFullScreenMode;

    //declare the components
    public Slider volumeSlider;
    public Slider musicSlider;

    //Serialized all of these instead of setting values in awake 
    //So we can split settings menu from the manager to set default values without having to re-open the menu
    [SerializeField] private TextMeshProUGUI volumeDisplayText;
    [SerializeField] private TextMeshProUGUI musicDisplayText;
    [SerializeField] private TMP_Dropdown qualityOptions;
    [SerializeField] private Button windowToggle;
    [SerializeField] private Button resetToggle;
    [SerializeField] private Button controlsToggle;
    [SerializeField] private TextMeshProUGUI controlSchemeDisplay;
    [SerializeField] private ControlSchemeManager controlSchemeManagerScript;
    [SerializeField] private TextMeshProUGUI windowDisplayText;
    [SerializeField] private AudioSource gameMusic;

    private void Start()
    {
        //set intital values and proporties of components
        volumeSlider.minValue = minimumVolume;
        volumeSlider.maxValue = maximumVolume;
        volumeSlider.value = startingVolume;
        volumeSlider.wholeNumbers = true;

        musicSlider.minValue = minimumVolume;
        musicSlider.maxValue = maximumVolume;
        musicSlider.value = startingVolume;
        musicSlider.wholeNumbers = true;

        resolutionsX = new List<int>();
        resolutionsY = new List<int>();
        displayResolutions = new List<string>();
        defaultResolution = Screen.currentResolution;

        //reset the resolution options when the settings are loaded
        resolutionsX.Clear();
        resolutionsY.Clear();
        displayResolutions.Clear();
        qualityOptions.ClearOptions();
        FindResolutions();
        qualityOptions.AddOptions(displayResolutions);
        qualityOptions.value = defaultResolutionIndex;

        //assign unity events
        SetWindow();
        SetControlScheme();
        qualityOptions.onValueChanged.AddListener(ChangeResolution);
        windowToggle.onClick.AddListener(ChangeWindow);
        resetToggle.onClick.AddListener(SetDefaultSettings);
        controlsToggle.onClick.AddListener(ChangeControlScheme);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);

        if(PlayerPrefs.HasKey("Volume") ||PlayerPrefs.HasKey("Music Volume") || PlayerPrefs.HasKey("Resolution") || PlayerPrefs.HasKey("ResolutionX") || PlayerPrefs.HasKey("ResolutionY"))
        {
            LoadPlayerPrefs();
        }
    }

    private void Update()
    {
        if (volumeSlider.value == 0)
        {
            volumeDisplayText.text = "X";
        }
        else
        {
            volumeDisplayText.text = volumeSlider.value.ToString();
        }

        if(musicSlider.value == 0)
        {
            musicDisplayText.text = "X";
        }
        else
        {
            musicDisplayText.text = musicSlider.value.ToString();
        }
    }

    public void ChangeVolume(float volume)
    {
        volume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value / 100;
        Debug.Log("Current volume: " + volume + " AudioListener value: " + AudioListener.volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void ChangeMusicVolume(float musicVolume)
    {
        musicVolume = musicSlider.value;
        gameMusic.volume = musicSlider.value / 100;
        Debug.Log("Current music volume: " + musicVolume);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }

    private void FindResolutions()
    {
        Resolution[] supportedResolutions = Screen.resolutions;

        //print all of the display's supported resolutions and values into lists
        foreach (Resolution resolution in supportedResolutions)
        {
            if(resolutionsX.Count == 0)
            {
                resolutionsX.Add(resolution.width);
                resolutionsY.Add(resolution.height);
                displayResolutions.Add(resolution.width.ToString() + " x " + resolution.height.ToString());
            }
            //only add resolutions with different width and height values to avoid redisplaying the same ones for different refresh rates (refresh rates are not displayed on dropdown)
            while (resolutionsX.Count >= 1 && !(resolution.width == resolutionsX[resolutionsX.Count - 1] && resolution.height == resolutionsY[resolutionsY.Count - 1]))
            {
                resolutionsX.Add(resolution.width);
                resolutionsY.Add(resolution.height);
                displayResolutions.Add(resolution.width.ToString() + " x " + resolution.height.ToString());

                if (resolutionsX.Count >= 1 && resolutionsX[resolutionsX.Count - 1] == defaultResolution.width && resolutionsY[resolutionsY.Count - 1] == defaultResolution.height)
                {
                    defaultResolutionIndex = resolutionsX.Count - 1;
                }
            }
        }
    }

    public void ChangeResolution(int displayResolutionIndex)
    {
        currentFullScreenMode = Screen.fullScreenMode;
        displayResolutionIndex = qualityOptions.value;
        Screen.SetResolution(resolutionsX[displayResolutionIndex], resolutionsY[displayResolutionIndex], currentFullScreenMode);
        PlayerPrefs.SetString("Resolution", displayResolutions[displayResolutionIndex]);
        PlayerPrefs.SetInt("ResolutionX", resolutionsX[displayResolutionIndex]);
        PlayerPrefs.SetInt("ResolutionY", resolutionsY[displayResolutionIndex]);
        Debug.Log(Screen.currentResolution.ToString());
        Debug.Log(resolutionsX[displayResolutionIndex] + " X " + resolutionsY[displayResolutionIndex] + ", " + currentFullScreenMode);
    }

    public void ChangeWindow()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            currentFullScreenMode = FullScreenMode.Windowed;
            windowDisplayText.text = "Windowed";
            Debug.Log("Set as windowed");
        }
        else if(Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            currentFullScreenMode = FullScreenMode.FullScreenWindow;
            windowDisplayText.text = "Fullscreen";
            Debug.Log("Set as fullscreen");
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            currentFullScreenMode = FullScreenMode.FullScreenWindow;
            windowDisplayText.text = "Fullscreen";
            Debug.Log("Set as fullscreen");
        }
    }

    public void ChangeControlScheme()
    {
        controlSchemeManagerScript.ChangeControlScheme();
        SetControlScheme();
    }

    private void SetControlScheme()
    {
        switch(controlSchemeManagerScript.GetControlScheme())
        {
            case 0:
                controlSchemeDisplay.text = "WASD";
                break;
            case 1:
                controlSchemeDisplay.text = "Arrow Keys";
                break;
            default:
                //failsafe to set control scheme back to WASD if int value does not match to an existing one
                ChangeControlScheme();
                break;
        }
    }

    private void SetWindow()
    {
        if(Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            windowDisplayText.text = "Fullscreen";
        }
        else if(Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            windowDisplayText.text = "Windowed";
        }
        else
        {
            //default to fullscreen settings if fullscreen mode somehow becomes an unintended option (ExclusiveFullScreen or MaximizedWindow settings)
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            currentFullScreenMode = FullScreenMode.FullScreenWindow;
            windowDisplayText.text = "Fullscreen";
        }
    }
    public void SetDefaultSettings()
    {
        float volume = volumeSlider.value;
        PlayerPrefs.DeleteKey("Volume");
        PlayerPrefs.DeleteKey("Resolution");
        PlayerPrefs.DeleteKey("ResolutionX");
        PlayerPrefs.DeleteKey("ResolutionY");
        volumeSlider.value = startingVolume;
        musicSlider.value = startingVolume;
        ChangeVolume(volume);
        ChangeMusicVolume(volume);

        Screen.SetResolution(defaultResolution.width, defaultResolution.height, currentFullScreenMode);
        PlayerPrefs.SetString("Resolution", (defaultResolution.width.ToString() + "x" + defaultResolution.height.ToString()));
        PlayerPrefs.SetInt("ResolutionX", defaultResolution.width);
        PlayerPrefs.SetInt("ResolutionY", defaultResolution.height);

        Debug.Log(defaultResolution);

        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        currentFullScreenMode = FullScreenMode.FullScreenWindow;
        windowDisplayText.text = "Fullscreen";
        qualityOptions.value = defaultResolutionIndex;

        controlSchemeManagerScript.ResetToDefault();
        controlSchemeDisplay.text = "WASD";
    }

    private void LoadPlayerPrefs()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        musicSlider.value = PlayerPrefs.GetFloat("Music Volume");
        ChangeVolume(PlayerPrefs.GetFloat("Volume"));
        ChangeMusicVolume(PlayerPrefs.GetFloat("Music Volume"));
        Screen.SetResolution(PlayerPrefs.GetInt("ResolutionX"), PlayerPrefs.GetInt("ResolutionY"), currentFullScreenMode);
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}