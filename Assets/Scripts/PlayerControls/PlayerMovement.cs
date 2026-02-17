using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//Luke script, Leyton added tutorial and dash logic, Shara did screen shake
//The original version of this script was done by me and everyone tweaked it
public class PlayerMovement : MonoBehaviour
{
    enum Lanes
    {
        Left, //0
        Center, //1
        Right //2
    }

    //Unity Events: Please use these to handle other logic when these events occur.
    //Please don't hook into Update or other functions in this script for these events.
    //Oh and remember to assign functions in inspector/through script :)
    [Header("Events")]
    public UnityEvent OnStumble; //Called when the player hits into a wall but does not die
    public UnityEvent OnRecover; //Called when the player has recovered from a stumble and is back to normal
    public UnityEvent OnLaneChange; //Called when the player successfully changes lanes
    public UnityEvent OnJump; //Called when the player is mid air
    public UnityEvent OnGameOver; //Called when the player dies. RIP.
    public UnityEvent OnDash; //Called when the player dashes forward
    public UnityEvent OnDashFinish; //Called when the player's dash finishes
    public UnityEvent OnEscapePressed;

    [Header("Mobile Platform Events")]
    public UnityEvent OnDashBarPressed;
    public UnityEvent OnJumpPressed;
    public UnityEvent OnLeftPressed;
    public UnityEvent OnRightPressed;

    //Inputs
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private InputAction pauseAction;
    
    [Header("References")]
    [SerializeField] private PlayerDashAndDisplay dashAndDisplay;
    [SerializeField] private TutorialStateManager tutorialStateManager;
    [SerializeField] private TutorialButtons tutorialButtons;
    [SerializeField] private StumbleRecoveryProgress stumbleRecoveryProgress;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulse; //Reference to the camera shake script to trigger shakes on stumble
    private GameMaster gameMaster;
    private LevelSpawner levelSpawner;
    private Rigidbody playerRigidbody;

