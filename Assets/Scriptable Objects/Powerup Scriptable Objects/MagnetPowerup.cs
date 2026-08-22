using UnityEngine;

[CreateAssetMenu(menuName = "Powerup/Magnet Powerup")]
public class MagnetPowerup : PowerUpEffect
{
    [SerializeField] private float radius = 25f;
    [SerializeField] private float attractionSpeed = 20f;
    private MagnetIconFlash magnetIcon; //The icon to display when the powerup is active
    public override void OnActivate(GameObject target)
    {
        Debug.Log("Magnet Powerup activated!");
        //Find the magnet icon in the player's UI and enable it
        magnetIcon = FindFirstObjectByType<MagnetIconFlash>();
        if(magnetIcon)
        {
            magnetIcon.MagnetPickedUp();
            Debug.Log("Magnet Icon found");
            return;
        }
        Debug.LogWarning("Magnet Icon not found");
    }

    public override void OnTick(GameObject target)
    {
        //Debug.Log("Magnet Powerup ticked!");
        //Use find objects by tag to find all collectibles named collectibles in the scene
        //If within radius, move towards player
        if(magnetIcon != null)
        {
            magnetIcon.MagnetIconUpdated();
        }
        GameObject[] collectibles = GameObject.FindGameObjectsWithTag("Collectible");
        foreach (GameObject collectible in collectibles)
        {
            float distance = Vector3.Distance(target.transform.position, collectible.transform.position);
            if (distance <= radius)
            {
                //Move towards player
                Vector3 direction = (target.transform.position - collectible.transform.position).normalized;
                collectible.transform.position += direction * Time.deltaTime * attractionSpeed;
            }
        }
    }

    public override void OnDeactivate(GameObject target)
    {
        Debug.Log("Magnet Powerup deactivated!");
        if(magnetIcon != null)
        {
            magnetIcon.MagnetPowerupHidden();
        }
    }
}
