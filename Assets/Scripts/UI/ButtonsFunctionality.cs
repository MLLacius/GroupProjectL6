using UnityEngine;
using UnityEngine.SceneManagement;
//Shara script
public class ButtonsFunctionality : MonoBehaviour
{
    // Play game button
    public void PlayGame()
    {
        SceneManager.LoadScene("Main Game");
    }

    // Quit game button
    public void QuitGame()
    {
        Debug.Log("Exit!");
        Application.Quit();
    }
}
