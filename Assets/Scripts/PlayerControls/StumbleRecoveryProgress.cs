using UnityEngine;
using UnityEngine.UI;
//Luke script

public class StumbleRecoveryProgress : MonoBehaviour
{
    [SerializeField] private Slider stumbleRecoverySlider;
    [SerializeField] private CanvasGroup dashChargeFillArea;
    private PlayerMovement playerMovement;
    private float totalRecoveryTime;
    private float currentRecoveryTime;
    [SerializeField, Range(0, 1)] private float startingDashAlpha;

    private void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();    
    }

    public void OnRecoveryStart()
    {
        //Enable stumble recovery slider 
        stumbleRecoverySlider.gameObject.SetActive(true);
        totalRecoveryTime = playerMovement.GetTotalRecoveryTime();
        currentRecoveryTime = playerMovement.GetCurrentStumblingTime();
        stumbleRecoverySlider.value = currentRecoveryTime / totalRecoveryTime;
        dashChargeFillArea.alpha = startingDashAlpha;
    }

    public void OnRecoveryTick()
    {
        currentRecoveryTime = playerMovement.GetCurrentStumblingTime();
        float progressAmount = currentRecoveryTime / totalRecoveryTime;
        stumbleRecoverySlider.value = progressAmount;
        //Makes the dash charge alpha increase from startingAlpha to 1 over time, so when it's back at 1 the recovery is over
        dashChargeFillArea.alpha =  Mathf.Lerp(1f, startingDashAlpha, progressAmount);
    }

    public void OnRecoveryEnd()
    {
        //Hide recovery slider when we've recovered
        stumbleRecoverySlider.value = 1;
        dashChargeFillArea.alpha = 1;
        stumbleRecoverySlider.gameObject.SetActive(false);
    }
}
