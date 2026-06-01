using System.Collections;
using UnityEngine;
//Leyton script
public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource collectibleCollect;
    [SerializeField] private AudioSource dashing;
    [SerializeField] private AudioSource hardPlayerImpact;
    [SerializeField] private AudioSource obstacleDestroy;
    [SerializeField] private AudioSource playerMove, playerJump;

    private float timePassed = 0;

    public void PlayCollectibleCollect()
    {
        collectibleCollect.Play();
        collectibleCollect.pitch += 0.03f;
        timePassed = 0f;
        StartCoroutine(CountTimePassed());
    }

    private IEnumerator CountTimePassed()
    {
        while (timePassed < 1)
        {
            timePassed += Time.deltaTime;
            yield return null;
        }
        if (timePassed >= 1)
        {
            collectibleCollect.pitch = 1f;
            yield return null;
        }
    }

    public void PlayDashing()
    {
        dashing.Play();
        dashing.loop = !dashing.loop;
    }
     
    public void PlayHardPlayerImpact()
    {
        hardPlayerImpact.Play();
    }

    public void PlayObstacleDestroy()
    {
        obstacleDestroy.Play();
    }

    public void PlayPlayerMove()
    {
        playerMove.Play();
    }

    public void PlayPlayerJump()
    {
        playerJump.Play();
    }
}