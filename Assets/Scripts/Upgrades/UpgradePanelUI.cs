using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

//Luke script
//This class takes the data in upgrade scriptable object and automatically fills in relevant UI fields.
public class UpgradePanelUI : MonoBehaviour
{
    [Header("Mandatory References")]
    [SerializeField] private UpgradeSciptableItem upgradeItem;
    [SerializeField] private TMP_Text upgradeNameText;
    [SerializeField] private TMP_Text upgradeCostText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private GameObject greyedOutButton;
    [SerializeField] private GameObject invalidPurchaseButton;
    [Header("Optional References")]
    [SerializeField] private TMP_Text upgradeDescriptionText;
    [SerializeField] private UnityEngine.UI.Image upgradeIconImage;
    [SerializeField] private TMP_Text levelText;

    private UpgradeManager upgradeManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //Get reference to Upgrade Manager
        upgradeManager = FindFirstObjectByType<UpgradeManager>();
        
        if(upgradeIconImage && upgradeItem.upgradeIcon)
        {
            upgradeIconImage.sprite = upgradeItem.upgradeIcon;
        }
        if(upgradeDescriptionText && upgradeItem.levelDefinitions.Length > 0)
        {
            upgradeDescriptionText.text = upgradeItem.levelDefinitions[0].levelDescription;
            upgradeDescriptionText.gameObject.SetActive(true);
        }
        
        purchaseButton.onClick.AddListener(OnPurchaseClick);
        UpdateUI();
    }

    private void OnPurchaseClick()
    {
        upgradeManager.PurchaseUpgrade(upgradeItem, () => UpdateUI(), () => StartCoroutine(EnableInvalidPurchase()));
    }

    private void UpdateUI()
    {
        int currentLvl = upgradeManager.GetUpgradeCurrentLevel(upgradeItem.upgradeID);
        int maxLvl = upgradeItem.levelDefinitions.Length;

        //Update optional Level Text
        if(levelText)
        {
            levelText.text = $"{currentLvl}/{maxLvl}";  
        } 

        //Reached max level
        if(currentLvl >= maxLvl)
        {
            purchaseButton.gameObject.SetActive(false);
            greyedOutButton.SetActive(true); // Show "Maxed" visual
            upgradeCostText.text = "Max";
            upgradeNameText.text = upgradeItem.displayedUpgradeName;
            
            // Show description of the final level
            if(upgradeDescriptionText)
                upgradeDescriptionText.text = upgradeItem.levelDefinitions[maxLvl-1].levelDescription;
            return;
        }

        //Next level available
        purchaseButton.gameObject.SetActive(true);
        greyedOutButton.SetActive(false);

        //Get data for next level
        var nextLevelData = upgradeItem.levelDefinitions[currentLvl];

        upgradeNameText.text = upgradeItem.displayedUpgradeName;
        upgradeCostText.text = nextLevelData.cost.ToString();
        if(upgradeDescriptionText) upgradeDescriptionText.text = nextLevelData.levelDescription;
    }

    private IEnumerator EnableInvalidPurchase()
    {
        invalidPurchaseButton.SetActive(true);

        yield return new WaitForSecondsRealtime(2f);

        invalidPurchaseButton.SetActive(false);

        yield return null;
    }
}