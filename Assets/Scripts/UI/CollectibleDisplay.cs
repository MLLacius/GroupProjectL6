using UnityEngine;
using TMPro;
//Leyton and Luke script
public class CollectibleDisplay : MonoBehaviour
{
    private bool enableCollectibleDisplay;

    private TextMeshProUGUI collectibleDisplay;
    [SerializeField] private GameMaster gameMaster;

    private void Awake()
    {
        collectibleDisplay = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if(collectibleDisplay)
        {
            enableCollectibleDisplay = true;
        }
        else
        {
            enableCollectibleDisplay = false;
        }
    }

    private void Update()
    {
        if(enableCollectibleDisplay)
        {
            collectibleDisplay.text = gameMaster.GetCollectiblesGained().ToString();
        }
    }
}
