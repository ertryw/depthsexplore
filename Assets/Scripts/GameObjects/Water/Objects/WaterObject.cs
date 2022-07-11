using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public enum RarityType
{
    Common, Rare, Legendary
}

public abstract class WaterObject : MonoBehaviour
{
    public static event Action<WaterObject> ScannedEnd;
    public static event Action<WaterObject> Click;
    public static event Action<WaterObject> UnClick;

    [SerializeField]
    public bool bomb;

    [SerializeField]
    private Sprite commnonSprite, rareSprite, legendarySprite;

    [SerializeField]
    private AudioSource OnDestoryClip;

    [SerializeField]
    private float scanDuration;

    [SerializeField]
    private Canvas canvas;

    public float pointsGainK;

    public int fromDepth;

    [SerializeField]
    private Slider scanningSlider;

    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public RarityType rarityType;

    public RarityData rarityData;

    private bool toDestroy;
    private float scanTime;

    private void Awake()
    {
        if (scanningSlider != null)
        {
            scanningSlider.gameObject.SetActive(false);
            scanningSlider.minValue = 0;
            scanningSlider.maxValue = scanDuration;
            scanningSlider.value = 0;
        }

        int perCent = UnityEngine.Random.Range(0, 100);

        if (perCent < rarityData.percents[0])
        {
            rarityType = RarityType.Common;
            SetSprite(commnonSprite);
        }
        else if (perCent < rarityData.percents[0] + rarityData.percents[1])
        {
            rarityType = RarityType.Rare;
            SetSprite(rareSprite);
        }
        else if (perCent < rarityData.percents[0] + rarityData.percents[1] + rarityData.percents[2])
        {
            rarityType = RarityType.Legendary;
            SetSprite(legendarySprite);
        }
    }

    public void Select()
    {
        Click?.Invoke(this);
    }

    public void DeSelect()
    {
        UnClick?.Invoke(this);
    }

    public void SetCommonSprite()
    {
        spriteRenderer.sprite = commnonSprite;
    }

    public void SetSprite(Sprite sprite)
    {
        if (sprite == null)
            SetCommonSprite();

        spriteRenderer.sprite = sprite;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        bool isLight = col.gameObject.TryGetComponent(out Light2D light);

        if (isLight == false || pointsGainK == 0.0f)
            return;

        if (scanningSlider != null)
            scanningSlider.gameObject.SetActive(true);

        if (canvas != null)
            canvas.gameObject.SetActive(true);

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        bool isLight = col.gameObject.TryGetComponent(out Light2D light);

        if (isLight == false || pointsGainK == 0.0f)
            return;

        if (scanningSlider != null)
            scanningSlider.gameObject.SetActive(false);

        if (canvas != null)
            canvas.gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (pointsGainK == 0.0f || scanningSlider.gameObject.activeSelf == false)
            return;

        if (scanningSlider != null)
            scanningSlider.value = scanTime;

        scanTime += Time.fixedDeltaTime * (1 + UserPreferences.Instance.playerData.statScanner.value);

        if (scanTime >= scanDuration && toDestroy == false)
        {
            toDestroy = true;
            ScannedEnd?.Invoke(this);
            DestroyObject(0.5f, true);
        }
    }

    public void DestroyObject(float speed, bool playSound)
    {
        transform.DOScale(0, speed)
            .SetEase(Ease.InBack)
            .OnComplete(() => StartCoroutine(PrepareToDestory(playSound)));
    }

    private IEnumerator PrepareToDestory(bool playSound)
    {

        if (playSound)
        {
            OnDestoryClip.Play();

            while (OnDestoryClip.isPlaying)
                yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

}