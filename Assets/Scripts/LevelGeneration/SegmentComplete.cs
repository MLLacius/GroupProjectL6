using UnityEngine;
using UnityEngine.Events;
//Luke script 
public class SegmentComplete : MonoBehaviour
{
    public UnityEvent CompletedSegment; //This unity event is invoked when the segment is completed
                                        //Level generation system will listen to this event and spawn a new segment
    private LevelSpawner levelSpawner;
    [SerializeField] private GameObject segmentObject; // Reference to the segment GameObject
    private bool isCompleted = false;

    public void OnTriggerEnter(Collider other)
    {
        if(!isCompleted)
        {
            isCompleted = true;
            if(other.CompareTag("Player"))
            {
                CompletedSegment.Invoke();
            }
        }
        else
        {
            return;
        }
    }
}
