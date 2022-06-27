using System;
using System.Collections;
using Hellmade.Sound;
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
    private Bathyscaphe bathyscaphe;

    [SerializeField]
    public EarthData earthData;

    [SerializeField]
    private AudioClip scanningSound;

    public static LevelManager Instance { get; private set; }

    [HideInInspector]
    public bool playEnds = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(2.0f);
    }

    private void DecreaseLight()
    {
        StartCoroutine(DecreaseLightCoroutine());
    }

    private void ActiveFlashLights()
    {
        bathyscaphe.lightControl.SetActiveFlashLights(true);
    }
    

    private IEnumerator DecreaseLightCoroutine()
    {
        float initIntensity = globalLigth.intensity;

        while (globalLigth.intensity >= globalLightDarkIntense)
        {
            float level = (-bathyscaphe.data.depth + earthData.depthWaterLevel) / 100;
            globalLigth.intensity = Mathf.Lerp(initIntensity, 0, darkeningSpeed * level);
            yield return new WaitForEndOfFrame();
        }
    }
    
    private void OnEnable()
    {
        bathyscaphe.onDepthWater += DecreaseLight;
        WinPanelUI.onWinPanelComplete += OnWinComplete;
        WinPanelUI.onWinPanel += OnWin;

    }

    private void OnDisable()
    {
        bathyscaphe.onDepthWater -= DecreaseLight;
        WinPanelUI.onWinPanelComplete -= OnWinComplete;
        WinPanelUI.onWinPanel -= OnWin;
    }

    private void OnWin()
    {
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
        if (bathyscaphe != null)
        {
            bathyscaphe.data.depth = bathyscaphe.transform.position.y * earthData.psiMultiply;

            if (bathyscaphe.data.depth < 0 && (bathyscaphe.State is BathyscapeOnSurface))
            {
                bathyscaphe.SetInWater();
            }

            if (bathyscaphe.data.depth < 0)
            {

                if (bathyscaphe.data.depth  < earthData.depthWaterLevel * (-1) && bathyscaphe.State is BathyscapheInWater)
                {
                    bathyscaphe.SetInDepthWater();
                }

                psiValueText.text = (-(int)bathyscaphe.data.depth).ToString(); // In water
            }
            else
            {
                psiValueText.text = ((int)earthData.depthOnEarth).ToString(); // On surface
            }
        }
    }

    Audio scanningAudio;
    public void PlayScanningSound()
    {
        float motorVolume = 0.6f;

        if (scanningAudio == null)
        {
            int soundId = EazySoundManager.PrepareSound(scanningSound);
            scanningAudio = EazySoundManager.GetAudio(soundId);
            scanningAudio.Pitch = 1f;
            scanningAudio.Play(motorVolume);
            scanningAudio.Loop = true;
        }
        else
        {
            if (scanningAudio.Volume == 0.0f)
            {
                scanningAudio.SetVolume(motorVolume);
            }

            if (scanningAudio.IsPlaying == false)
            {
                scanningAudio.Play(motorVolume);
            }
        }
    }

    public void StopScanningSound()
    {
        if (scanningAudio != null)
        {
            scanningAudio.SetVolume(0.0f);
        }
    }
}
