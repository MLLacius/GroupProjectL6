using System.Collections;
using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    [SerializeField] private GameObject creditsHolder;
    [SerializeField] private float scrollAmountSeconds;
    private float startingY;

    private void OnEnable()
    {
        startingY = creditsHolder.transform.localPosition.y;
        StartCoroutine(ScrollCredits());
    }

    private IEnumerator ScrollCredits()
    {


        if(creditsHolder.transform.localPosition.y < startingY)
        {
            yield return null;
        }
    }

    private void OnDisable()
    {
        StopCoroutine(ScrollCredits());
        Vector2 pos = creditsHolder.transform.localPosition;
        pos.y = startingY;
        creditsHolder.transform.localPosition = pos;
    }
}
