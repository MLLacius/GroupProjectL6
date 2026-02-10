using System.Collections.Generic;
using UnityEngine;

//Luke script

//This script will sit on any object which can collect a powerup (player)
//It will have some kind of data structure with every powerup name with a duration as the 2nd value
public class ActivePowerupManager : MonoBehaviour
{
    //Dictionary of all active powerups and durations
    private Dictionary<PowerUpEffect, float> powerupDurations = new Dictionary<PowerUpEffect, float>();
    //List of all powerups to remove 
    private List<PowerUpEffect> powerupsToRemove = new List<PowerUpEffect>();

    // Update is called once per frame
    private void Update()
    {
        HandleTimers();
    }

    private void HandleTimers()
    {
        var values = new List<PowerUpEffect>(powerupDurations.Keys);

        foreach (var effect in values)
        {
            powerupDurations[effect] -= Time.deltaTime;
            effect.OnTick(gameObject);
            if(powerupDurations[effect] <= 0)
            {
                powerupsToRemove.Add(effect);
            }
        }
        foreach(var effect in powerupsToRemove)
        {
            powerupDurations.Remove(effect);
            effect.OnDeactivate(gameObject);
        }
        powerupsToRemove.Clear();
    }

    public void ActivatePowerup(PowerUpEffect powerup, float duration)
    {
        //If we already have powerup, reset it's duration
        if(powerupDurations.ContainsKey(powerup))
        {
            powerupDurations[powerup] = duration;
        }
        else
        {
            powerupDurations.Add(powerup, duration);
            powerup.OnActivate(gameObject);
        }
    }

    public float GetRemainingDuration(PowerUpEffect powerup)
    {
        return powerupDurations[powerup];
    }
    public bool HasPowerup(PowerUpEffect powerup)
    {
        return powerupDurations.ContainsKey(powerup);
    }
}
