using System;
using System.Collections;
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
    private bool autoFucusing;

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
    private WaterObject focusedWaterObject;

    private void Start()
    {
        lightDevice = new Device(flashLight.transform);

        lightDevice.SetActiveDelegate = (x) => UserPreferences.Instance.playerData.statLight.active = x;
        lightDevice.LevelDelegate = () => UserPreferences.Instance.playerData.statLight.value;
        lightDevice.CanActivateDelegate = () => LevelManager.Instance.Finished == false;
        lightDevice.UpgradeDelegate = UpgradeFlashLight;

        lightDevice.Active = Bathyscaphe.Instance.data.statLight.active;

        if (autoFucusing)
            StartCoroutine(AutoFocusing());
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += OnFingerMove;
        ButtonControl.Changed += OnControlChange;
        StatisticUI.OnChange += OnStatChange;
        BathyscapheEnergyControl.Finished += OnPlayEnd;

        if (autoFucusing)
        {
            WaterObject.Click += OnClick;
            WaterObject.UnClick += OnUnClick;
        }
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= OnFingerMove;
        ButtonControl.Changed -= OnControlChange;
        StatisticUI.OnChange -= OnStatChange;
        BathyscapheEnergyControl.Finished -= OnPlayEnd;

        if (autoFucusing)
        {
            WaterObject.Click -= OnClick;
            WaterObject.UnClick -= OnUnClick;
        }
    }

    private void OnClick(WaterObject obj)
    {
        Debug.Log("Click");
        focusedWaterObject = obj;
    }

    private void OnUnClick(WaterObject obj)
    {
        Debug.Log("UnClick");
        focusedWaterObject = null;
    }

    private IEnumerator AutoFocusing()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (focusedWaterObject == null)
                continue;
            
            flashLight.transform.parent.RotateTo2D(focusedWaterObject.transform.position, Bathyscaphe.Instance.data.rotationSpeed);
        }
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
        if (obj.IsOverGui || focusedWaterObject != null)
            return;

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