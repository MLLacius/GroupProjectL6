using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

//Luke and Shara script
public class ScreenFading : MonoBehaviour
{
    public UnityEvent onFadeComplete;

    [SerializeField] private float targetAlpha; //Final alpha value at the end of fade
    [SerializeField] private float startingAlpha; //Initial alpha value at the start of fade
    [SerializeField] private float duration; //How long the fade will take
    [SerializeField] private Image image;
    private Color imageColor;

    public void StartFade()
    {
        if(image == null){ return; }

        imageColor = image.color;
        imageColor.a = startingAlpha;
        
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            //Calculate how far along we are
            float lerpValue = elapsedTime / duration;

            //Move alpha from start to target based on time
            imageColor.a = Mathf.Lerp(startingAlpha, targetAlpha, lerpValue);
            image.color = imageColor;

            yield return null;
        }

        //Ensure we hit the exact target alpha at the end
        imageColor.a = targetAlpha;
        image.color = imageColor;
        onFadeComplete.Invoke();
    }
}