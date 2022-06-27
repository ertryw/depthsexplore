using DG.Tweening;
using TMPro;
using UnityEngine;

public class GainedPointsUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI pointsText;

    [SerializeField]
    private Ease ease;

    public void Setup(int points)
    {
        if (points == 0)
            return;
            
        pointsText.text = points.ToString();
        pointsText.rectTransform.DOScale(0.05f, 0.8f)
        .SetEase(ease)
        .SetLoops(2, LoopType.Yoyo)
        .OnComplete(() => Destroy(gameObject));
    }

}
