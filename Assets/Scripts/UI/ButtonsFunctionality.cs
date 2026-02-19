using UnityEngine;
using UnityEngine.SceneManagement;
//Shara script
public class ButtonsFunctionality : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu, settingsMenuMobile;

    // Play game button
    public void PlayGame()
    {
        SceneManager.LoadScene("Main Game");
    }

    // Quit game button
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettingsMainMenu()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            settingsMenuMobile.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(true);
        }
    }
}
