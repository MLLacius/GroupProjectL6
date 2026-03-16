using UnityEngine;
using System.Collections.Generic;
using System.Linq;
//Luke script (the original version of this script was done together with everyone week 1)
public class LevelSpawner : MonoBehaviour
{
    /*
    Spawns prefabs in a line which move under the player to create an endless runner effect
    The player only moves left and right, the level moves underneath them
    */    
    [Header("Level Settings")]
    [SerializeField] private GameObject [] levelPrefabs;
    [SerializeField] private GameObject [] tutorialLevelPrefabs;
    [SerializeField] private int defaultSegmentLength;
    private float segmentLength;
    [SerializeField] private int menuInitialSegmentCount;
    [SerializeField] private int additionalInitialSegmentCount; 
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f; // Speed the world moves towards camera
    [SerializeField] private float deleteZ = -20f; // The Z position where segments get destroyed
    [SerializeField] private float moveSpeedGainPerSec = 0.01f;
    [SerializeField] private float maxMoveSpeed = 30f;

    [Header("Level Variation")]
    [SerializeField] private Material[] segmentBaseOptions;
    protected Dictionary<int, Material> segmentBaseOptionsDict = new Dictionary<int, Material>();
    
    private GameMaster gameMaster;
    private TutorialStateManager tutorialStateManager;
    private List<GameObject> spawnedLevels = new List<GameObject>(); 
    private float spawnZ = 0f;
    private List<GameObject> activeSegments = new List<GameObject>(); 

    private bool movementStopped = false;
    private string lastSpawnedSegment;
    

    //Instead of instanting and destroying segments, we're using an object pool
    //This means we can just reactivate and move segments; only Instantiating and Destroying when necessary as these calls are spenny. 
    //The string is the prefab name with the queue of objects being all the instances of that prefab
    private Dictionary<string, Queue<GameObject>> segmentPool = new Dictionary<string, Queue<GameObject>>();

    private void Start()
    {
        gameMaster = GameObject.Find("Game Master").GetComponent<GameMaster>();
        tutorialStateManager = GameObject.Find("Game Master").GetComponent<TutorialStateManager>();

        for (int i = 0; i < segmentBaseOptions.Length; i++)
        {
            segmentBaseOptionsDict.Add(i, segmentBaseOptions[i]);
            Debug.Log(segmentBaseOptionsDict.Keys.ElementAt(i) + ", " + segmentBaseOptionsDict[i] + " added to dictionary");
        }
        //Setup the dictionary
        foreach (GameObject prefab in levelPrefabs)
        {
            segmentPool.Add(prefab.name, new Queue<GameObject>());
        }
        //Spawn the initial level segments
        for (int i = 0; i < menuInitialSegmentCount; i++)
        {
            SpawnSegment();
        }
    }

    private void Update()
    {
        UpdateMoveSpeed();
        MoveSegments();
        CheckForCleanup();
    }

    private void UpdateMoveSpeed()
    {
        if (moveSpeed < maxMoveSpeed && gameMaster.GetGameplayState())
        {
            moveSpeed += moveSpeedGainPerSec * Time.deltaTime;
            if (moveSpeed > maxMoveSpeed)
            {
                moveSpeed = maxMoveSpeed;
            }
        }
    }

    public void UpdateSegmentCount()
    {
        if (tutorialStateManager.GetIsFirstTutorial())
        {
            for (int i = 0; i < additionalInitialSegmentCount; i++)
            {
                if (i < tutorialLevelPrefabs.Length)
                {
                    SpawnTutorialSegment(i);
                }
                else if (i >= tutorialLevelPrefabs.Length)
                {
                    SpawnSegment();
                }
            }
        }
        else
        {
            //add a specified number of extra segments when play button is pressed
            for (int i = 0; i < additionalInitialSegmentCount; i++)
            {
                SpawnSegment();
            }
        }
    }

