using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class BathyscapheEnergyControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI energyValueText;

    [SerializeField]
    private TextMeshProUGUI energyConsumptionValueText;

    [HideInInspector]
    public Bathyscaphe bathyscaphe;

    public bool ConsumeEnergy { get; set; }

    private void Start()
    {
        bathyscaphe.data.energyValue = bathyscaphe.data.energyStartMultiply * bathyscaphe.data.statEnergy.value;
        StartCoroutine(EnergyConsumption());
    }


    private void OnEnable()
    {
        StatisticUI.OnChange += OnStatChange;
    }

    private void OnDisable()
    {
        StatisticUI.OnChange -= OnStatChange;
    }

    private void OnStatChange(string statName, float arg2)
    {
        if (statName != "statEnergy")
            return;

        bathyscaphe.data.energyValue = bathyscaphe.data.energyStartMultiply * bathyscaphe.data.statEnergy.value;
    }

    private IEnumerator EnergyConsumption()
    {
        while (true)
        {
            (List<StatData> stat, List<FieldInfo> field) d = GetAllStatData();
            float energyConsumption = d.stat.Where(x => x.active).Sum(x => x.energyConsumption);

            bathyscaphe.data.energyValue -= energyConsumption / 10;

            yield return new WaitForSecondsRealtime(0.1f);

            energyValueText.text = ((int)bathyscaphe.data.energyValue).ToString();
            energyConsumptionValueText.text = energyConsumption.ToString();
        }
    }
    
    private (List<StatData> stat, List<FieldInfo> field) GetAllStatData()
    {
        Type statType = typeof(BathyscapheData);
        FieldInfo[] fields = statType.GetFields();
        List<FieldInfo> statFields = fields.Where(x => x.Name.Contains("stat")).ToList();
        List<StatData> stats = statFields.Select(x => (StatData)x.GetValue(bathyscaphe.data)).ToList();
        return (stats, statFields);
    }


}