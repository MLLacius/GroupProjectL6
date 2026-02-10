using UnityEngine;
using UnityEditor;
using System.IO;
//Luke script

public class SaveDataTool
{
    [MenuItem("Tools/Save Data/Open Upgrades Save Data File")]
    public static void OpenUpgradesSaveDataFile()
    {
        string filePath = Application.persistentDataPath + "/upgrades.json";
        if (File.Exists(filePath))
        {
            EditorUtility.RevealInFinder(filePath);
        }
        else
        {
            Debug.LogWarning("Upgrades save data file does not exist at: " + filePath);
        }
    }
    [MenuItem("Tools/Save Data/Delete Upgrades Save Data File")]
    public static void DeleteUpgradesSaveDataFile()
    {
        string filePath = Application.persistentDataPath + "/upgrades.json";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("Deleted upgrades save data file at: " + filePath);
        }
        else
        {
            Debug.LogWarning("Upgrades save data file does not exist at: " + filePath);
        }
    }
}
