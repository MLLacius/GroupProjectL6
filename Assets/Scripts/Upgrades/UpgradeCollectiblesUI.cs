using TMPro;
using UnityEngine;

//Luke script
public class UpgradeCollectiblesUI : MonoBehaviour
{
    [SerializeField] private TMP_Text collectiblesText;
    //Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        UpdateCollectiblesDisplay();
    }

    public void UpdateCollectiblesDisplay()
    {
        int currentCollectibles = PlayerPrefs.GetInt("Collectibles", 0);
        if(currentCollectibles > 999999)
        {
            collectiblesText.text = "999999+";
            return;
        }
        collectiblesText.text = currentCollectibles.ToString();
    }
}
