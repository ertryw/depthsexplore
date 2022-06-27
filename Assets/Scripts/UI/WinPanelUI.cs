using DG.Tweening;
using TMPro;
using UnityEngine;

public class WinPanelUI : MonoBehaviour
{
    public delegate void OnWinPanelComplete();
    public static event OnWinPanelComplete onWinPanelComplete;
    public delegate void OnWinPanel();
    public static event OnWinPanel onWinPanel;

    [SerializeField]
    private RectTransform imageBathyscaphe;

    [SerializeField]
    private RectTransform congratulationPanel;

    [SerializeField]
    private TextMeshProUGUI depthValueText;

    [SerializeField]
    private Vector2 hidePosition;

    [SerializeField]
    private Vector2 showPosition;

    [SerializeField]
    private BathyscapheData controlData;

    // Update is called once per frame
    private RectTransform rectTransform;
    private Tween moveTween;

    // Start is called before the first frame update
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowHidePanel()
    {
        if (moveTween != null && moveTween.IsComplete() == false)
            return;

        if (gameObject.activeSelf == false)
            gameObject.SetActive(true);

        if (rectTransform.anchoredPosition == hidePosition)
        {
            moveTween = rectTransform.DOAnchorPos(showPosition, 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() => onWinPanel?.Invoke())
                .SetAutoKill(false);

            OnShow();
        }
        else
        {
            moveTween = rectTransform.DOAnchorPos(hidePosition, 0.5f).SetEase(Ease.Linear).SetAutoKill(false);
        }
    }

    private void OnShow()
    {
        congratulationPanel.gameObject.SetActive(false);
        imageBathyscaphe.gameObject.SetActive(false);
        
        if (UserPreferences.instance.playerData.deepestLevel > controlData.depth)
        {
            congratulationPanel.gameObject.SetActive(true);
            UserPreferences.instance.playerData.deepestLevel = (int)controlData.depth;
            UserPreferences.instance.Save(); 
        }
        else
        {
            imageBathyscaphe.gameObject.SetActive(true);
        }

        depthValueText.text = ((int)controlData.depth).ToString();
    }

    private void OnEnable()
    {
        Bathyscaphe.Finished += ShowHidePanel;
    }

    private void OnDisable()
    {
        Bathyscaphe.Finished -= ShowHidePanel;
    }

    public void HidePanel()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (rectTransform.anchoredPosition == showPosition)
        {
            if (moveTween != null)
                moveTween.Complete();

            moveTween = rectTransform.DOAnchorPos(hidePosition, 1f)
                .OnComplete(() => gameObject.SetActive(false))
                .SetEase(Ease.OutBack)
                .OnComplete(() => onWinPanelComplete?.Invoke())
                .SetAutoKill(false);

            LevelManager.Instance.playEnds = false;
        }
    }
}
