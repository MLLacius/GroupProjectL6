using UnityEngine;
using TMPro;
using System.Collections;
//Luke, Shara and Leyton script
public class ScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameMaster gameMaster;

    // Update is called once per frame
    void Update()
    {
        if(gameMaster && scoreText &&  highScoreText)
        {
            scoreText.text = "Score: " + gameMaster.GetCurrentScore().ToString();
            if (gameMaster.HasAchievedHighScore())
            {
                highScoreText.text = "High Score: " + gameMaster.GetHighScore().ToString();
            }
        }
    }

    public void EnableUI()
    {
        int highScore = gameMaster.GetHighScore();
        highScoreText.text = "High Score: " + highScore.ToString();
        Debug.Log(highScore);
    }

    public void HighScoreAchievedGameplay()
    {
        Color startingColor = highScoreText.color;
        StartCoroutine(FlashHighScore());
        highScoreText.color = startingColor;
    }

    private IEnumerator FlashHighScore()
    {


        yield return null;
    }
}
