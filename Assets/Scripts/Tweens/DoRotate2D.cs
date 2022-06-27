using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class DoRotate2D : MonoBehaviour
{
    [SerializeField]
    [Range(0, 360)]
    private float angle;

    [SerializeField]
    [Range(-360, 360)]
    private float startAngle;

    [SerializeField]
    [Range(-360, 360)]
    private float endAngle;

    [SerializeField]
    private bool enableOnStart;

    [SerializeField]
    private float duration;

    [SerializeField]
    private Ease ease;

    [SerializeField]
    private RotateMode rotateMode;

    private TweenerCore<Quaternion, Vector3, QuaternionOptions> tween;

    private IEnumerator Start()
    {
        if (enableOnStart == false)
            yield break;


        tween = RotateTick(startAngle);

        yield return tween.WaitForCompletion();

        StartSwing();
    }

    private void OnEnable()
    {
        WinPanelUI.onWinPanelComplete += StartSwing;
    }

    private void OnDisable()
    {
        WinPanelUI.onWinPanelComplete -= StartSwing;
    }

    private TweenerCore<Quaternion, Vector3, QuaternionOptions> RotateTick(float angle)
    {
        return transform.DORotate(Vector3.forward * angle, duration, RotateMode.Fast)
            .SetEase(ease);
    }

    public void StartSwing()
    {
        tween = transform.DORotate(Vector3.forward * angle, duration, rotateMode)
            .SetEase(ease)
            .SetLoops(-1, LoopType.Yoyo); 
    }

    public void StopTween()
    {
        StartCoroutine(LateStopTween());
    }

    private IEnumerator LateStopTween()
    {
        yield return new WaitForSecondsRealtime(duration);

        if (tween != null)
        {
            tween.Kill();
            RotateTick(endAngle);
        }
    }

}
