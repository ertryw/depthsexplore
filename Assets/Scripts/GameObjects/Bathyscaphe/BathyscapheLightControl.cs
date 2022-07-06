using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Lean.Touch;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class StatIncrease
{
    public float initValue;
    public float multiplyValue;
}

public class BathyscapheLightControl : MonoBehaviour
{

    [SerializeField]
    private Light2D flashLight;

    [SerializeField]
    private CapsuleCollider2D lightScanCollider;

    [SerializeField]
    private StatIncrease intensity;

    [SerializeField]
    private StatIncrease radiusOuter;

    [SerializeField]
    private StatIncrease spotOuterAngle;

    [SerializeField]
    private StatIncrease colliderSizeX;

    [SerializeField]
    private StatIncrease colliderSizeY;

    private bool fingerDown;
    private TweenerCore<Quaternion, Quaternion, NoOptions> rotateTween;
    private Device lightDevice;

    private void Start()
    {
        lightDevice = Device.Init(flashLight.transform);
        lightDevice.Setup(() => UserPreferences.Instance.playerData.statLight.value, () => !LevelManager.Instance.IsEnded(), UpgradeFlashLight); 
        lightDevice.Active = Bathyscaphe.Instance.data.statLight.active;
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += OnFingerMove;
        ButtonControl.OnChange += OnControlChange;
        StatisticUI.OnChange += OnStatChange;
        Bathyscaphe.Finished += OnPlayEnd;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= OnFingerMove;
        ButtonControl.OnChange -= OnControlChange;
        StatisticUI.OnChange -= OnStatChange;
        Bathyscaphe.Finished -= OnPlayEnd;
    }

    private void OnPlayEnd()
    {
        lightDevice.Active = false;
    }

    private void OnStatChange(string statName, float arg2)
    {
        if (statName != "statLight")
            return;

        UpgradeFlashLight();
    }

    private void OnControlChange(string statName, bool value)
    {
        if (statName != "statLight")
            return;

        lightDevice.Active = value;
    }

    private void OnFingerMove(LeanFinger obj)
    {
        flashLight.transform.parent.RotateTo2D(obj.GetWorldPosition(1.0f), Bathyscaphe.Instance.data.rotationSpeed);
    }

    public void UpgradeFlashLight()
    {
        flashLight.intensity = intensity.initValue + (intensity.multiplyValue * lightDevice.Level);
        flashLight.pointLightOuterRadius = radiusOuter.initValue + (radiusOuter.multiplyValue * lightDevice.Level);
        flashLight.pointLightOuterAngle = spotOuterAngle.initValue + (spotOuterAngle.multiplyValue * lightDevice.Level);

        float sizeX = colliderSizeX.initValue + (colliderSizeX.multiplyValue * lightDevice.Level);
        float sizeY = colliderSizeY.initValue + (colliderSizeY.multiplyValue * lightDevice.Level);

        lightScanCollider.size = new Vector2(sizeX, sizeY);
    }
}