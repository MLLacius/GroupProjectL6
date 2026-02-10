using UnityEngine;
using UnityEngine.UI;
//Shara script
public class Music : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string VolumePref = "VolumePref";
    private int firstPlayInt;
    public Slider volumeSlider;
    private float volumeFloat;
    public AudioSource[] volumeAudio;
    public AudioSource volumeAudios;

    private void Start()
    {
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        if (firstPlayInt == 0)
        {
            //telling the system that the volume needs to start at 0.5
            volumeFloat = 0.5f;
            volumeSlider.value = volumeFloat;
            PlayerPrefs.SetFloat(VolumePref, volumeFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else
        {
            volumeFloat = PlayerPrefs.GetFloat(VolumePref);
            volumeSlider.value = volumeFloat;
        }
    }

    public void SaveVolume()
    {
        //Allows you to save things over different scenes and play sessions
        PlayerPrefs.SetFloat(VolumePref, volumeSlider.value);
    }

    public void OnApplicationFocus(bool infocus)
    {
        //Saves all settings done and when you leave the game and come back the settings are the same as when you left
        if (!infocus)
        {
            SaveVolume();
        }
    }

    //this allows the player to control the volume through the slider
    public void UpdateVolume()
    {
        volumeAudios.volume = volumeSlider.value;

        for (int i = 0; i < volumeAudio.Length; i++)
        {
            volumeAudio[i].volume = volumeSlider.value;
        }
    }
}