    private void MoveSegments()
    {
        if(movementStopped) { return; }
        // 1. Move the actual objects
        foreach (GameObject segment in activeSegments)
        {
            segment.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        spawnZ -= moveSpeed * Time.deltaTime;
    }

    private void CheckForCleanup()
    {
        //Check if the oldest segment (index 0) has moved past the delete threshold
        if (activeSegments.Count > 0 && activeSegments[0].transform.position.z < deleteZ)
        {
            RemoveOldestSegment();
            SpawnSegment(); //Add a new one at the end to keep the loop going
        }
    }

    private void SpawnSegment()
    {
        int selectedPrefabIndex = Random.Range(0, levelPrefabs.Length);
        GameObject selectedPrefab = levelPrefabs[selectedPrefabIndex];
        GameObject segment = GetSegmentFromPool(selectedPrefab);

        for (int i = 0; levelPrefabs.Length > i; i++)
        {
            if(lastSpawnedSegment == levelPrefabs[i].name)
            {
                levelPrefabs[i].GetComponent<SegmentData>().GetImpossibleSegments();

                break;
            }
            else
            {
                continue;
            }
        }

        segment.transform.position = new Vector3(0, 0, spawnZ);
        segment.SetActive(true);
        activeSegments.Add(segment);

        //create a reference to the last segment to check the difficulty of the next segment
        lastSpawnedSegment = segment.name;

        //Get the individual segment length (this makes it so each segment does not have to be of equal length)
        SegmentData segmentData = segment.GetComponent<SegmentData>();
        if (segmentData != null)
        {
            segmentLength = segmentData.GetSegmentLength();
            spawnZ += segmentLength;
        }
        else
        {
            Debug.LogWarning("Level spawner could not find: " + segment.name + "'s SegementData");
            spawnZ += defaultSegmentLength;
        }


        //If main game has started
        if (gameMaster.GetGameplayState())
        {
            //Iterate through each obstacle in segment and spawn in obstacle
            ObjectSpawner[] spawners = segment.GetComponentsInChildren<ObjectSpawner>();
            foreach (ObjectSpawner spawner in spawners)
            {
                spawner.SpawnObject();
            }
        } 
    }

    private void SpawnTutorialSegment(int index)
    {
        GameObject selectedPrefab = tutorialLevelPrefabs[index];
        GameObject segment = Instantiate(selectedPrefab);

        segment.transform.position = new Vector3(0, 0, spawnZ);
        segment.SetActive(true);
        activeSegments.Add(segment);

        //Get the individual segment length (this makes it so each segment does not have to be of equal length)
        SegmentData segmentData = segment.GetComponent<SegmentData>();
        if (segmentData != null)
        {
            segmentLength = segmentData.GetSegmentLength();
            spawnZ += segmentLength;
        }
        else
        {
            Debug.LogWarning("Level spawner could not find: " + segment.name + "'s SegementData");
            spawnZ += defaultSegmentLength;
        }

        //If main game has started
        if (gameMaster.GetGameplayState())
        {
            //Iterate through each obstacle in segment and spawn in obstacle
            ObjectSpawner[] spawners = segment.GetComponentsInChildren<ObjectSpawner>();
            foreach (ObjectSpawner spawner in spawners)
            {
                spawner.SpawnObject();
            }
        }
    }

    private void RemoveOldestSegment()
    {
        GameObject oldSegment = activeSegments[0];
        activeSegments.RemoveAt(0); 
        ReturnSegmentToPool(oldSegment);
    }

    private GameObject GetSegmentFromPool(GameObject prefab)
    {
        //If we have an available segment in the pool, reuse it
        if (segmentPool.ContainsKey(prefab.name) && segmentPool[prefab.name].Count > 0)
        {
            GameObject segment = segmentPool[prefab.name].Dequeue();
            segment.SetActive(true);
            return segment;
        }
        //Otherwise, instantiate a new one
        else
        {
            GameObject segment = Instantiate(prefab);
            segment.name = prefab.name; // Ensure the name matches for pooling
            return segment;
        }
    }

    private void ReturnSegmentToPool(GameObject segment)
    {
        segment.SetActive(false);
        if (segmentPool.ContainsKey(segment.name))
        {
            segmentPool[segment.name].Enqueue(segment);
        }
        else
        {
            Destroy(segment);
        }
    }

    public float GetSpeed()
    {
        return moveSpeed;
    }

    public void SetSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public void SetMenuInitialSegmentCount(int intialCount)
    {
        menuInitialSegmentCount = intialCount;
        CalculateSegmentRatio();
    }

    private void CalculateSegmentRatio()
    {
        additionalInitialSegmentCount = defaultSegmentLength - menuInitialSegmentCount;
    }

    public void StopMovement()
    {
        moveSpeed = 0f;
        movementStopped = true;
    }
}