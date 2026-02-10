using UnityEngine;
//Shara script
public class AudioPlayer : MonoBehaviour
{
    private static readonly string VolumePref = "VolumePref";
    private float volumeFloat;
    public AudioSource[] volumeAudio;
    public AudioSource volumeAudios;

    private void ContinueSettings()
    {
        volumeFloat = PlayerPrefs.GetFloat(VolumePref);

        volumeAudios.volume = volumeFloat;

        for (int i = 0; i < volumeAudio.Length; i++)
        {
            volumeAudio[i].volume = volumeFloat;
        }
    }
}
