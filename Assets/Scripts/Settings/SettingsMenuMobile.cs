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

    private void Start()
    {
        //set intital values and proporties of components
        volumeSlider.minValue = minimumVolume;
        volumeSlider.maxValue = maximumVolume;
        volumeSlider.value = startingVolume;
        volumeSlider.wholeNumbers = true;

        //assign unity events
        resetToggle.onClick.AddListener(SetDefaultSettings);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);

        if(PlayerPrefs.HasKey("Volume"))
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
            musicDisplayText.text = Convert.ToInt32(musicSlider.value * 100).ToString();
        }
    }

    public void ChangeVolume(float volume)
    {
        volume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value / 100;
        Debug.Log("Current volume: " + volume + " AudioListener value: " + AudioListener.volume);
        PlayerPrefs.SetFloat("Volume", volume);
    }

    public void SetDefaultSettings()
    {
        float volume = volumeSlider.value;
        PlayerPrefs.DeleteKey("Volume");
        volumeSlider.value = startingVolume;
        ChangeVolume(volume);

        musicSlider.value = 0.5f;
    }

    private void LoadPlayerPrefs()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        ChangeVolume(PlayerPrefs.GetFloat("Volume"));
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}