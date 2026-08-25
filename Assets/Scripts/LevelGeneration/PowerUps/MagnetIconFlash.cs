using System;
using UnityEngine;
using UnityEngine.UI;
//Luke script

public class MagnetIconFlash : MonoBehaviour
{
    [SerializeField] private Image magnetIcon;
    [SerializeField] private float flashSpeed;
    [Tooltip("The time in seconds of the magnet duration remaning to start flashing the icon")]
    [SerializeField] private float durationStartFlashing;
    private float magnetDurationRemanining; //Time in seconds
    [SerializeField] private ActivePowerupManager playerPowerupManager;
    [SerializeField] private PowerUpEffect magnetPowerup;
    public void MagnetPickedUp()
    {
        if(magnetIcon)
        {
            magnetIcon.enabled = true;
            SetImageAlpha(0);
            return;
        }
    }

    public void MagnetIconUpdated()
    {
        if(playerPowerupManager)
        {
            float remaningMagnetDuration = playerPowerupManager.GetRemainingDuration(magnetPowerup);
            if(remaningMagnetDuration <= durationStartFlashing && remaningMagnetDuration > 0)
            {
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * flashSpeed));
                SetImageAlpha(alpha);
            }
            else
            {
                SetImageAlpha(1);
            }
        }
    }

    public void MagnetPowerupHidden()
    {
        magnetIcon.enabled = false;
    }

    private void SetImageAlpha(float alpha)
    {
        if(magnetIcon)
        {
            Color color = magnetIcon.color;
            color.a = alpha;
            magnetIcon.color = color;
        }
    }
}