    [Header("Mobile Control References")]
    [SerializeField] private Button dashButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    [Header("Movement Speed and Input Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float laneWidth = 2f;
    [SerializeField] private float nextInputDelay = 3f; //Time delay between lane switch inputs
    [SerializeField] private float jumpInputDelay = 1.0f;
    [SerializeField] private float laneChangeSpeed = 5f;
    [SerializeField] private bool isPlayerDashing = false;
    [SerializeField] private float dashEndInvincibilityTime = 0.5f; //How long the player is invincible after dash ends
    [SerializeField] private bool canDashWhileStumbling = false;
    [SerializeField] private bool enableTouchControls;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheckTransform;
    [SerializeField] private float groundCheckRayCastDistance;
    [SerializeField] private LayerMask groundLayers;

    //States
    private Lanes currentLane = Lanes.Center;
    private float currentJumpDelay;
    private float currentStumbleInvincibilityTime;
    private float currentStumbleTimer;
    private float inputDelayTimer = 0f;
    private RigidbodyConstraints lockedX = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    private RigidbodyConstraints unlockedX = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

    private bool isStumbling = false;
    private bool isJumping = false;
    private bool isGameOver = false;

    //Stumble Values
    [Header("Stumble Settings")]
    [SerializeField] private float stumbleInvincibilityTime;
    [SerializeField] private float stumbleRecoverTime;
    [SerializeField] private Transform stumbleCheckOrigin;
    [SerializeField] private float stumbleRayDefaultDistance;

    [Tooltip("How much the stumble check ray's distance is multiplied by world speed e.g. 1 is multiply world speed x default distance and 2 is 2 x world speed x default distance")]
    [SerializeField] private float worldSpeedRayDistanceMultiplier = 1f;
    [SerializeField] LayerMask obstacleLayers;
    [SerializeField] private bool invincibilityTesting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameMaster = GameObject.Find("Game Master").GetComponent<GameMaster>();
        levelSpawner = GameObject.Find("Level Spawner").GetComponent<LevelSpawner>();
        playerRigidbody = GetComponent<Rigidbody>();
        InitialiseControlScheme();
        pauseAction = InputSystem.actions.FindAction("Pause");


        if(tutorialStateManager.GetIsFirstTutorial())
        {
            moveAction.Disable();
            jumpAction.Disable();
            dashAction.Disable();
        }

        //Check stumble timings are valid
        if(stumbleRecoverTime <= stumbleInvincibilityTime)
        {
            Debug.LogError("Player cannot die as stumble recover time and stumble invincibility time are the same in PlayerMovement");
        }

        playerRigidbody.constraints = lockedX;

        //if(Application.platform == RuntimePlatform.Android || EditorApplication.isRemoteConnected)
        {
            dashButton.onClick.AddListener(MobileDash);
            leftButton.onClick.AddListener(MobileLaneSwitchLeft);
            rightButton.onClick.AddListener(MobileLaneSwitchRight);
            jumpButton.onClick.AddListener(MobileJump);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameMaster != null && !gameMaster.GetGameplayState()) { return; }
        if (isGameOver) { return; }

        HandleTimers();
        HandleInputs();
    }
    
    private void HandleTimers()
    {
        //Lane switch and jump delay timers
        if(inputDelayTimer > 0f) inputDelayTimer -= Time.deltaTime;
        if(currentJumpDelay >0f) currentJumpDelay -= Time.deltaTime;
        //Stumbling logic
        if(currentStumbleInvincibilityTime > 0f)
        {
            currentStumbleInvincibilityTime -= Time.deltaTime;
        } 
        if(isStumbling)
        {
            currentStumbleTimer -= Time.deltaTime;
            if(currentStumbleTimer <= 0f)
            {
                RecoverFromStumble();
            }
            stumbleRecoveryProgress.OnRecoveryTick();
        }
    }
    private void HandleInputs()
    {
#if !UNITY_ANDROID || UNITY_EDITOR
        //Lane switch
        if (inputDelayTimer <= 0f)
        {
            float moveInput = moveAction.ReadValue<float>();
            if(moveInput < -0.1f) { TrySwitchLane(Lanes.Left, Lanes.Right);} //Left input
            else if(moveInput > 0.1f) { TrySwitchLane(Lanes.Right, Lanes.Left);} //Right input
        } 
        //Jump
        if(jumpAction.WasPressedThisFrame() && currentJumpDelay <= 0f && GroundCheck())
        {
           PerformJump();
        }
        //Reset jump state if grounded
        if(GroundCheck() && isJumping && playerRigidbody.linearVelocity.y <= -0.1f) { isJumping = false; }
        if(!GroundCheck() && isJumping)
        {
            AlterJumpVelocity();
        }
        //Dash
        if(dashAction.WasPressedThisFrame() && dashAndDisplay.canDash && !isPlayerDashing)
        {
            //If we cant dash while stumbling and are stumbling, return
            if(!canDashWhileStumbling && isStumbling) 
            {
                Debug.Log("Can't dash while stumbling");
                return;
            } 
            OnPlayerDash();
        }
        if(pauseAction.WasPressedThisFrame())
        {
            OnEscapePressed.Invoke();
        }
#endif
        //Reset jump state if grounded
        if (GroundCheck() && isJumping && playerRigidbody.linearVelocity.y <= -0.1f) { isJumping = false; }
        if (!GroundCheck() && isJumping)
        {
            AlterJumpVelocity();
        }

        if (pauseAction.WasPressedThisFrame())
        {
            OnEscapePressed.Invoke();
        }
    }

    private void MobileDash()
    {
        //Dash
        if (dashAndDisplay.canDash && !isPlayerDashing)
        {
            //If we cant dash while stumbling and are stumbling, return
            if (!canDashWhileStumbling && isStumbling)
            {
                Debug.Log("Can't dash while stumbling");
                return;
            }
            OnDashBarPressed.Invoke();
            OnPlayerDash();
        }
    }

    private void MobileLaneSwitchLeft()
    {
        //Lane switch
        if (inputDelayTimer <= 0f)
        {
            OnLeftPressed.Invoke();
            TrySwitchLane(Lanes.Left, Lanes.Right); //Left input
        }
    }

    private void MobileLaneSwitchRight()
    {
        //Lane switch
        if (inputDelayTimer <= 0f)
        {
            OnRightPressed.Invoke();
            TrySwitchLane(Lanes.Right, Lanes.Left); //Right input
        }
    }

    private void MobileJump()
    {
        //Jump
        if (currentJumpDelay <= 0f && GroundCheck())
        {
            OnJumpPressed.Invoke();
            PerformJump();
        }
    }

    private void TrySwitchLane(Lanes targetLane, Lanes oppositeLane)
    {
        Lanes finalTargetLane = currentLane;
        if(targetLane == Lanes.Left)
        {
            if(currentLane == Lanes.Center) { finalTargetLane = Lanes.Left;}
            else if(currentLane == Lanes.Right) { finalTargetLane = Lanes.Center;}
            else{ AttemptStumble(); return;}
        }
        else
        {
            if(currentLane == Lanes.Center) { finalTargetLane = Lanes.Right;}
            else if(currentLane == Lanes.Left) { finalTargetLane = Lanes.Center;}
            else { AttemptStumble(); return;}
        }
        //Calculate target X position based on final target lane
        float targetX = 0f;
        switch(finalTargetLane)
        {
            case Lanes.Left:
                targetX = -laneWidth;
                break;
            case Lanes.Center:
                targetX = 0f;
                break;
            case Lanes.Right:
                targetX = laneWidth;
                break;
        }
        if(CheckForCloseCallObstacles())
        {
            Debug.Log("Close call lane switch");
            AttemptStumble();
        }
        playerRigidbody.constraints = unlockedX;
        StartCoroutine(SmoothLaneSwitch(targetX));
        currentLane = finalTargetLane;
        inputDelayTimer = nextInputDelay;
        OnLaneChange.Invoke();
    }

    private IEnumerator SmoothLaneSwitch(float targetX)
    {
        float startX = playerRigidbody.position.x;
        float t = 0f;
        while(t < 1f)
        {
            t += Time.deltaTime * laneChangeSpeed;
            float newX = Mathf.Lerp(startX, targetX, t);
            Vector3 newPosition = new Vector3(newX, playerRigidbody.position.y, playerRigidbody.position.z);
            playerRigidbody.MovePosition(newPosition);
            yield return null;
        }
        Vector3 finalPosition = new Vector3(targetX, playerRigidbody.position.y, playerRigidbody.position.z);
        playerRigidbody.MovePosition(finalPosition);
        yield return new WaitForFixedUpdate(); //Wait for physics update until we lock player X position again
        playerRigidbody.constraints = lockedX;
    }

    private bool CheckForCloseCallObstacles()
    {
        if(levelSpawner == null) { return false; }
        float currentSpeed = levelSpawner.GetSpeed();
        float rayDistance = worldSpeedRayDistanceMultiplier * (currentSpeed / 10) * stumbleRayDefaultDistance;
        RaycastHit hit;
        if(Physics.Raycast(stumbleCheckOrigin.position, stumbleCheckOrigin.forward, out hit, rayDistance, obstacleLayers))
        {
            Debug.DrawLine(stumbleCheckOrigin.position, hit.point, Color.red, 2);
            return true;
        }
        Debug.DrawLine(stumbleCheckOrigin.position, stumbleCheckOrigin.position + stumbleCheckOrigin.forward * rayDistance, Color.green, 2);
        return false;
    }  

    private void PerformJump()
    {
        Vector3 velocity = playerRigidbody.linearVelocity;
        velocity.y = 0;
        playerRigidbody.linearVelocity = velocity;
        playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        currentJumpDelay = jumpInputDelay;
        OnJump.Invoke();
    }

    private void AlterJumpVelocity()
    {
        if(playerRigidbody.linearVelocity.y < 0)
        {
            Vector3 gravityForce = Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            playerRigidbody.linearVelocity += gravityForce;
        }
    }

    private bool GroundCheck()
    {
        return Physics.Raycast(groundCheckTransform.position, Vector3.down, groundCheckRayCastDistance, groundLayers);
    }

    public void AttemptStumble()
    {
        //Don't stumble if invincible, is game over or dashing
        if(invincibilityTesting || isGameOver || isPlayerDashing)
        {
            Debug.Log("Stumble failed");
             return;      
        }
        //In I frames, ignore hit
        if(currentStumbleInvincibilityTime > 0 )
        {
            Debug.Log("Ignored stumble due to invincibility frames");
            return;
        }
        //Death
        if(isStumbling)
        {
            Debug.Log("Player hit while stumbling, triggering game over");
            TriggerGameOver();
        }
        //First stumble
        else
        {
            Debug.Log("Player hit, starting stumble");
            StartStumble();
        }
    }

    private void StartStumble()
    {
        isStumbling = true;
        TriggerShake();
        currentStumbleInvincibilityTime = stumbleInvincibilityTime;
        currentStumbleTimer = stumbleRecoverTime;
        stumbleRecoveryProgress.OnRecoveryStart();
        OnStumble.Invoke();
    }

    private void RecoverFromStumble()
    {
        if(isGameOver) { return; }
        isStumbling = false;
        Debug.Log("Recovered from stumble");
        stumbleRecoveryProgress.OnRecoveryEnd();
        OnRecover.Invoke();
    }

    public void TriggerGameOver()
    {
        TriggerShake();
        isGameOver = true;
        Debug.Log("Game Over");
        OnGameOver.Invoke();
        playerRigidbody.isKinematic = true; //Stop all player movement
    }

    public float GetTotalRecoveryTime()
    {
        return stumbleRecoverTime;
    }

    public float GetCurrentStumblingTime()
    {
        return currentStumbleTimer;
    }

    //Have the camera shake up when stumbling
    private void TriggerShake()
    {
        Debug.Log("Camera Shake Triggered");
        cinemachineImpulse.GenerateImpulse();
    }

    public void InitialiseControlScheme()
    {
        if (!(Application.platform == RuntimePlatform.Android) && !enableTouchControls)
        {
            jumpAction = InputSystem.actions.FindAction("Jump");

            if (PlayerPrefs.HasKey("ControlSchemeKey"))
            {
                //change the referenced input actions based on current control scheme pulled from player prefs
                switch (PlayerPrefs.GetInt("ControlSchemeKey"))
                {
                    case 0:
                        moveAction = InputSystem.actions.FindAction("Move (WASD)");
                        dashAction = InputSystem.actions.FindAction("Dash (WASD)");
                        break;
                    case 1:
                        moveAction = InputSystem.actions.FindAction("Move (ArrowKeys)");
                        dashAction = InputSystem.actions.FindAction("Dash (ArrowKeys)");
                        break;
                    default:
                        moveAction = InputSystem.actions.FindAction("Move (WASD)");
                        dashAction = InputSystem.actions.FindAction("Dash (WASD)");
                        break;
                }
            }
            else
            {
                moveAction = InputSystem.actions.FindAction("Move (WASD)");
                dashAction = InputSystem.actions.FindAction("Dash (WASD)");
            }
        }
        else
        {
            moveAction = InputSystem.actions.FindAction("Move (Mobile)");
            dashAction = InputSystem.actions.FindAction("Dash (WASD)");
            jumpAction = InputSystem.actions.FindAction("Jump (Mobile)");
        }

        dashAction.Enable(); //this prevents a very specific bug where dashing will not work during runs following the first tutorial (if the game is not closed/reset) for the control scheme that was not used in said tutorial if the control scheme was changed before the first tutorial

        if (tutorialStateManager.GetIsFirstTutorial())
        {
            moveAction.Disable();
            dashAction.Disable();
        }
    }

    public void OnPlayerDash()
    {
        isPlayerDashing = true; //toggle to let player destroy obstacles instead of dying to them
        dashAndDisplay.OnPlayerDash(); //reset dash meter
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        OnDash.Invoke();
        float startingDashSpeed = levelSpawner.GetSpeed();
        float maxDashSpeed = levelSpawner.GetSpeed() * 3;
        bool hasDashFinished = false;

        //increase player's speed
        for (float increasingSpeed = levelSpawner.GetSpeed(); levelSpawner.GetSpeed() < maxDashSpeed; increasingSpeed += (startingDashSpeed / 10))
        {
            levelSpawner.SetSpeed(increasingSpeed);
            yield return new WaitForSeconds(0.02f);
        }

        //duration player will stay at max speed of dash before decreasing
        float dashTime = dashAndDisplay.GetDashDuration();
        yield return new WaitForSeconds(dashTime);

        //decrease player's speed
        for (float decreasingSpeed = levelSpawner.GetSpeed(); levelSpawner.GetSpeed() > startingDashSpeed; decreasingSpeed -= ((startingDashSpeed * 3) / 20))
        {
            levelSpawner.SetSpeed(decreasingSpeed);
            yield return new WaitForSeconds(0.08f);

            if(levelSpawner.GetSpeed() <= startingDashSpeed)
            {
                hasDashFinished = true;
                levelSpawner.SetSpeed(startingDashSpeed);
                break;
            }
        }
        
        if (hasDashFinished)
        {
            isPlayerDashing = false;
            OnDashFinish.Invoke();
            //Adds dash end invincibility frames
            currentStumbleInvincibilityTime = dashEndInvincibilityTime;
            yield return null;
        }
    }

    public bool GetIsPlayerDashing()
    {
        return isPlayerDashing;
    }

#if !UNITY_ANDROID || UNITY_EDITOR
    //assign event to fade out glyphs only once first tutorial has been completed
    public void AssignTutorialEvents()
    {
        OnLaneChange.AddListener(delegate {tutorialButtons.StartFadeOut();});
        OnJump.AddListener(delegate {tutorialButtons.StartFadeOut();});
        OnDash.AddListener(delegate () { tutorialButtons.StartFadeOut();});
    }
#endif

    //add relevant bool checks to progress through first tutorial
    public void AssignFirstTutorialEvents()
    {
        OnLaneChange.AddListener(delegate {tutorialStateManager.ToggleTutorialX(0);});
        OnJump.AddListener(delegate {tutorialStateManager.ToggleTutorialX(1);});
        OnDash.AddListener(delegate { tutorialStateManager.ToggleTutorialX(2);});
    }

    //remove the above bool checks after first tutorial is finished since they aren't needed anymore
    public void UnassignFirstTutorialEvents()
    {
        OnLaneChange.RemoveListener(delegate { tutorialStateManager.ToggleTutorialX(0);});
        OnJump.RemoveListener(delegate { tutorialStateManager.ToggleTutorialX(1);});
        OnDash.RemoveListener(delegate { tutorialStateManager.ToggleTutorialX(2);});
    }

    //methods to remotely enable and disable player's actions if needed (currently used to help with interactions between settings menu and first tutorial and disable movement during the panning cutscene)
    public void EnableActions(int i)
    {
        InputAction[] actions = {moveAction, jumpAction, dashAction};
        actions[i].Enable();
    }

    public void DisableActions(int i)
    {
        InputAction[] actions = {moveAction, jumpAction, dashAction};
        actions[i].Disable();
    }

    public void ForceStumblingFalse() //this method is used only in the first tutorial to make sure the player can dash at the dash segment and doesn't get softlocked
    {
        isStumbling = false;
    }
}
