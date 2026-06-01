using UnityEngine;
using System.Collections;
//Shara script
public class Magnet : MonoBehaviour
{
    public GameObject collectibleDectectorObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        collectibleDectectorObj = GameObject.FindGameObjectWithTag("Collectible Detector");
        collectibleDectectorObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(ActivateCollectible());
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    private IEnumerator ActivateCollectible()
    {
        collectibleDectectorObj.SetActive(true);
        yield return new WaitForSeconds(10f);
        collectibleDectectorObj.SetActive(false);
    }
}
