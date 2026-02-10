using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Leyton script, tweaked by Luke to make bars smoothly fill/deplete and for tutorial values
public class PlayerDashAndDisplay : MonoBehaviour
{
    [SerializeField] private TutorialStateManager tutorialManager;
    [SerializeField] private GameObject sliderFill;
    [SerializeField] private GameObject controlGlyphWASD;
    [SerializeField] private GameObject controlGlyphArrowKeys;
    private Slider dashDisplay;

    private float collectedCoins = 0;
    [SerializeField] private Color nonFilledColor; //Red for when using dash
    [SerializeField] private Color filledColor = new Color (1f, (100f / 255f), 0f); //Orange for fully charged

    [SerializeField] private int meterStartValue = 3;
    [SerializeField] private int meterMinimum = 0;
    [SerializeField] private int meterMaximum = 15;
    [SerializeField] private float dashDuration;
    [SerializeField] private Color[] flashcolors; 
    private bool isDashPrepared;
    public bool canDash = false;

    //Bool to say if player is dashing; this determines how the bar fills/depletes
    //If depleting, bar shows time left on dash
    //If filling, bar shows how much dash is charged
    private bool isDashing = false;
    private float dashPercentageLeft;

    //Dash upgrade variables
    [SerializeField] private UpgradeSciptableItem dashDurationUpgrade; //Increases dash time
    [SerializeField] private UpgradeSciptableItem dashCostUpgrade; //Reduces number of coins used per dash
    [Tooltip("How many fewer coins the dash costs with the upgrade")]
    [SerializeField] private int reducedDashCost;
    private UpgradeManager upgradeManager;
    private bool hasDashDurationUpgrade = false;
    private bool hasDashCostUpgrade = false;

    [SerializeField] private float fillSpeed = 5f;

    [Header("Tutorial Values")]
    [Tooltip("How many coins are required to charge dash in first tutorial")]

    [SerializeField, Min(1)] private int tutorialDashCost; //Must be greater than or equal to 1
    [SerializeField, Min(0)] private int tutorialDashStartValue; //Must be greater than or equal to 0


    public void EnableTutorialDashCost()
    {
        dashDisplay.minValue = meterMinimum;
        dashDisplay.maxValue = tutorialDashCost;
        dashDisplay.value = tutorialDashStartValue;
        Debug.Log("Tutorial Values");
        Debug.Log("Dash meter max value set to: " + tutorialDashCost);
        Debug.Log("Dash duration set to: " + dashDuration);
        
    }

    public void EnableDefaultDashValues()
    {
        //Check if we own upgrades and set values accordingly
        upgradeManager = GameObject.Find("Upgrades Manager").GetComponent<UpgradeManager>();
        int durationLevel = upgradeManager.GetUpgradeCurrentLevel(dashDurationUpgrade.upgradeID);
        if (durationLevel > 0) //If level is greater than 0, we own the upgrade
        {
            hasDashDurationUpgrade = true;
            float extendDurationAmount = dashDurationUpgrade.GetValueForLevel(durationLevel);
            dashDuration += extendDurationAmount;
        }
        if (upgradeManager.GetUpgradeCurrentLevel(dashCostUpgrade.upgradeID) > 0) //If level is greater than 0, we own the upgrade
        {
            hasDashCostUpgrade = true;
            meterMaximum -= reducedDashCost;
        }

        //nonFilledColor = sliderFill.GetComponent<Image>().color;
        dashDisplay.minValue = meterMinimum;
        dashDisplay.maxValue = meterMaximum;
        dashDisplay.value = meterStartValue;
        Debug.Log("Default Values");
        Debug.Log("Dash meter max value set to: " + meterMaximum);
        Debug.Log("Dash duration set to: " + dashDuration);
    }

    private void Awake()
    {
        dashDisplay = GetComponent<Slider>();
    }

    private void Start()
    {
        isDashPrepared = false;
        if(!tutorialManager.isFirstTutorial)
        {
            EnableDefaultDashValues();
        }
        return;
    }

    private void Update()
    {
        //Show dash charge amount on display
        if (dashDisplay && !isDashing)
        {
            dashDisplay.value = Mathf.Lerp(dashDisplay.value, collectedCoins, fillSpeed * Time.deltaTime);
        }
        //Show dash time left on display
        else if (dashDisplay && isDashing)
        {
            dashDisplay.value = dashPercentageLeft;
        }
        else
        {
            Debug.Log("Can't find display object");
        }

        if ((collectedCoins >= meterMaximum || (collectedCoins >= tutorialDashCost && tutorialManager.isFirstTutorial)) && !isDashPrepared)
        {
            sliderFill.GetComponent<Image>().color = filledColor;
            canDash = true;
            ShowDashGlyph();
            StartCoroutine(TweenColorOnFullBar());
            isDashPrepared = true;
        }
        //Failsafe to reset color 
        else if (!isDashPrepared)
        {
            sliderFill.GetComponent<Image>().color = nonFilledColor;
        }
    }

    public void IncrementCollectedCoins()
    {
        if (collectedCoins < meterMaximum)
        {
            collectedCoins++;
        }
    }

    public void OnPlayerDash()
    {
        Debug.Log("Player dashed, starting dash depletion");
        StartCoroutine(DecreaseCoinCount());
        canDash = false;
        controlGlyphWASD.SetActive(false);
        controlGlyphArrowKeys.SetActive(false);
        sliderFill.GetComponent<Image>().color = nonFilledColor;
        isDashing = true;
        isDashPrepared = false;
    }

    private IEnumerator DecreaseCoinCount()
    {
        float timePassed = 0;
        
        //Assume meter full (meterMaximum) at start of dash
        float startValue = meterMaximum; 

        while (timePassed < dashDuration)
        {
            timePassed += Time.deltaTime;
            
            // Calculate the percentage complete 
            float t = timePassed / dashDuration;
            float currentValue = Mathf.Lerp(startValue, 0, t);
            
            dashPercentageLeft = currentValue;
            collectedCoins = currentValue; 

            yield return null;
        }

        //Ensure values are 0 at end
        isDashing = false;
        Debug.Log("Dash ended, resetting coin count, time: " + timePassed);
        collectedCoins = 0;
        dashDisplay.value = 0;
    }

    private void ShowDashGlyph()
    {
        switch(PlayerPrefs.GetInt("ControlSchemeKey"))
        {
            case 0:
                controlGlyphWASD.SetActive(true);
                controlGlyphArrowKeys.SetActive(false);
                break;

            case 1:
                controlGlyphArrowKeys.SetActive(true);
                controlGlyphWASD.SetActive(false);
                break;

            default:
                controlGlyphWASD.SetActive(true);
                controlGlyphArrowKeys.SetActive(false);
                break;
        }
    }

    private IEnumerator TweenColorOnFullBar()
    {
        while (true)
        {
            foreach (Color i in flashcolors)
            {
                sliderFill.GetComponent<Image>().color = i;
                yield return new WaitForSeconds(0.4f);
                if(isDashing)
                {
                    yield break;
                }
            }
        }
    }

    public float GetDashDuration()
    {
        return dashDuration;
    }
}
