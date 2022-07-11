using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Bathyscaphe : MonoBehaviour
{
    public static event Action SurfaceEntry;
    public static event Action DepthWaterEntry;
    public static event Action WaterEntry;

    private static Bathyscaphe _instance;
    public static Bathyscaphe Instance => _instance;

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

    [SerializeField]
    private UnityEvent OnStartSwim;

    public AudioClip motorAudio;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        initPosition = transform.position;
        initParent = transform.parent;
    }

    private void Start()
    {
        SetOnSurface();
    }

    public void StartSwim()
    {
        if (LevelManager.Instance.Finished == false)
        {
            Debug.Log("StartSwim");
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
        WaterObject.ScannedEnd += ScannedEnd;
        WinPanelUI.onWinPanel += ResetBathyscaphe;
        UserPreferences.FirstPlay += FirstPlay;
        UserPreferences.NextPlay += NextPlay;
    }

    private void OnDisable()
    {
        WaterObject.ScannedEnd -= ScannedEnd;
        WinPanelUI.onWinPanel -= ResetBathyscaphe;
        UserPreferences.FirstPlay -= FirstPlay;
        UserPreferences.NextPlay -= NextPlay;
    }

    private void FirstPlay()
    {
        Debug.Log("firstPlay");
        UserPreferences.Instance.playerData.points = data.startPoints;
        UserPreferences.Instance.playerData.statEnergy = data.statEnergy.Init();
        UserPreferences.Instance.playerData.statLight = data.statLight.Init();
        UserPreferences.Instance.playerData.statRadar = data.statRadar.Init();
        UserPreferences.Instance.playerData.statScanner = data.statScanner.Init();
        UserPreferences.Instance.playerData.statSteering = data.statSteering.Init();
        UserPreferences.Instance.Save();
    }

    private void NextPlay()
    {
        Debug.Log("nextPlay");

        data.startPoints = UserPreferences.Instance.playerData.points;
        data.statEnergy = UserPreferences.Instance.playerData.statEnergy;
        data.statLight = UserPreferences.Instance.playerData.statLight;
        data.statRadar = UserPreferences.Instance.playerData.statRadar;
        data.statScanner = UserPreferences.Instance.playerData.statScanner;
        data.statSteering = UserPreferences.Instance.playerData.statSteering;
    }

    private void ScannedEnd(WaterObject obj)
    {
        GainedPointsUI gainedPoints = Instantiate(gainedPointsUIPrefab, obj.transform.position, Quaternion.identity);

        int points = (int)(obj.pointsGainK * -data.depth * obj.rarityData.multiply[(int)obj.rarityType]);
        gainedPoints.Setup(points);

        UserPreferences.Instance.playerData.points += points;
        UserPreferences.Instance.Save();
    }

#if UNITY_ANDROID

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

        while (Mathf.Ceil(steeringSlider.value) > Mathf.Ceil(steeringMiddle) + 1
            || Mathf.Ceil(steeringSlider.value) < Mathf.Ceil(steeringMiddle) - 1)
        {
            steeringSlider.value = Mathf.Lerp(steeringSlider.value, steeringMiddle, Time.deltaTime * steeringMiddleSpeed );
            yield return new WaitForEndOfFrame();
        }

        steeringSlider.interactable = true;
    }
#endif

    public void SetOnSurface()
    {
        if (State is BathyscapeOnSurface)
            return;

        Debug.Log("set on surface");
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
        WaterEntry?.Invoke();

        upgradeButton.interactable = false;
        upgradePanel.HidePanel();
    }

    public void SetInDepthWater()
    {
        if (State is BathyscapeInDepthWater)
            return;

        State = new BathyscapeInDepthWater(data.depthWaterMaxVelocity);
        onDepthWaterEvent.Invoke();
        DepthWaterEntry?.Invoke();
    }
}
