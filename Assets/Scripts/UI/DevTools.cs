using UnityEngine;
using System.IO;

public class DevTools : MonoBehaviour
{

    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs deleted");
    }

    public void DeleteJSONData()
    {
        string filePath = Application.persistentDataPath + "/upgrades.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted upgrades save data file at: " + filePath);
        }
    }
}
