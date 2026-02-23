using Unity.Cinemachine;
using UnityEngine;
//Rob script 
public class CameraAnimation : MonoBehaviour
{
    [SerializeField] private GameObject cineCamera;
    [SerializeField] private CinemachineCamera cineCam;
    [SerializeField] private CinemachineSplineDolly splineDolly;
    private CinemachineBrain cineBrain;

    private void Start()
    {
        cineBrain = GetComponent<CinemachineBrain>();
    }

    public void CameraAnim()
    {
        cineCamera.SetActive(true);
    }

    public void CameraPanIn(float i, float decreaseAmount)
    {
        if (cineCam.Lens.FieldOfView > i)
        {
            cineCam.Lens.FieldOfView -= decreaseAmount;
            Debug.Log("FOV: " + cineCam.Lens.FieldOfView);
        }

        if(splineDolly.SplineOffset.y > -0.5)
        {
            splineDolly.SplineOffset.y -= decreaseAmount / 10;
        }
    }

    public void CameraPanOut(float i, float increaseAmount)
    {
        if (cineCam.Lens.FieldOfView < i)
        {
            cineCam.Lens.FieldOfView += increaseAmount;
            Debug.Log("FOV: " + cineCam.Lens.FieldOfView);
        }

        if (splineDolly.SplineOffset.y < 0)
        {
            splineDolly.SplineOffset.y += increaseAmount / 10;
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
