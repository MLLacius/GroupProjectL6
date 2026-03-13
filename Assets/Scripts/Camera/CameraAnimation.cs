using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
//Rob script 
public class CameraAnimation : MonoBehaviour
{
    [SerializeField] private GameObject cineCamera;
    [SerializeField] private CinemachineCamera cineCam;
    [SerializeField] private CinemachineSplineDolly splineDolly;
    private CinemachineBrain cineBrain;

    private float startingFOV;
    private float targetFOV;
    private float startingSplineOffset;
    private float targetSplineOffset;

    private void Start()
    {
        cineBrain = GetComponent<CinemachineBrain>();
        startingFOV = cineCam.Lens.FieldOfView;
        targetFOV = startingFOV - 10;
        startingSplineOffset = splineDolly.SplineOffset.y;
        targetSplineOffset = startingSplineOffset - 0.5f;
    }

    public void CameraAnim()
    {
        cineCamera.SetActive(true);
    }

    public IEnumerator CameraPanIn(float timeToComplete)
    {
        float elapsedTime = 0f;

        while(elapsedTime < timeToComplete)
        {
            elapsedTime += Time.deltaTime;

            float lerpValue = elapsedTime / timeToComplete;

            cineCam.Lens.FieldOfView = Mathf.Lerp(startingFOV, targetFOV, lerpValue);
            splineDolly.SplineOffset.y = Mathf.Lerp(startingSplineOffset, targetSplineOffset, lerpValue);

            yield return null;
        }
    }

    public IEnumerator CameraPanOut(float timeToComplete)
    {
        float elapsedTime = 0f;

        while (elapsedTime < timeToComplete)
        {
            elapsedTime += Time.deltaTime;

            float lerpValue = elapsedTime / timeToComplete;

            cineCam.Lens.FieldOfView = Mathf.Lerp(targetFOV, startingFOV, lerpValue);
            splineDolly.SplineOffset.y = Mathf.Lerp(targetSplineOffset, startingSplineOffset, lerpValue);

            yield return null;
        }
    }

    public float GetCameraFOV()
    {
        return cineCam.Lens.FieldOfView;
    }

    public void SetCameraFOV(float fov)
    {
        cineCam.Lens.FieldOfView = fov;
    }

    public void SetSplineOffset(float offset)
    {
        splineDolly.SplineOffset.y = offset;
    }
}
