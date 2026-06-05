using UnityEngine;
using TMPro;
using System.Collections;
//Shara script; leyton added a few things later on
public class EndGameDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text lastScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text collectibleText;
    [SerializeField] private GameMaster gameMaster;

    //Function to set text values
    public void DisplayScores()
    {
        if (gameMaster.HasAchievedHighScore())
        {
            int lastScore = PlayerPrefs.GetInt("LastScore", 0);

            lastScoreText.text = "Score: " + lastScore.ToString();
            highScoreText.text = "New High Score!";
            collectibleText.text = "Collectibles Collected: " + gameMaster.GetCollectiblesGained();
            HighScoreEffects();
        }
        else
        {
            //find the last saved score
            int lastScore = PlayerPrefs.GetInt("LastScore", 0);
            int highScore = PlayerPrefs.GetInt("HighScore", 0);

            lastScoreText.text = "Score: " + lastScore.ToString();
            highScoreText.text = "High Score: " + highScore.ToString();
            collectibleText.text = "Collectibles Collected: " + gameMaster.GetCollectiblesGained();
        }
    }

    private void HighScoreEffects()
    {
        lastScoreText.color = Color.yellow;
        StartCoroutine(FlashHighScore());
    }

    private IEnumerator FlashHighScore()
    {
        while(true)
        {
            highScoreText.canvasRenderer.SetAlpha(0f);
            yield return new WaitForSeconds(0.5f);
            highScoreText.canvasRenderer.SetAlpha(1f);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
