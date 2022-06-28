using System;
using System.Collections;
using Hellmade.Sound;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Bathyscaphe : MonoBehaviour
{
    public static event Action Finished;
    public static event Action SurfaceEntry;

    [SerializeField]
    private Button upgradeButton;

    [SerializeField]
    private UpgradePanelUI upgradePanel;

    [SerializeField]
    private Slider steeringSlider;

    [SerializeField]
    private float steeringMiddle;

    [SerializeField]
    [Range(0, 50)]
    private float steeringMiddleSpeed;

    [Space(20)]
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private GainedPointsUI gainedPointsUIPrefab;

    [SerializeField]
    private MyControl movement;

    private Vector3 initPosition;
    private Transform initParent;

    [HideInInspector]
    public BathyscapheLightControl lightControl;

    [HideInInspector]
    public BathyscapheEnergyControl energyControl;

    [HideInInspector]
    public BathyscapheRadar radarControl;

    public BathyscapheData data;
    public BathyscapeState State { get; set; }
    public UnityEvent onSurface;
    public UnityEvent onWater;
    public UnityEvent onDepthWaterEvent;
    public Action onDepthWater;

    [SerializeField]
    private UnityEvent OnStartSwim;

    public AudioClip motorAudio;

    private void Awake()
    {
        SetOnSurface();
        lightControl = GetComponent<BathyscapheLightControl>();
        lightControl.bathyscaphe = this;

        energyControl = GetComponent<BathyscapheEnergyControl>();
        energyControl.bathyscaphe = this;
        radarControl = GetComponent<BathyscapheRadar>();
        radarControl.bathyscaphe = this;

        movement = new MyControl();
        movement.Enable();

        data.statLight.active = false;
        data.statRadar.active = false;

        initPosition = transform.position;
        initParent = transform.parent;
    }

    public void StartSwim()
    {
        if (LevelManager.Instance.playEnds == false)
        {
            OnStartSwim?.Invoke();
        }
    }

    private void ResetBathyscaphe()
    {
        SetOnSurface();
        rb.simulated = false;
        transform.parent = initParent;
        transform.position = initPosition;
        data.energyValue = data.energyStartMultiply * data.statEnergy.value;
    }

    private void OnEnable()
    {
        WaterObject.scannedEnd += ScannedEnd;
        UserPreferences.onFirstPlay += FirstPlay;
        UserPreferences.onNextPlay += NextPlay;
        WinPanelUI.onWinPanel += ResetBathyscaphe;
    }

    private void OnDisable()
    {
        WaterObject.scannedEnd -= ScannedEnd;
        UserPreferences.onFirstPlay -= FirstPlay;
        UserPreferences.onNextPlay -= NextPlay;
        WinPanelUI.onWinPanel -= ResetBathyscaphe;
    }

    private void FirstPlay()
    {
        Debug.Log("firstPlay");
        UserPreferences.instance.playerData.points = data.startPoints;
        UserPreferences.instance.playerData.statEnergy = data.statEnergy.InitValue();
        UserPreferences.instance.playerData.statLight = data.statLight.InitValue();
        UserPreferences.instance.playerData.statRadar = data.statRadar.InitValue();
        UserPreferences.instance.playerData.statScanner = data.statScanner.InitValue();
        UserPreferences.instance.playerData.statSteering = data.statSteering.InitValue();

        UserPreferences.instance.Save();
    }

    private void NextPlay()
    {
        Debug.Log("nextPlay");
        data.statEnergy.value = UserPreferences.instance.playerData.statEnergy;
        data.statLight.value = UserPreferences.instance.playerData.statLight;
        data.statRadar.value = UserPreferences.instance.playerData.statRadar;
        data.statScanner.value = UserPreferences.instance.playerData.statScanner;
        data.statSteering.value = UserPreferences.instance.playerData.statSteering;
    }

    private void ScannedEnd(WaterObject obj)
    {
        GainedPointsUI gainedPoints = Instantiate(
            gainedPointsUIPrefab,
            obj.transform.position,
            Quaternion.identity
        );

        int points = (int)(obj.pointsGainK * -data.depth * obj.rarityData.multiply[(int)obj.rarityType]);
        gainedPoints.Setup(points);

        UserPreferences.instance.playerData.points += points;
        UserPreferences.instance.Save();
    }

    Audio motorAudioEazy;

    private void PlayMotorSound()
    {
        float motorVolume = 0.4f;

        if (motorAudioEazy == null)
        {
            int soundId = EazySoundManager.PrepareSound(motorAudio);
            motorAudioEazy = EazySoundManager.GetAudio(soundId);
            motorAudioEazy.Pitch = 1.3f;
            motorAudioEazy.Play(motorVolume);
        }
        else
        {
            if (motorAudioEazy.Volume == 0.0f)
            {
                motorAudioEazy.SetVolume(motorVolume);
            }

            if (motorAudioEazy.IsPlaying == false)
            {
                motorAudioEazy.Play(motorVolume);
            }
        }
    }

    private void FixedUpdate()
    {
        if (data.energyValue <= 0)
        {
            if (LevelManager.Instance.playEnds == false)
            {
                LevelManager.Instance.playEnds = true;
                Finished?.Invoke();
            }

            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 move = Vector2.zero;

        if (State is BathyscapheInWater || State is BathyscapeInDepthWater)
        {
            move = movement.Bathyscaphe.Move.ReadValue<Vector2>();
            rb.AddForce(data.steeringSpeedMultiply * data.statSteering.value * move);
        }

        if (move.x != 0)
            data.statSteering.active = true;
        else
            data.statSteering.active = false;

        if (move.y != 0)
        {
            rb.velocity = Vector2.ClampMagnitude(
                rb.velocity,
                State.MaxVelocity + (data.statSteering.value * 2)
            );
            data.statSteeringDown.active = true;
        }
        else
        {
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, State.MaxVelocity);
            data.statSteeringDown.active = false;
        }

        if (move.magnitude > 0)
        {
            PlayMotorSound();
        }
        else
        {
            if (motorAudioEazy != null)
            {
                motorAudioEazy.SetVolume(0.0f);
            }
        }

        // Stering(steeringSlider.value);
    }

    // Steering for Mobile
    
    // public void Steer(float directionMagnitude)
    // {
    //     rb.AddForce(Vector2.right * directionMagnitude);
    // }

    // private void Stering(float value)
    // {
    //     float steeringValue = value - steeringMiddle;

    //     if (steeringValue.IsBetweenRange(-5, 5) == false)
    //         data.statSteering.active = true;
    //     else
    //         data.statSteering.active = false;

    //     Steer(steeringValue);
    // }

    public void SteringReset()
    {
        StartCoroutine(ResetSteringCoroutine());
    }

    public IEnumerator ResetSteringCoroutine()
    {
        steeringSlider.interactable = false;

        while ( Mathf.Ceil(steeringSlider.value) > Mathf.Ceil(steeringMiddle) + 1
            || Mathf.Ceil(steeringSlider.value) < Mathf.Ceil(steeringMiddle) - 1
        )
        {
            steeringSlider.value = Mathf.Lerp(
                steeringSlider.value,
                steeringMiddle,
                Time.deltaTime * steeringMiddleSpeed
            );
            yield return new WaitForEndOfFrame();
        }

        steeringSlider.interactable = true;
    }

    public void SetOnSurface()
    {
        if (State is BathyscapeOnSurface)
            return;

        upgradeButton.interactable = true;
        State = new BathyscapeOnSurface(data.surfaceMaxVelocity);

        data.statLight.active = false;
        data.statRadar.active = false;

        SurfaceEntry?.Invoke();
        onSurface?.Invoke();
    }

    public void SetInWater()
    {
        if (State is BathyscapheInWater)
            return;

        State = new BathyscapheInWater(data.waterMaxVelocity);
        onWater?.Invoke();

        upgradeButton.interactable = false;
        upgradePanel.HidePanel();
    }

    public void SetInDepthWater()
    {
        if (State is BathyscapeInDepthWater)
            return;

        State = new BathyscapeInDepthWater(data.depthWaterMaxVelocity);
        onDepthWaterEvent.Invoke();
        onDepthWater?.Invoke();
    }
}
