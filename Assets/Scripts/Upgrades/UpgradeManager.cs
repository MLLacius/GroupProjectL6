using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Luke script
//Saves/Loads/Manages upgrades purchased by the player

public class UpgradeManager : MonoBehaviour
{
    //References
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private AudioController audioController;
    [SerializeField] private UpgradeSciptableItem[] allUpgrades; //List of all possible upgrades in the game

    //Maps upgrade ID to current level (if 0, not owned, 1 = lv1, 2 = lv2 etc)
    private static Dictionary<string, int> upgradeLevels = new Dictionary<string, int>();
    private HashSet<string> purchasedUpgrades;
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Application.persistentDataPath + "/upgrades.json";
        LoadUpgrades();
    }   

    //Save System Wrappers
    [System.Serializable]
    public class UpgradeEntry
    {
        public string id;
        public int level;
    }

    [System.Serializable]
    private class UpgradeSaveDataWrapper
    {
        public List<UpgradeEntry> entries = new List<UpgradeEntry>();
    }

    private void LoadUpgrades()
    {
        upgradeLevels.Clear();
        if(File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            UpgradeSaveDataWrapper saveData = JsonUtility.FromJson<UpgradeSaveDataWrapper>(json);
            foreach(UpgradeEntry entry in saveData.entries)
            {
                upgradeLevels[entry.id] = entry.level;
            }
        }    
    }

    private void SaveUpgrades()
    {
        UpgradeSaveDataWrapper saveData = new UpgradeSaveDataWrapper();
        foreach(var pair in upgradeLevels)
        {
            saveData.entries.Add(new UpgradeEntry{id = pair.Key, level = pair.Value});
        }
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(saveData));
    }

    public int GetUpgradeCurrentLevel(string id)
    {
        return upgradeLevels.ContainsKey(id) ? upgradeLevels[id] : 0;
    }

    public void PurchaseUpgrade(UpgradeSciptableItem upgrade, System.Action onSuccess)
    {
        int currentLv = GetUpgradeCurrentLevel(upgrade.upgradeID);
        if(currentLv >= upgrade.levelDefinitions.Length)
        {
            audioController.PlayMaxedPurchase();
            Debug.Log("Upgrade already maxed.");
            return;
        }
        int cost = upgrade.levelDefinitions[currentLv].cost; //Cost is next levels upgrade
        if(gameMaster.TrySpendCollectibles(cost))
        {
            upgradeLevels[upgrade.upgradeID] = currentLv + 1;
            audioController.PlayValidPurchase();
            SaveUpgrades();
            Debug.Log("Purchased item:" + upgrade.upgradeID);
            onSuccess?.Invoke();
        }
        else
        {
            audioController.PlayInvalidPurchase();
        }
    }
}
