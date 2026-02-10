using UnityEngine;
//Luke script
//Powerup collectible; no powerup logic is held in this class
public class PowerupCollection : MonoBehaviour
{
    [SerializeField] private PowerUpEffect powerupEffect;
    [SerializeField] private float duration;

    private void OnTriggerEnter(Collider other)
    {
        var manager = other.gameObject.GetComponent<ActivePowerupManager>();
        if(manager != null)
        {
            manager.ActivatePowerup(powerupEffect, duration);
            Destroy(gameObject);
        }
    }
}