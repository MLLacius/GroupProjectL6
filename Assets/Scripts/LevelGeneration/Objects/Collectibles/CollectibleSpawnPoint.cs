using UnityEngine;
//Luke script
public class CollectibleSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private GameObject activeCollectible;
    private Vector3 initialLocalPosition;

    [Header("References")]
    private UpgradeManager upgradeManager;
    private GameMaster gameMaster;

    [Header("Powerups")]
    [SerializeField] private UpgradeSciptableItem magnetUpgrade;
    [SerializeField] private GameObject magnetPrefab;

    private bool spawnMagnets = false;
    private float magnetSpawnRate;

    private void Awake()
    {
        //Capture Position
        if (activeCollectible != null)
        {
            initialLocalPosition = activeCollectible.transform.localPosition;
        }
        else
        {
            initialLocalPosition = Vector3.zero;
        }

        //Find Managers
        GameObject upgradeManager = GameObject.Find("Upgrades Manager");
        if (upgradeManager) this.upgradeManager = upgradeManager.GetComponent<UpgradeManager>();
        
        GameObject gameMaster = GameObject.Find("Game Master");
        if (gameMaster) this.gameMaster = gameMaster.GetComponent<GameMaster>();

        int currentMagnetSpawnLevel = this.upgradeManager.GetUpgradeCurrentLevel(magnetUpgrade.upgradeID);
        if (this.upgradeManager && currentMagnetSpawnLevel > 0) //If level is greater than 0, upgrade is owned
        {
            spawnMagnets = true;
            magnetSpawnRate = magnetUpgrade.GetValueForLevel(currentMagnetSpawnLevel);
        }
    }

    private void OnEnable()
    {
        if (gameMaster)
        {
            //Subscribe to the "Start Button" event
            gameMaster.OnGameStart.AddListener(SpawnCollectible);
            
            //Only spawn immediately if the game is running.
            if (gameMaster.GetGameplayState())
            {
                SpawnCollectible();
            }
        }
    }

    private void Start()
    {
        //Hide existing collectible immediately on load
        if (activeCollectible != null && gameMaster != null && !gameMaster.GetGameplayState())
        {
            activeCollectible.SetActive(false);
            Destroy(this);
        }
    }

    private void SpawnCollectible()
    {
        //Handle the hidden collectibles from start
        if (activeCollectible != null)
        {
            // We only turn it on. We assume if it's there, it's the one we hid.
            activeCollectible.SetActive(true);
            return;
        }

        //Spawning Logic in case no collectible exists at the moment
        if (spawnMagnets && Random.value < magnetSpawnRate && magnetPrefab != null)
        {
            activeCollectible = Instantiate(magnetPrefab, transform);
            activeCollectible.transform.localPosition = initialLocalPosition;
            return;
        }

        if (collectiblePrefab != null)
        {
            activeCollectible = Instantiate(collectiblePrefab, transform);
            activeCollectible.transform.localPosition = initialLocalPosition;
        }
    }

    private void OnDisable()
    {
        if (gameMaster)
        {
            gameMaster.OnGameStart.RemoveListener(SpawnCollectible);
        }
    }
}