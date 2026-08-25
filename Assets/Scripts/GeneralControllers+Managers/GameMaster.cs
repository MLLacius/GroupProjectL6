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
    [Tooltip("Starting difficulty scaling from the high score will be divided by this amount (default is +500 score for each difficulty increase")]
    [SerializeField] private float gameDifficultyScalingModifier;

    private float rawScore;
    private bool gameplayStarted;
    private bool highScoreAchieved = false;
    private int currentScore = 0;
    private int highScore = 0;
    private int previousHighScore = 0;
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
        levelSpawner = GameObject.Find("Level Spawner").GetComponent<LevelSpawner>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        previousHighScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreAchieved = false;
        SetInitialDifficulty();
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
        return PlayerPrefs.GetInt("LastScore", 0);
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
        PlayerPrefs.SetInt("LastScore", lastScore);
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
        rawScore += dashDestructionBonus;
        return;
    }

    private void SetInitialDifficulty()
    {
        if(highScore < 500 / gameDifficultyScalingModifier)
        {
            gameDifficultyID = 0;
        }
        else if(highScore >= 500 / gameDifficultyScalingModifier && highScore < 1000 / gameDifficultyScalingModifier)
        {
            gameDifficultyID = 1;
        }
        else if(highScore >= 1000 / gameDifficultyScalingModifier)
        {
            gameDifficultyID = 2;
        }
    }

    private void UpdateDifficulty()
    {
        int difficultyScore = (int)(currentScore + (previousHighScore / gameDifficultyScalingModifier)); //this int is to increase the difficulty earlier depending on how far the player has previously gotten
        if (difficultyScore < 500)
        {
            gameDifficultyID = 0;
        }
        else if (difficultyScore >= 500 && difficultyScore < 1000)
        {
            gameDifficultyID = 1;
        }
        else if (difficultyScore >= 1000)
        {
            gameDifficultyID = 2;
        }
    }
}
