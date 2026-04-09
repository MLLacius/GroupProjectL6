using UnityEngine;

//This script just holds some data on the segment for level gen
//There shouldn't be any functionality within this script outside of getters/setters (other than updating for section types)
//Luke script
public class SegmentData : MonoBehaviour
{
    [SerializeField] private float segmentLength;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private GameObject segmentFloor;
    [SerializeField] private GameObject[] objectSpawners;

    [SerializeField] private GameObject[] hardSegments;
    [SerializeField] private GameObject[] impossibleSegments;

    private LevelSpawner levelSpawner;

    //Hard and impossible segments combos notes for reference
    /*
    Hard -
    Segment 1 - Segment 4
    Segment 1 - Segment 8
    Segment 8 - Segment 4
    Segment 5 - Segment 6
    Segment 1 - Segment 6
    Segment 6 - Segment 6
    Segment 6 - Segment 5
    Segment 3 - Segment 5
    Segment 3 - Segment 8 (Right Lane)
    Segment 2 - Segment 5
    Segment 4 - Segment 6

    Impossible -
    Segment 7 - Segment 6
    Segment 3 - Segment 6 (Right Lane)
    Segment 2 - Segment 6
    */

    private void Awake()
    {
        levelSpawner = FindFirstObjectByType<LevelSpawner>();
        segmentFloor.GetComponent<MeshRenderer>().material = baseMaterial;
    }

    private void OnEnable()
    {
        UpdateSectionType();
    }

    public void UpdateSectionType()
    {
        //update segment for current section type
        segmentFloor.GetComponent<MeshRenderer>().material = levelSpawner.GetSectionData().baseMaterial;
        for (int i = 0; i < objectSpawners.Length; i++)
        {
            objectSpawners[i].GetComponent<ObstacleSpawner>().UpdatePrefabList();
        }
    }

    public float GetSegmentLength()
    {
        return segmentLength; 
    }

    public GameObject[] GetHardSegments()
    {
        return hardSegments;
    }

    public GameObject[] GetImpossibleSegments()
    {
        return impossibleSegments;
    }
}
