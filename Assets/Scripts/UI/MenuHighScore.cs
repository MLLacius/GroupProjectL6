using TMPro;
using UnityEngine;

//Shara script
public class MenuHighScore : MonoBehaviour
{ 
    [SerializeField] private TMP_Text highScoreText;

    //Function to set text values
    private void Start()
    {
        //find the saved highscore
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        highScoreText.text = "High Score: " + highScore.ToString();
    }
}
