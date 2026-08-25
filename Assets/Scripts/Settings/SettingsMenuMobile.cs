using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
//Mainly a Leyton script, Luke helped fix a few bugs
public class SettingsMenuMobile : MonoBehaviour
{
    //declare the values used for the components
    private const int minimumVolume = 0;
    private const int maximumVolume = 100;
    private const int startingVolume = 50;

    //declare the components
    public Slider volumeSlider;
    public Slider musicSlider;

    //Serialized all of these instead of setting values in awake 
    //So we can split settings menu from the manager to set default values without having to re-open the menu
    [SerializeField] private TextMeshProUGUI volumeDisplayText;
    [SerializeField] private TextMeshProUGUI musicDisplayText;
    [SerializeField] private Button resetToggle;
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

        //assign unity events
        resetToggle.onClick.AddListener(SetDefaultSettings);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        musicSlider.onValueChanged.AddListener(ChangeMusicVolume);

        if(PlayerPrefs.HasKey("Volume") || PlayerPrefs.HasKey("Music Volume"))
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
        PlayerPrefs.SetFloat("Volume", volume);
    }
    public void ChangeMusicVolume(float musicVolume)
    {
        musicVolume = musicSlider.value;
        gameMusic.volume = musicSlider.value / 100;
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }

    public void SetDefaultSettings()
    {
        float volume = volumeSlider.value;
        PlayerPrefs.DeleteKey("Volume");
        volumeSlider.value = startingVolume;
        musicSlider.value = startingVolume;
        ChangeVolume(volume);
        ChangeMusicVolume(volume);
    }

    private void LoadPlayerPrefs()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        musicSlider.value = PlayerPrefs.GetFloat("Music Volume");
        ChangeVolume(PlayerPrefs.GetFloat("Volume"));
        ChangeMusicVolume(PlayerPrefs.GetFloat("Music Volume"));
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}