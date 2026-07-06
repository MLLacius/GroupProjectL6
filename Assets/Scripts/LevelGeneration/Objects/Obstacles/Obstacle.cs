using UnityEngine;
//Luke script mainly, Leyton added audio, debris and dash logic, Shara did screen shake (unused)
public class Obstacle : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private AudioController audioController;
    private GameMaster gameMaster;
    [SerializeField] private bool instantGameOver = false;
    [SerializeField] private GameObject debrisParticles;

    private ScreenShake screenShake;
    
    private void Awake()
    {
        gameMaster = FindFirstObjectByType<GameMaster>();
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Hit player 
        if(collision.gameObject.CompareTag("Player"))
        {
            //PLayer isn't dashing; trigger game over/stumble
            if (!playerMovement.GetIsPlayerDashing())
            {
                if (instantGameOver)
                {
                    collision.gameObject.GetComponent<PlayerMovement>().AttemptStumble();
                    Debug.Log("Player Collision: Attempt stumble forced instead of instant game over");
                }
                else
                {
                    collision.gameObject.GetComponent<PlayerMovement>().AttemptStumble();
                    if(!collision.gameObject.GetComponent<PlayerMovement>().GetIsGameOver())
                    {
                        audioController.PlayObstacleDestroy();
                        RemoveObject();
                    }
                    Debug.Log("Player Collision: Stumble");
                }
            }
            //destroy obstacle on collision with player if they're dashing instead of triggering gameover/stumbling
            else if(playerMovement.GetIsPlayerDashing())
            {
                audioController.PlayObstacleDestroy();
                Instantiate(debrisParticles, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
                RemoveObject();
                gameMaster.AwardDashDestructionBonus();
            }
        }
        //Hit another obstacle somehow
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            ObstacleSpawner.ReturnObjectToPool(collision.gameObject);
        }
    }

    private void RemoveObject()
    {
        //Force obstacle back to pool
        ObstacleSpawner.ReturnObjectToPool(this.gameObject);
    }


    public void Update()
    {
       //Return to pool if obstacle goes off screen
       if(transform.position.z < -10f)
       {
           ObjectSpawner.ReturnObjectToPool(this.gameObject);
       }
    }
}
