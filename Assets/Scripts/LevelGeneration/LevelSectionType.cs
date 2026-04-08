using UnityEngine;

[CreateAssetMenu(fileName = "LevelSectionType", menuName = "Scriptable Objects/Level Section Type")]
public class LevelSectionType : ScriptableObject
{
    /*
    Area Requirements;

    - Obstacle Pool (look at changing the object spawner's list in runtime for this)
    - Types Of Obstacles
    - Base Material
    - Min/Max Length/Duration
    */

    public Material baseMaterial;
    public GameObject[] obstacles;
    [Min(15)] public int minLength;
    public int maxLength;
}
