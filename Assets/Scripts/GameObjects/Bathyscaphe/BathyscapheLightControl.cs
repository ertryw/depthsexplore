using System;
using DG.Tweening;
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
    private Transform targetRotation;

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

    [HideInInspector]
    public Bathyscaphe bathyscaphe;

    private void Start()
    {
        SetActiveFlashLights(bathyscaphe.data.statLight.active);
        SetupFlashLight();
    }

    private void OnEnable()
    {
        LeanTouch.OnFingerUpdate += OnFingerMove;
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
        ButtonControl.OnChange += OnControlChange;
        StatisticUI.OnChange += OnStatChange;
        Bathyscaphe.Finished += OnPlayEnd;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerUpdate -= OnFingerMove;
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
        ButtonControl.OnChange -= OnControlChange;
        StatisticUI.OnChange -= OnStatChange;
        Bathyscaphe.Finished -= OnPlayEnd;
    }

    private void OnPlayEnd()
    {
        SetActiveFlashLights(false);
    }

    private void OnFingerUp(LeanFinger obj)
    {
        if (rotateTween != null)
            rotateTween.Kill();

        fingerDown = false;
    }

    private void OnStatChange(string statName, float arg2)
    {
        if (statName != "statLight")
            return;

        SetupFlashLight();
    }

    private void OnControlChange(string statName, bool value)
    {
        if (statName != "statLight")
            return;

        Debug.Log(statName);
        SetActiveFlashLights(value);
    }

    private void OnFingerDown(LeanFinger obj)
    {
        if (obj.IsOverGui)
            return;

        //RotateToTween(obj.GetWorldPosition(1.0f), bathyscaphe.data.rotationDuration, () => fingerDown = true);
        RotateTo(obj.GetWorldPosition(1.0f));
        fingerDown = true;
    }

    private void OnFingerMove(LeanFinger obj)
    {
        if (obj.IsOverGui || fingerDown == false)
            return;

        RotateTo(obj.GetWorldPosition(1.0f));
    }

    public void RotateToTween(Vector3 target, float duration, TweenCallback onComplete)
    {
        Vector3 dir = target - transform.position;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90.0f;

        rotateTween = transform.DORotateQuaternion(Quaternion.AngleAxis(angle, Vector3.forward), duration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(onComplete);
    }

    public void SetupFlashLight()
    {
        float statLight = UserPreferences.instance.playerData.statLight;

        flashLight.intensity = intensity.initValue + (intensity.multiplyValue * statLight);
        flashLight.pointLightOuterRadius = radiusOuter.initValue + (radiusOuter.multiplyValue * statLight);
        flashLight.pointLightOuterAngle = spotOuterAngle.initValue + (spotOuterAngle.multiplyValue * statLight);

        float sizeX = colliderSizeX.initValue + (colliderSizeX.multiplyValue * statLight);
        float sizeY = colliderSizeY.initValue + (colliderSizeY.multiplyValue * statLight);

        lightScanCollider.size = new Vector2(sizeX, sizeY);
    }

    public void RotateTo(Vector3 target)
    {
        Vector3 dir = target - targetRotation.position;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90.0f;
        Quaternion qAngle = Quaternion.AngleAxis(angle, Vector3.forward);
        targetRotation.rotation = Quaternion.Slerp(targetRotation.rotation, qAngle, Time.deltaTime * bathyscaphe.data.rotationSpeed);
    }

    public void SetActiveFlashLights(bool value)
    {
        if (LevelManager.Instance.playEnds)
            return;

        flashLight.gameObject.SetActive(value);
    }
}