using UnityEngine;
using UnityEngine.InputSystem;
//Leyton script
//makes sure this script is executed before SettingsMenu to avoid issues with values being read prior to being correctly set
[DefaultExecutionOrder(-1)]
public class ControlSchemeManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    [SerializeField] private TutorialButtons tutorialButtons;
    [SerializeField] private TutorialStateManager tutorialStateManager;

    [Header("Mobile Controls")]
    [SerializeField] private GameObject mobileControlHolder;
    [SerializeField] private GameObject[] mobileControlButtons;

    public InputActionAsset inputActions;

    private InputAction WASD;
    private InputAction ArrowKeys;
    private InputAction Touchscreen;
    private int currentControlScheme;

    private void Awake()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        inputActions.FindActionMap("Player");
        WASD = inputActions.FindAction("Player/Move (WASD)");
        ArrowKeys = inputActions.FindAction("Player/Move (ArrowKeys)");
        Touchscreen = inputActions.FindAction("Player/Move (Mobile)");
    }

    private void Start()
    {
        //if(Application.platform == RuntimePlatform.Android)
        {
            mobileControlHolder.SetActive(true);
            for(int i = 0; i < mobileControlButtons.Length; i++)
            {
                mobileControlButtons[i].SetActive(true);
            }
        }

        if(WASD == null || ArrowKeys == null)
        {
            Debug.Log("Input Action can't be found");
        }
        if (PlayerPrefs.HasKey("ControlSchemeKey"))
        {
            currentControlScheme = PlayerPrefs.GetInt("ControlSchemeKey");
            SetControlScheme();
        }
        else
        {
            currentControlScheme = 0;
            SetControlScheme();
        }
    }

    private void SetControlScheme()
    {
        playerMovement.InitialiseControlScheme();
        switch(currentControlScheme)
        {
            case 0:
                Debug.Log("WASD Controls");
                WASD.Enable();
                ArrowKeys.Disable();
                break;
            case 1:
                Debug.Log("Arrow key controls");
                WASD.Disable();
                ArrowKeys.Enable();
                break;
        }

        if (!tutorialStateManager.GetIsFirstTutorial())
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            tutorialButtons.UpdateGlyphs();
#endif
            return;
        }
        else
        {
            playerMovement.DisableActions(0);
        }
    }

    public void ChangeControlScheme()
    {
        if (currentControlScheme > 0 || currentControlScheme < 0)
        {
            currentControlScheme = 0;
        }
        else
        {
            currentControlScheme++;
        }
        PlayerPrefs.SetInt("ControlSchemeKey", currentControlScheme);
        SetControlScheme();
    }

    public int GetControlScheme()
    {
        return currentControlScheme;
    }

    public void ResetToDefault()
    {
        currentControlScheme = 0;
        if (PlayerPrefs.HasKey("ControlSchemeKey"))
        {
            PlayerPrefs.SetInt("ControlSchemeKey", 0);
        }
        SetControlScheme();
    }
}
