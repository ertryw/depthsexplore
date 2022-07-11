using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Light2D globalLigth;

    [SerializeField]
    private float globalLightDarkIntense;

    [SerializeField]
    [Range(0, 1)]
    private float darkeningSpeed;

    [SerializeField]
    private TextMeshProUGUI psiValueText;

    [SerializeField]
    public EarthData earthData;

    [SerializeField]
    private AudioClip scanningAudio;

    private float globalLightIntesityInit;

    public static LevelManager Instance { get; private set; }
    public bool Finished { get; set; } = false;



    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;


        globalLightIntesityInit = globalLigth.intensity;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2.0f);
    }

    private void DecreaseLight()
    {
        StartCoroutine(DecreaseLightCoroutine());
    }

    private IEnumerator DecreaseLightCoroutine()
    {
        float initIntensity = globalLigth.intensity;

        while (globalLigth.intensity >= globalLightDarkIntense && Finished == false)
        {
            float level = (Bathyscaphe.Instance.data.depth + earthData.depthWaterLevel) / 100;
            globalLigth.intensity = Mathf.Lerp(initIntensity, 0, Mathf.Abs(darkeningSpeed * level));
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnEnable()
    {
        Bathyscaphe.DepthWaterEntry += DecreaseLight;
        WinPanelUI.onWinPanelComplete += OnWinComplete;
        WinPanelUI.onWinPanel += OnWin;
    }

    private void OnDisable()
    {
        Bathyscaphe.DepthWaterEntry -= DecreaseLight;
        WinPanelUI.onWinPanelComplete -= OnWinComplete;
        WinPanelUI.onWinPanel -= OnWin;
    }

    private void OnWin()
    {
        Finished = true;
        globalLigth.intensity = globalLightIntesityInit;

        Water[] waters = FindObjectsOfType<Water>();

        foreach (Water item in waters)
        {
            item.DestoryWater();
        }
    }

    private void OnWinComplete()
    {
        Water[] waters = FindObjectsOfType<Water>();

        foreach (Water item in waters)
        {
            item.Setup();
        }
    }

    private void Update()
    {
        if (Bathyscaphe.Instance != null)
        {
            Bathyscaphe.Instance.data.depth = Bathyscaphe.Instance.transform.position.y * earthData.psiMultiply;

            if (Bathyscaphe.Instance.data.depth < 0 && (Bathyscaphe.Instance.State is BathyscapeOnSurface))
            {
                Bathyscaphe.Instance.SetInWater();
            }

            if (Bathyscaphe.Instance.data.depth < 0)
            {

                if (Bathyscaphe.Instance.data.depth < earthData.depthWaterLevel * (-1) && Bathyscaphe.Instance.State is BathyscapheInWater)
                {
                    Bathyscaphe.Instance.SetInDepthWater();
                }

                psiValueText.text = (-(int)Bathyscaphe.Instance.data.depth).ToString(); // In water
            }
            else
            {
                psiValueText.text = ((int)earthData.depthOnEarth).ToString(); // On surface
            }
        }
    }

}
