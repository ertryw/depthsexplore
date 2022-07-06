using System;
using UnityEngine;

public class Device
{
    private Action upgrade;
    private Func<float> getLevel;
    private Func<bool> canActive;

    private Transform deviceObject;
    private bool isActive = false;

    public Transform DeviceGameObject => deviceObject;
    public float Level => getLevel();

    public bool Active
    {
        get => isActive;

        set
        {
            if (value && canActive?.Invoke() == false)
                return;

            isActive = value;

            if (deviceObject != null)
                deviceObject.gameObject.SetActive(value);
        }
    }

    public static Device Init(Transform deviceObject = null)
    {
        Device device = new Device
        {
            deviceObject = deviceObject,
        };

        return device;
    }

    public void Setup(Func<float> getLevel, Func<bool> canActive, Action upgrade)
    {
        this.getLevel = getLevel;
        this.canActive = canActive;
        this.upgrade = upgrade;
        
        Upgrade();
    } 

    public void Upgrade()
    {
        upgrade?.Invoke();
    }

}