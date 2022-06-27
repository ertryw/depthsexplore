using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DoUIScale : MonoBehaviour
{
    [SerializeField]
    private float duration;

    [SerializeField]
    private Ease ease;

    [SerializeField]
    private float scale;

    [SerializeField]
    private bool enableOnStart;

    private TweenerCore<Vector3, Vector3, VectorOptions> tween;
    
    private IEnumerator Start()
    {
        if (enableOnStart == false)
            yield break;

        tween = transform.DOScale(new Vector3(1, 1, 0) * scale, duration)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void StopTween()
    {
        if (tween != null)
        {
            tween.Kill();
        }
    }

}
