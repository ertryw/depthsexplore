using DG.Tweening;
using UnityEngine;

public class UpgradePanelUI : MonoBehaviour
{
    [SerializeField]
    private Vector2 hidePosition;

    [SerializeField]
    private Vector2 showPosition;

    private RectTransform rectTransform;
    private Tween moveTween;

    // Start is called before the first frame update
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowHidePanel()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (moveTween != null && moveTween.IsComplete() == false)
            return;

        if (rectTransform.anchoredPosition == hidePosition)
            moveTween = rectTransform.DOAnchorPos(showPosition, 0.5f).SetEase(Ease.Linear).SetAutoKill(false);
        else
            moveTween = rectTransform.DOAnchorPos(hidePosition, 0.5f).SetEase(Ease.Linear).SetAutoKill(false); 
    }

    public void HidePanel()
    {
        if (moveTween != null)
            moveTween.Complete();

        moveTween = rectTransform.DOAnchorPos(hidePosition, 0.5f).SetEase(Ease.Linear).SetAutoKill(false); 
    }

}
