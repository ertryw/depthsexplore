using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonControl : MonoBehaviour
{    
    [SerializeField]
    private BathyscapheData controlData;
    private StatData stat;
    
    public Button button;
    public Image image;
    public Sprite ImageOn;
    public Sprite ImageOff;

    public string statName;
    public static Action<string, bool> OnChange;
    
    public void SetActive(bool value)
    {
        image.sprite = value ? ImageOn : ImageOff;
    }
    private void OnEnable()
    {
        Bathyscaphe.SurfaceEntry += Reset;
    }

    private void OnDisable()
    {
        Bathyscaphe.SurfaceEntry -= Reset;
    }

    private void Reset()
    {
        SetActive(false);
    }

    public void OnClick()
    {
        StatData stat = controlData.GetField<StatData>(statName);
        
        if (stat.value == 0.0f)
            return;

        if (stat.active)
            stat.active = false;
        else
            stat.active = true;

        SetActive(stat.active);
        controlData.SetField(statName, stat);
        OnChange?.Invoke(statName, stat.active);
    }

    private void Awake()
    {
        StatData stat = controlData.GetField<StatData>(statName);
        SetActive(stat.active);
    }
}
