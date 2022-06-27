using UnityEngine;
using DG.Tweening;
using System;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class SpriteRendererDoColor : MonoBehaviour
{
    public event Action Start;
    public event Action Complete;
    private float duration;
    private Ease ease;
    private SpriteRenderer spriteRenderer;
    private TweenerCore<Color, Color, ColorOptions> tween;
    private Color color;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public SpriteRendererDoColor DoColor()
    {
        tween = spriteRenderer.DOColor(color, duration).SetEase(ease);
        return this;
    }

    public SpriteRendererDoColor OnStart(TweenCallback start)
    {
        tween.OnStart(start);
        return this;
    }

    public SpriteRendererDoColor OnComplete(TweenCallback start)
    {
        tween.OnComplete(start);
        return this;
    }

    public SpriteRendererDoColor SetCompleteColor(Color color)
    {
        this.color = color;
        return this;
    }

    public SpriteRendererDoColor SetStartColor(Color color)
    {
        spriteRenderer.color = color;
        return this;
    }

    public SpriteRendererDoColor Duration(float duration)
    {
        this.duration = duration;
        return this;
    }

    public SpriteRendererDoColor SetEase(Ease ease)
    {
        this.ease = ease;
        return this;
    }
}
