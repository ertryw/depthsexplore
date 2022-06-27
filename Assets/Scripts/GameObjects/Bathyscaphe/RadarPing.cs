using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class RadarPing : MonoBehaviour
{
    [SerializeField]
    private AudioSource PingAudio;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Color dangerColor;

    [SerializeField]
    private Color goodColor;
    private TweenerCore<Color, Color, ColorOptions> tween;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetDisappearTimer(bool danger)
    {
        PingAudio.Play();

        if (UserPreferences.instance.playerData.statRadar > 0.5f)
        {
            if (danger)
                spriteRenderer.color = dangerColor;
            else
                spriteRenderer.color = goodColor;
        }

        tween = spriteRenderer
            .DOColor(new Color(0, 0, 0, 0), 0.5f)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));

    }

    public RadarPing OnStart(TweenCallback start)
    {
        tween.OnStart(start);
        return this;
    }

}
