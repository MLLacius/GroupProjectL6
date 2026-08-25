using UnityEngine;

//Please see ObjectSpawner.cs for base class details
//Luke script 
public class ObstacleSpawner : ObjectSpawner
{
    private bool hasAttemptedSpawn = false;
    private GameMaster gameMaster;
    [SerializeField] private Transform spawnTransform;

    protected override void Start()
    {
        base.Start(); //Calls base start in ObjectSpawner.cs
    }

    private void OnEnable()
    {
        hasAttemptedSpawn = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Awake()
    {
        base.Awake(); //Calls base awake in ObjectSpawner.cs
        gameMaster = GameObject.Find("Game Master").GetComponent<GameMaster>();
    }

    public override void SpawnObject()
    {
        base.SpawnObject();
        spawnedObject.transform.position = spawnTransform.position;
        //Do a check to see if this object has any children (i.e. spawned obstacle)
        if(transform.childCount == 0)
        {
            hasAttemptedSpawn = false;
        }
        else { hasAttemptedSpawn = true; }
    }
}
