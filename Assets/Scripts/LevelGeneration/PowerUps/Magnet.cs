using UnityEngine;
using System.Collections;
//Shara script
public class Magnet : MonoBehaviour
{
    public GameObject coinDectectorObj;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        coinDectectorObj = GameObject.FindGameObjectWithTag("CoinDetector");
        coinDectectorObj.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(ActivateCoin());
            Destroy(transform.GetChild(0).gameObject);
        }
    }

    private IEnumerator ActivateCoin()
    {
        coinDectectorObj.SetActive(true);
        yield return new WaitForSeconds(10f);
        coinDectectorObj.SetActive(false);
    }
}
