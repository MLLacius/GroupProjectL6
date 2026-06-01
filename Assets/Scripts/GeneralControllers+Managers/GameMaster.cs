using UnityEngine;
using UnityEngine.Events;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
//Luke script mainly, shara contributed a few lines, Leyton added scoring/tutorial logic
public class GameMaster : MonoBehaviour
{
    enum GameState
    {
        MainMenu,
        Gameplay,
        GameOver,
        FirstTutorial
    }


    //Unity Events
    public UnityEvent OnHighScoreAchieved; //Called when the player gets a highscore in the current run
    public UnityEvent OnGameStart;
    public UnityEvent OnSuccessfulPurchase;
    public UnityEvent OnFailedPurchase;

    [SerializeField] private GameState gameState;
    [SerializeField] private CinemachineCamera cineCam;

    [Tooltip("0 = Easy, 1 = Normal, 2 = Hard")]
    [SerializeField, UnityEngine.Min(0), Max(2)] private int gameDifficultyID; 

    private float rawScore;
    private float scoreOffset;
    private float tutorialOffset;
    private bool gameplayStarted;
    private bool highScoreAchieved = false;
    private int currentScore = 0;
    private int highScore = 0;
    private int collectiblesGained = 0;

    private LevelSpawner levelSpawner;
    private PlayerMovement playerMovement;
    [SerializeField] PlayerDashAndDisplay PlayerDashAndDisplay;
    [SerializeField] TutorialStateManager tutorialStateManager;

    [SerializeField] private float dashScoreMultiplier = 3f;
    private float currentDashMultiplier = 1f;
    private float scoreMultiplier;
    [SerializeField] private UpgradeSciptableItem dashDestructionBonusUpgrade;
    [SerializeField] private UpgradeManager upgradeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rawScore = 0;
        scoreOffset = 0;
        levelSpawner = GameObject.Find("Level Spawner").GetComponent<LevelSpawner>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("Highscore:" + highScore.ToString());
        highScoreAchieved = false;
        if(!upgradeManager)
        {
            upgradeManager = GameObject.Find("Upgrades Manager").GetComponent<UpgradeManager>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameState == GameState.MainMenu)
        {
            gameplayStarted = false;
            scoreOffset = Time.time;
            return;
        }
        else if (gameState == GameState.Gameplay)
        {
            scoreMultiplier = levelSpawner.GetSpeed() / 10;
            rawScore += Time.deltaTime * (scoreMultiplier + currentDashMultiplier);
            currentScore = (int)rawScore;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                //Only fire high score achieved event once per run
                if (!highScoreAchieved)
                {
                    highScoreAchieved = true;
                    OnHighScoreAchieved.Invoke();
                }
            }
            UpdateDifficulty();
            return;
        }
        else if (gameState == GameState.FirstTutorial)
        {
            tutorialOffset = Time.time - scoreOffset;
            return;
        }
    }

    public void EnableDashScoreMultiplier()
    {
        currentDashMultiplier = dashScoreMultiplier;
    }
    public void DisableDashScoreMultiplier()
    {
        currentDashMultiplier = 1f;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        gameplayStarted = true;

        if (tutorialStateManager.GetIsFirstTutorial())
        {
            gameState = GameState.FirstTutorial;
        }
        else
        {
            gameState = GameState.Gameplay;
        }
        OnGameStart.Invoke();
        levelSpawner.UpdateSegmentCount();
    }

    private void OnDestroy()
    {
        SaveValues();
    }

    public bool HasAchievedHighScore()
    {
        return highScoreAchieved;
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public int GetLastScore()
    {
        return PlayerPrefs.GetInt("Last Score", 0);
    }

    public bool GetGameplayState()
    {
        return gameplayStarted;
    }

    public int GetCollectiblesGained()
    {
        return collectiblesGained;
    }

    public int GetDifficulty()
    {
        return gameDifficultyID;
    }

    //Returns true/false depending on whether the player can spend the requested amount of collectibles
    public bool TrySpendCollectibles(int amount)
    {
        int totalCollectibles = PlayerPrefs.GetInt("Collectibles", 0);
        if (totalCollectibles >= amount)
        {
            totalCollectibles -= amount;
            PlayerPrefs.SetInt("Collectibles", totalCollectibles);
            PlayerPrefs.Save();
            OnSuccessfulPurchase.Invoke();
            return true;
        }
        else
        {
            OnFailedPurchase.Invoke();
            return false;
        }
    }

    public void IncrementCollectiblesGained()
    {
        if (gameplayStarted)
        {
            collectiblesGained++;
            //Collecibles = Total collectibles collected across all runs
            PlayerPrefs.SetInt("Collectibles", PlayerPrefs.GetInt("Collectibles", 0) + 1);
            if (!playerMovement.GetIsPlayerDashing())
            {
                PlayerDashAndDisplay.IncrementCollectedCollectibles();
            }
        }
    }

    public void SaveValues()
    {
        if (highScoreAchieved)
        {
            PlayerPrefs.SetInt("HighScore", highScore);
        }
        PlayerPrefs.Save();
    }

    //This gets called when OnGameOver unity event in playerMovement.cs is invoked
    public void OnGameOver()
    {
        int lastScore = currentScore;
        SaveValues();
        //Save last score seperately to ensure the last score is always accurate and not saved mid run.
        PlayerPrefs.SetInt("Last Score", lastScore);
        PlayerPrefs.Save();
        gameState = GameState.GameOver;
        gameplayStarted = false;
    }

    //This only exists to leave the first tutorial state after the player completes it and start counting score.
    public void SetStateGameplay()
    {
        gameState = GameState.Gameplay;
    }

    public void AwardDashDestructionBonus()
    {
        int dashDestructionUpgradeLevel = upgradeManager.GetUpgradeCurrentLevel(dashDestructionBonusUpgrade.upgradeID);
        //Upgrade not owned, return
        if(dashDestructionUpgradeLevel == 0) { return; }
        //Else, its owned and get the upgrade value for that level
        float dashDestructionBonus = dashDestructionBonusUpgrade.GetValueForLevel(dashDestructionUpgradeLevel);
        Debug.Log("Dash destruction bonus: " + dashDestructionBonus.ToString());
        rawScore += dashDestructionBonus;
        return;
    }

    private void UpdateDifficulty()
    {
        switch (currentScore)
        {
            case 0:
                gameDifficultyID = 0;
                Debug.Log("Difficulty set to easy");
            break;

            case 500: 
                gameDifficultyID = 1;
                Debug.Log("Difficulty set to medium");
                break;

            case 1000:
                gameDifficultyID = 2;
                Debug.Log("Difficulty set to hard");
                break;
        }
    }
}
