using UnityEngine;

//This script just holds some data on the segment for level gen
//There shouldn't be any functionality within this script outside of getters/setters
//Luke script
public class SegmentData : MonoBehaviour
{
    [SerializeField] private float segmentLength;

    [SerializeField] private GameObject[] hardSegments;
    [SerializeField] private GameObject[] impossibleSegments;

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
