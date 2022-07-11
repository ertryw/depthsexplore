using DG.Tweening;
using System;
using UnityEngine;

public class Device
{
    public Func<float> LevelDelegate { get; set; }
    public Func<bool> CanActivateDelegate { get; set; }
    public Action<bool> SetActiveDelegate { get; set; }
    public Action UpgradeDelegate { get; set; }

    private bool isActive;
    private Transform deviceObject;

    public Transform DeviceGameObject => deviceObject;
    public float Level => LevelDelegate.Invoke();


    public bool Active
    {
        get => isActive;

        set
        {
            if (value && CanActivateDelegate?.Invoke() == false)
                return;

            isActive = value;
            SetActiveDelegate.Invoke(value);

            if (deviceObject != null)
                deviceObject.gameObject.SetActive(value);
        }
    }

    public Device(Transform deviceObject = null)
    {
        this.deviceObject = deviceObject;
    }

    public void Upgrade()
    {
        UpgradeDelegate?.Invoke();
    }

}