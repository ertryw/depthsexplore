using System;
using System.Reflection;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticUI : MonoBehaviour
{
    [SerializeField]
    private string statPrefix;

    [SerializeField]
    private string statName;

    [SerializeField]
    private TextMeshProUGUI statNameText;

    [Space(10)]
    [SerializeField]
    private Slider statSlider;

    [SerializeField]
    private Image sliderColorImage;

    [SerializeField]
    private Color sliderColor;

    private StatData stat;
    private FieldInfo field;
    private string statFullName;
    public static Action<string, float> OnChange;

    void Start()
    {
        statNameText.text = statName;
        statFullName = statPrefix + statName;
        
        (stat, field) = GetStatData();

        sliderColorImage.color = sliderColor;
        statSlider.maxValue = stat.maxValue;
        statSlider.value = stat.value;
    }

    private (StatData stat, FieldInfo field) GetStatData()
    {
        Type statType = typeof(BathyscapheData);
        FieldInfo field = statType.GetField(statFullName);

        return ((StatData)field.GetValue(Bathyscaphe.Instance.data), field);
    }

    public void IncreaseStat()
    {
        stat.value += stat.increaseDelta;
        float cost = stat.value * Bathyscaphe.Instance.data.upgradeCostMultiply;

        if (cost > UserPreferences.Instance.playerData.points || stat.value >= stat.maxValue + stat.increaseDelta)
        {
            stat.value -= stat.increaseDelta;
            return;
        }
        else
        {
            field.SetValue(Bathyscaphe.Instance.data, stat);
            statSlider.value = stat.value;
            OnChange?.Invoke(statFullName, stat.value);

            UserPreferences.Instance.playerData.points -= (int)cost;
            UserPreferences.Instance.playerData.SetStatValueField(statFullName, stat.value);
            UserPreferences.Instance.Save();
        }
    }


}
