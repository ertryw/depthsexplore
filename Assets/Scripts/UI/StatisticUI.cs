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

    private Bathyscaphe bathyscaphe;
    private StatData stat;
    private FieldInfo field;
    public static Action<string, float> OnChange;

    void Awake()
    {
        bathyscaphe = FindObjectOfType<Bathyscaphe>();

        statNameText.text = statName;

        (stat, field) = GetStatData();

        sliderColorImage.color = sliderColor;
        statSlider.maxValue = stat.maxValue;
        statSlider.value = stat.value;
    }

    private (StatData stat, FieldInfo field) GetStatData()
    {
        Type statType = typeof(BathyscapheData);
        FieldInfo field = statType.GetField(statPrefix + statName);

        return ((StatData)field.GetValue(bathyscaphe.data), field);
    }

    public void IncreaseStat()
    {
        stat.value += stat.increaseDelta;
        float cost = stat.value * bathyscaphe.data.upgradeCostMultiply;

        if (cost > UserPreferences.instance.playerData.points || stat.value >= stat.maxValue + stat.increaseDelta)
        {
            stat.value -= stat.increaseDelta;
            return;
        }
        else
        {
            field.SetValue(bathyscaphe.data, stat);
            statSlider.value = stat.value;
            OnChange?.Invoke(statPrefix + statName, stat.value);

            UserPreferences.instance.playerData.points -= (int)cost;
            UserPreferences.instance.playerData.SetFloatField(statPrefix + statName, stat.value);
            UserPreferences.instance.Save();
        }
    }


}
