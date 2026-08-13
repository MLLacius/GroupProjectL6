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
    private bool isPanningForDash;

    private void Start()
    {
        cineBrain = GetComponent<CinemachineBrain>();
        startingFOV = cineCam.Lens.FieldOfView;
    }

    public void CameraAnim()
    {
        cineCamera.SetActive(true);
    }


    public IEnumerator CameraPanIn(float timeToComplete)
    {
        isPanningForDash = true;
        float elapsedTime = 0f;
        float currentFOV = cineCam.Lens.FieldOfView;
        float targetFOV = cineCam.Lens.FieldOfView - 10;
        float startingSplineOffset = splineDolly.SplineOffset.y;
        float targetSplineOffset = startingSplineOffset - 0.5f;

        while(elapsedTime < timeToComplete)
        {
            elapsedTime += Time.deltaTime;

            float lerpValue = elapsedTime / timeToComplete;

            cineCam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, lerpValue);
            splineDolly.SplineOffset.y = Mathf.Lerp(startingSplineOffset, targetSplineOffset, lerpValue);

            yield return null;
        }
    }

    public IEnumerator CameraPanOut(float timeToComplete)
    {
        float elapsedTime = 0f;
        float currentFOV = cineCam.Lens.FieldOfView;
        float targetFOV = cineCam.Lens.FieldOfView + 10;
        float startingSplineOffset = splineDolly.SplineOffset.y;
        float targetSplineOffset = startingSplineOffset + 0.5f;

        while (elapsedTime < timeToComplete)
        {
            elapsedTime += Time.deltaTime;

            float lerpValue = elapsedTime / timeToComplete;

            cineCam.Lens.FieldOfView = Mathf.Lerp(currentFOV, targetFOV, lerpValue);
            splineDolly.SplineOffset.y = Mathf.Lerp(startingSplineOffset, targetSplineOffset, lerpValue);
            Debug.Log("FOV: " + cineCam.Lens.FieldOfView);

            yield return null;
        }
        isPanningForDash = false;
    }

    public void UpdateCameraPan(float levelSpeed)
    {
        //float newPan = ((startingFOV / (levelSpeed * 700)) / 60);
        float newPan = (startingFOV + 10) - levelSpeed;
        cineCam.Lens.FieldOfView = newPan;
    }

    public float GetCameraFOV()
    {
        return cineCam.Lens.FieldOfView;
    }

    public bool GetIsPanningForDash()
    {
        return isPanningForDash;
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
