using UnityEngine;
using TMPro;
using System.Collections;
//Luke, Shara and Leyton script
public class ScoreText : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameMaster gameMaster;

    private void Start()
    {
        highScoreText.canvasRenderer.SetAlpha(0f);
    }

    // Update is called once per frame
    void Update()
    {
        if(gameMaster && scoreText &&  highScoreText)
        {
            scoreText.text = "Score: " + gameMaster.GetCurrentScore().ToString();
            /*
            if (gameMaster.HasAchievedHighScore())
            {
                highScoreText.text = "High Score: " + gameMaster.GetHighScore().ToString();
            }
            */
        }
    }

    public void EnableUI()
    {
        /*
        int highScore = gameMaster.GetHighScore();
        highScoreText.text = "High Score: " + highScore.ToString();
        Debug.Log(highScore);
        */
    }

    public void HighScoreAchievedGameplay()
    {
        StartCoroutine(FlashHighScore());
    }

    private IEnumerator FlashHighScore()
    {
        Color startingColor = scoreText.color;
        scoreText.color = new Color(0.918f, 0.718f, 0f);
        for (int i = 0; i < 6; i++)
        {
            highScoreText.canvasRenderer.SetAlpha(1f);
            yield return new WaitForSeconds(0.3f);
            highScoreText.canvasRenderer.SetAlpha(0f);
            yield return new WaitForSeconds(0.3f);
        }

        scoreText.color = startingColor;
        yield return null;
    }
}
