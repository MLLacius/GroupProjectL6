using UnityEngine;
using Unity.Cinemachine;
//Rob script 
public class CameraAnimation : MonoBehaviour
{
    [SerializeField] private GameObject cineCamera;
    [SerializeField] private CinemachineBrain cineBrain;
    [SerializeField] private CinemachineSplineDolly splineDolly; 
    public void CameraAnim()
    {
        cineCamera.SetActive(true);
    }
}
