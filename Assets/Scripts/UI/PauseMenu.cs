using UnityEngine;
using UnityEngine.SceneManagement;

//Leyton script
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu, settingsMenuMobile, gameplayBackButton, gameplayBackButtonMobile;

    private bool isPauseMenuActive;

    private void Start()
    {
        isPauseMenuActive = false;
    }

    public void TogglePauseMenu()
    {
        isPauseMenuActive = !isPauseMenuActive;

        if (pauseButton.activeSelf)
        {
            pauseButton.SetActive(false);
            Time.timeScale = 0f;
        }

        if (pauseMenu.activeSelf)
        {
            pauseButton.SetActive(true);
            Time.timeScale = 1f;
        }

        pauseMenu.SetActive(isPauseMenuActive);
    }

    public void OpenSettingsPauseMenu()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            settingsMenuMobile.SetActive(true);
            gameplayBackButtonMobile.SetActive(true);
        }
        else
        {
            settingsMenu.SetActive(true);
            gameplayBackButton.SetActive(true);
        }
    }

    public void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
