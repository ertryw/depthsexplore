using System;
using System.Collections.Generic;
using Hellmade.Sound;
using UnityEngine;

public class BathyscapheRadar : MonoBehaviour
{

    [SerializeField]
    private Transform radarPingPrefab;

    [SerializeField]
    private LayerMask radarLayerMask;

    [SerializeField]
    private SpriteRenderer pulseSpriteRenderer;

    [SerializeField]
    private float rangeSpeedInit;

    public AudioClip clickAudio;
    public AudioClip sonarAudio;

    private float range;
    private float rangeMax;
    private float fadeRange;
    private Color pulseColor;
    private List<Collider2D> alreadyPingedColliderList;

    [HideInInspector]
    public Bathyscaphe bathyscaphe;

    private void Start()
    {
        pulseColor = pulseSpriteRenderer.color;
        rangeMax = 18f;
        fadeRange = 5f;
        alreadyPingedColliderList = new List<Collider2D>();
        pulseSpriteRenderer.gameObject.SetActive(bathyscaphe.data.statRadar.active);
    }

    private void OnEnable()
    {
        ButtonControl.OnChange += OnControlChange;
        Bathyscaphe.Finished += OnPlayEnd;
    }

    private void OnDisable()
    {
        ButtonControl.OnChange -= OnControlChange;
        Bathyscaphe.Finished -= OnPlayEnd;
    }

    private void OnPlayEnd()
    {
        pulseSpriteRenderer.gameObject.SetActive(false);
    }

    private void OnControlChange(string statName, bool value)
    {
        if (statName != "statRadar")
            return;

        int soundID = EazySoundManager.PrepareSound(clickAudio);
        Audio clickMusicAudio = EazySoundManager.GetAudio(soundID);
        clickMusicAudio.Pitch = 0.5f;
        clickMusicAudio.Play(0.4f);

        if (value == true)
        {
            PlaySonarSound();
        }

        pulseSpriteRenderer.gameObject.SetActive(value);
    }

    private void PlaySonarSound()
    {
        int soundId = EazySoundManager.PrepareSound(sonarAudio);
        Audio clickMusicAudio = EazySoundManager.GetAudio(soundId);
        clickMusicAudio.Pitch = 1f;
        clickMusicAudio.Play(1f);
    }

    private void Update()
    {
        if (bathyscaphe.data.statRadar.active == false || LevelManager.Instance.playEnds)
        {
            range = 0.0f;
            return;
        }

        range += (rangeSpeedInit + rangeSpeedInit * UserPreferences.instance.playerData.statRadar) * Time.deltaTime;

        if (range > rangeMax)
        {
            PlaySonarSound();
            range = 0f;
            alreadyPingedColliderList.Clear();
        }

        pulseSpriteRenderer.transform.localScale = new Vector3(range, range);
        RaycastHit2D[] raycastHit2DArray = Physics2D.CircleCastAll(transform.position, range / 1.2f, Vector2.zero, 0f, radarLayerMask);

        foreach (RaycastHit2D raycastHit2D in raycastHit2DArray)
        {
            if (raycastHit2D.collider != null)
            {
                // Hit something
                if (!alreadyPingedColliderList.Contains(raycastHit2D.collider))
                {
                    bool hit = raycastHit2D.collider.gameObject.TryGetComponent(out WaterObject waterObject);

                    if (hit == false)
                        continue;

                    alreadyPingedColliderList.Add(raycastHit2D.collider);

                    Transform radarPingTransform = Instantiate(radarPingPrefab, raycastHit2D.collider.transform.position, Quaternion.identity);
                    RadarPing radarPing = radarPingTransform.GetComponent<RadarPing>();

                    radarPing.SetDisappearTimer(waterObject.bomb);
                }
            }
        }

        // Fade Pulse
        if (range > rangeMax - fadeRange)
        {
            pulseColor.a = Mathf.Lerp(0f, 0.3f, (rangeMax - range) / fadeRange);
        }
        else
        {
            pulseColor.a = 0.3f;
        }

        pulseSpriteRenderer.color = pulseColor;
    }

}
