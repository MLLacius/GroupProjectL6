using UnityEngine;

public class LevelSectionManager : LevelSpawner
{
    /*
    Area Requirements;

    - Obstacle Pool (look at changing the object spawner's list in runtime for this)
    - Types Of Obstacles
    - Base Material
    - Min/Max Length/Duration
    */
    private GameObject[] prefabGroup1;
    private GameObject[] prefabGroup2;
    private GameObject[] prefabGroup3;

    private void Awake()
    {
        prefabGroup1 = Resources.LoadAll<GameObject>("Area1Obstacles");

        foreach (GameObject prefab in prefabGroup1)
        {
            Debug.Log(prefab.name);
        }
    }
}
