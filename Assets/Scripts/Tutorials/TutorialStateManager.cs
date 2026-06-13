using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//Leyton script
public class TutorialStateManager : MonoBehaviour
{
    //since almost all the code in this script will ideally only need to be executed once, I have tried to keep it as self-contained as possible and not worry too much about keeping it efficient when fully executed

    //tutorial elements to be individually accessed to explain controls to player
    [SerializeField] private GameObject WASDGlyphHolder;
    [SerializeField] private GameObject[] WASDGlyphs = new GameObject[3];
    [SerializeField] private GameObject arrowKeysGlyphHolder;
    [SerializeField] private GameObject[] arrowKeysGlyphs = new GameObject[3];
    [SerializeField] private GameObject firstTutorialObjectsHolder;
    [SerializeField] private GameObject[] firstTutorialObjects = new GameObject[4];
    [SerializeField] private GameObject swipeTutorialObjects;
    [SerializeField] private GameObject swipeUpTutorialObject;
    [SerializeField] private Button stumbleExplaination;

    //ui elements to be disabled during first tutorial
    [SerializeField] private GameObject highScoreText;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject pauseButton;

    //use this to change the segment ratio for tutorial to increase distance away obstacles spawn
    public UnityEvent setFirstTutorialSegments;

    //checks to update the current tutorial state
    public bool isFirstTutorial = false;
    private bool[] tutorialChecks = {false, false, false};
    private bool stumbleTutorialCheck = true;

    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private GameMaster gameMaster;

    //remove the below value for builds
    [Tooltip("Toggle this to access the starting tutorial without changing the high score.")]
    [SerializeField] private bool devToggleFirstTutorial = false;

    //Events to mark start/end of first in depth tutorial
    //These get hooked into for things like modifying number of collectibles needed to charge dash in and outside the tutorial
    public UnityEvent FirstTutorialStart;
    public UnityEvent FirstTutorialEnd;

    private void Awake()
    {
        //either use the playerprefs editor to reset the highscore or edit the below condition to be true to have the intial tutorial run
        if (!PlayerPrefs.HasKey("HighScore") || PlayerPrefs.GetInt("HighScore") == 0 || devToggleFirstTutorial)
        {
            isFirstTutorial = true;
            setFirstTutorialSegments.Invoke();
            playerMovement.AssignFirstTutorialEvents();
        }
    }

    public void SetFirstTutorialSequence()
    {
        if (isFirstTutorial)
        {
            FirstTutorialStart.Invoke();
            Debug.Log("Started first tutorial");
            stumbleTutorialCheck = false;
            firstTutorialObjectsHolder.SetActive(true);

            highScoreText.SetActive(false);
            scoreText.SetActive(false);
            pauseButton.SetActive(false);
        }
    }

    public IEnumerator ExplainLeftRight()
    {
        //yield return new WaitForSeconds(4.5f); //wait for the camera to fully pan around

        playerMovement.EnableActions(0);

#if !UNITY_ANDROID || UNITY_EDITOR
        switch (PlayerPrefs.GetInt("ControlSchemeKey"))
        {
            case 0:
                WASDGlyphHolder.SetActive(true);
                WASDGlyphs[0].SetActive(true);
                WASDGlyphs[1].SetActive(true);

                arrowKeysGlyphHolder.SetActive(false);
                arrowKeysGlyphs[0].SetActive(false);
                arrowKeysGlyphs[1].SetActive(false);
                Debug.Log("Showing WASD Glyphs");
                break;

            case 1:
                arrowKeysGlyphHolder.SetActive(true);
                arrowKeysGlyphs[0].SetActive(true);
                arrowKeysGlyphs[1].SetActive(true);

                WASDGlyphHolder.SetActive(false);
                WASDGlyphs[0].SetActive(false);
                WASDGlyphs[1].SetActive(false);
                Debug.Log("Showing ArrowKeys Glyphs");
                break;

            default:
                WASDGlyphHolder.SetActive(true);
                WASDGlyphs[0].SetActive(true);
                WASDGlyphs[1].SetActive(true);

                arrowKeysGlyphHolder.SetActive(false);
                arrowKeysGlyphs[0].SetActive(false);
                arrowKeysGlyphs[1].SetActive(false);
                Debug.Log("Showing WASD Glyphs by default");
                break;
        }
#endif

#if UNITY_ANDROID
        swipeTutorialObjects.SetActive(true);
#endif

        firstTutorialObjects[0].SetActive(true);
        firstTutorialObjects[1].SetActive(true);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => tutorialChecks[0]);

#if !UNITY_ANDROID || UNITY_EDITOR
        WASDGlyphHolder.SetActive(false);
        WASDGlyphs[0].SetActive(false);
        WASDGlyphs[1].SetActive(false);
        arrowKeysGlyphHolder.SetActive(false);
        arrowKeysGlyphs[0].SetActive(false);
        arrowKeysGlyphs[1].SetActive(false);
#endif
        swipeTutorialObjects.SetActive(false);
        firstTutorialObjects[0].SetActive(false);
        firstTutorialObjects[1].SetActive(false);

