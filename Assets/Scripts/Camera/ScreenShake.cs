using System.Collections;
using UnityEngine;
//Shara script
public class ScreenShake : MonoBehaviour
{
    [Header("Shake Settings")]
    //Duration of the screen shake effect
    public float shakeDuration = 0.2f;

    //How far the camera moves during the shake
    public float shakeMagnitude = 0.5f;

    [SerializeField] private bool startShake;

    //controls the shape of the shake
    public AnimationCurve dampingCurve = new AnimationCurve
    (
        new Keyframe(0, 0), //start at the center
        new Keyframe(0.5f, 1), //peak at half duration
        new Keyframe(1, 0) //end at the center smoothly
    );

    private Vector3 initialPosition;
    private Coroutine currentShakeCoroutine;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void TriggerShake(Vector3 direction)
    {
        //stops any jittery overlapping shakes
        if (currentShakeCoroutine != null)
        {
            StopCoroutine(currentShakeCoroutine);
            transform.localPosition = initialPosition;
        }

        //if not shaking, set init position
        else
        {
            initialPosition = transform.localPosition;
        }
        currentShakeCoroutine = StartCoroutine(DoShake(direction));
    }

    private IEnumerator DoShake(Vector3 direction)
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            //calculating how far along we are in the shake
            float percentage = elapsed / shakeDuration;

            //evaluate the curve and strength of the shake
            float curveStrength = dampingCurve.Evaluate(percentage);

            //direction * strength * curve
            Vector3 offset = direction * shakeMagnitude * curveStrength;

            //apply local pososition
            transform.localPosition = initialPosition + offset;

            yield return null;
        }

        //ensure we end at the initial position
        transform.localPosition = initialPosition;
        currentShakeCoroutine = null;
    }

}
