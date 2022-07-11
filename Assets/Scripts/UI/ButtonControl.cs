using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonControl : MonoBehaviour
{    
    private StatData stat;
    
    public Button button;
    public Image image;
    public Sprite ImageOn;
    public Sprite ImageOff;

    public string statName;
    public static Action<string, bool> Changed;
    
    public void SetActive(bool value)
    {
        image.sprite = value ? ImageOn : ImageOff;
    }

    private void Awake()
    {
        StatData stat = UserPreferences.Instance.playerData.GetField<StatData>(statName);
        SetActive(stat.active);
    }

    private void OnEnable()
    {
        Bathyscaphe.SurfaceEntry += ResetControl;
    }

    private void OnDisable()
    {
        Bathyscaphe.SurfaceEntry -= ResetControl;
    }

    private void ResetControl()
    {
        SetActive(false);
    }

    public void OnClick()
    {
        StatData stat = UserPreferences.Instance.playerData.GetField<StatData>(statName);
        
        if (stat.value == 0.0f)
            return;

        if (stat.active)
            stat.active = false;
        else
            stat.active = true;

        SetActive(stat.active);
        Changed?.Invoke(statName, stat.active);
    }

}
