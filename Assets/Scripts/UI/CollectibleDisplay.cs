using UnityEngine;
using TMPro;
//Leyton and Luke script
public class CollectibleDisplay : MonoBehaviour
{
    private bool enableCoinDisplay;

    [SerializeField] private TextMeshProUGUI coinDisplay;
    [SerializeField] private GameMaster gameMaster;

    private void Start()
    {
        if(coinDisplay)
        {
            enableCoinDisplay = true;
        }
        else
        {
            enableCoinDisplay = false;
            Debug.Log("Coin display not found");
        }
    }

    private void Update()
    {
        if(enableCoinDisplay)
        {
            coinDisplay.text = "Coins: " + gameMaster.GetCollectiblesGained();
        }
    }
}
