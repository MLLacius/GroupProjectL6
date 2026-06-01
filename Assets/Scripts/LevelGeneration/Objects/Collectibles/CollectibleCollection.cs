using UnityEngine;
//Luke script, Leyton added audio logic
public class CollectibleCollection : MonoBehaviour
{
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private AudioController audioController;
    [SerializeField] private ParticleSystem collectibleCollectEffect;

    private void Awake()
    {
        if (!gameMaster)
        {
            gameMaster = GameObject.Find("Game Master").GetComponent<GameMaster>();
        }

        if (!audioController)
        {
            audioController = GameObject.Find("AudioController").GetComponent<AudioController>();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            gameMaster.IncrementCollectiblesGained();
        }

        if (gameMaster.GetGameplayState())
        {
            audioController.PlayCollectibleCollect();
            Instantiate(collectibleCollectEffect, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
        }
        Destroy(gameObject);
    }
}
