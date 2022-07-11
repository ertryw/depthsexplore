using System.Collections.Generic;
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
    private float rangeMax = 18;

    [SerializeField]
    private float fadeRange = 5f;

    [SerializeField]
    private float rangeSpeedInit;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip sonarAudio;


    private float range;
    private Color pulseColor;
    private List<Collider2D> alreadyPingedColliderList;
    private Device radar;

    private void Start()
    {
        radar = new Device(pulseSpriteRenderer.transform);

        radar.SetActiveDelegate = (x) => UserPreferences.Instance.playerData.statRadar.active = x;
        radar.LevelDelegate = () => UserPreferences.Instance.playerData.statRadar.value;
        radar.CanActivateDelegate = () => LevelManager.Instance.Finished == false;

        pulseColor = pulseSpriteRenderer.color;
        alreadyPingedColliderList = new List<Collider2D>();

        audioSource.clip = sonarAudio;
    }

    private void OnEnable()
    {
        ButtonControl.Changed += OnControlChange;
        BathyscapheEnergyControl.Finished += OnPlayEnd;
    }

    private void OnDisable()
    {
        ButtonControl.Changed -= OnControlChange;
        BathyscapheEnergyControl.Finished -= OnPlayEnd;
    }

    private void OnPlayEnd()
    {
        radar.Active = false;
    }

    private void OnControlChange(string statName, bool value)
    {
        if (statName != "statRadar")
            return;

        if (value == true)
        {
            range = 0.0f;
            PlaySonarSound();
        }

        radar.Active = value;
    }

    private void PlaySonarSound()
    {
        audioSource.Play();
    }

    private void Update()
    {
        if (radar.Active == false)
            return;

        range += (rangeSpeedInit + rangeSpeedInit * radar.Level) * Time.deltaTime;

        if (range > rangeMax)
        {
            range = 0f;
            alreadyPingedColliderList.Clear();
        }

        if (range == 0.0f)
            PlaySonarSound();

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
            pulseColor.a = Mathf.Lerp(0f, 0.3f, (rangeMax - range) / fadeRange);
        else
            pulseColor.a = 0.3f;

        pulseSpriteRenderer.color = pulseColor;
    }

}
