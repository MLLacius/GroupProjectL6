using TMPro;
using UnityEngine;

//Luke script
public class UpgradeCoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UpdateCoinsDisplay();
    }

    public void UpdateCoinsDisplay()
    {
        int currentCoins = PlayerPrefs.GetInt("Collectibles", 0);
        if(currentCoins > 999999)
        {
            coinsText.text = "Coins: 999999+";
            Debug.Log("Coins exceed display limit, showing 999999+");
            return;
        }
        coinsText.text = "Coins: " + currentCoins.ToString();
        Debug.Log("Updated coins display");
    }
}
