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

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    [Tooltip("Material used for segment floor")]
    public Material baseMaterial;

    [Tooltip("Pool of obstacles that obstacle spawners on sgement can use")]
    public GameObject[] obstacles;

    [Tooltip("Segments that can appear during this segment type")]
    public GameObject[] segments;

    [Tooltip("Minimum amount of segments section can last for")]
    [Min(15)] public int minSegmentCount;

    [Tooltip("Maximum amount of segemnts section can last for")]
    public int maxSegmentCount;

    [Tooltip("Section will start appearing from this difficulty onwards")]
    public Difficulty startingDifficulty;

    [Tooltip("Colour of the particle smoke following the sheep during the section")]
    public Gradient sheepParticles;
}
