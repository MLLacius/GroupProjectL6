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

    private SegmentData parentSegmentData;
    private GameObject player;
    private Gradient currentSectionPlayerParticles;

    private void Awake()
    {
        levelSpawner = FindFirstObjectByType<LevelSpawner>();
        parentSegmentData = GetComponentInParent<SegmentData>();
        player = GameObject.Find("Player");
    }

    private void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            parentSegmentData.UpdatePlayerRunParticles(currentSectionPlayerParticles, player.GetComponentInChildren<ParticleSystem>());
        }
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

    public void SetCurrentSectionPlayerParticles(Gradient particleColour)
    {
        currentSectionPlayerParticles = particleColour;
    }
}
