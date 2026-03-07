using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Luke script and Leyton tweaked slightly
public abstract class ObjectSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject[] prefabList;
    
    // Pool of objects
    private static Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private static bool isInitialized = false;
    protected GameObject spawnedObject;

    protected virtual void Awake()
    {
        if (!isInitialized)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            isInitialized = true;
        }
    }

    protected virtual void Start() { }
    protected virtual void Update() { }

    public virtual void SpawnObject()
    {
        if(prefabList.Length == 0) return;

        int selectedIndex = Random.Range(0, prefabList.Length);
        GameObject selectedPrefab = prefabList[selectedIndex];
        
        GameObject spawnedPrefab = GetPooledObject(selectedPrefab);
        
        // Ensure the object is positioned correctly
        //spawnedPrefab.transform.position = transform.position;
        //spawnedPrefab.transform.rotation = transform.rotation; 
        spawnedPrefab.SetActive(true);
        
        spawnedObject = spawnedPrefab;
    }

    protected GameObject GetPooledObject(GameObject searchObject)
    {
        if(!objectPool.ContainsKey(searchObject.name))
        {
            objectPool.Add(searchObject.name, new Queue<GameObject>());
        }

        // Loop through the queue until we find a valid object or run out
        while (objectPool[searchObject.name].Count > 0)
        {
            GameObject pooledObject = objectPool[searchObject.name].Dequeue();

            // Check if the object still exists (hasn't been destroyed externally)
            if (pooledObject != null)
            {
                pooledObject.transform.parent = transform; // Reparent to current spawner
                return pooledObject;
            }
            else
            {
                Debug.LogWarning("Found destroyed object in pool. Cleaning up.");
            }
        }
        
        // If queue is empty or all items were null, create new
        GameObject newObject = Instantiate(searchObject, transform);
        newObject.name = searchObject.name; 
        return newObject;
    }

    public static void ReturnObjectToPool(GameObject returnObject)
    {
        //Just in case returnObject is already destroyed
        if (returnObject == null) return;

        returnObject.SetActive(false);

        // Unparent the object so it doesn't get destroyed if the Spawner is destroyed
        returnObject.transform.parent = null; 

        if(objectPool.ContainsKey(returnObject.name))
        {
            objectPool[returnObject.name].Enqueue(returnObject);
        }
        else
        {
            //Debug.LogWarning("Attempted to return object to non-existent pool: " + returnObject.name);
            Destroy(returnObject);
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        objectPool.Clear();
    }
}