        Time.timeScale = 1f;

        yield return null;
    }

    public IEnumerator ExplainJump()
    {
        playerMovement.DisableActions(0);
        playerMovement.EnableActions(1);

#if !UNITY_ANDROID || UNITY_EDITOR
        switch (PlayerPrefs.GetInt("ControlSchemeKey"))
        {
            case 0:
                WASDGlyphHolder.SetActive(true);
                WASDGlyphs[2].SetActive(true);

                arrowKeysGlyphHolder.SetActive(false);
                arrowKeysGlyphs[2].SetActive(false);

                break;

            case 1:
                arrowKeysGlyphHolder.SetActive(true);
                arrowKeysGlyphs[2].SetActive(true);

                WASDGlyphHolder.SetActive(false);
                WASDGlyphs[2].SetActive(false);

                break;

            default:
                WASDGlyphHolder.SetActive(true);
                WASDGlyphs[2].SetActive(true);

                arrowKeysGlyphHolder.SetActive(false);
                arrowKeysGlyphs[2].SetActive(false);

                break;
        }
#endif

#if UNITY_ANDROID
        swipeUpTutorialObject.SetActive(true);
#endif

        firstTutorialObjects[2].SetActive(true);

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => tutorialChecks[1]);

#if !UNITY_ANDROID || UNITY_EDITOR
        WASDGlyphHolder.SetActive(false);
        WASDGlyphs[2].SetActive(false);
        arrowKeysGlyphHolder.SetActive(false);
        arrowKeysGlyphs[2].SetActive(false);
#endif
        swipeUpTutorialObject.SetActive(false);
        firstTutorialObjects[2].SetActive(false);

        Time.timeScale = 1f;
        playerMovement.EnableActions(0);

        yield return null;
    }

    public IEnumerator ExplainDash()
    {
        playerMovement.DisableActions(0);
        playerMovement.DisableActions(1);
        playerMovement.EnableActions(2);
        firstTutorialObjects[3].SetActive(true);

        Time.timeScale = 0f;

        playerMovement.ForceStumblingFalse();
#if !UNITY_ANDROID || UNITY_EDITOR
        playerMovement.AssignTutorialEvents();
#endif
        yield return new WaitForSecondsRealtime(1f);
        yield return new WaitUntil(() => tutorialChecks[2]);

        firstTutorialObjects[3].SetActive(false);

        Time.timeScale = 1f;
        playerMovement.EnableActions(0);
        playerMovement.EnableActions(1);

        yield return new WaitForSeconds(0.8f);

        highScoreText.SetActive(true);
        scoreText.SetActive(true);
        pauseButton.SetActive(true);
        pauseButton.GetComponent<Button>().interactable = true;

        playerMovement.UnassignFirstTutorialEvents();
        gameMaster.SetStateGameplay();
        isFirstTutorial = false;

        yield return null;
        //End of tutorial
        Debug.Log("End of tutorial");
        FirstTutorialEnd.Invoke();
    }

    public void AttemptStumbleTutorial()
    {
        if(!stumbleTutorialCheck)
        {
            StartCoroutine(ExplainStumble());
        }
    }

    private IEnumerator ExplainStumble()
    {
        if (stumbleTutorialCheck)
        {
            yield break;
        }
        else
        {
            stumbleExplaination.gameObject.SetActive(true);
            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(3f);
            yield return new WaitUntil(() => stumbleTutorialCheck);

            stumbleExplaination.gameObject.SetActive(false);
            Time.timeScale = 1f;

            playerMovement.OnStumble.RemoveListener(delegate{AttemptStumbleTutorial();});

            yield return null;
        }
    }

    public void ToggleTutorialX (int n)
    {
        tutorialChecks[n] = true;
    }

    public bool GetIsFirstTutorial()
    {
        return isFirstTutorial;
    }

    public void EnableStumbleTutorialCheck()
    {
        stumbleTutorialCheck = true;
    }
}
