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


    private void Start()
    {
        Bathyscaphe.Instance.data.energyValue = Bathyscaphe.Instance.data.energyStartMultiply * Bathyscaphe.Instance.data.statEnergy.value;
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

        Bathyscaphe.Instance.data.energyValue = Bathyscaphe.Instance.data.energyStartMultiply * Bathyscaphe.Instance.data.statEnergy.value;
    }

    private IEnumerator EnergyConsumption()
    {
        while (true)
        {
            (List<StatData> stat, List<FieldInfo> field) d = UserPreferences.Instance.playerData.GetAllStatData();
            float energyConsumption = d.stat.Where(x => x.active).Sum(x => x.energyConsumption);

            Bathyscaphe.Instance.data.energyValue -= energyConsumption / 10;

            yield return new WaitForSecondsRealtime(0.1f);

            energyValueText.text = ((int)Bathyscaphe.Instance.data.energyValue).ToString();
            energyConsumptionValueText.text = energyConsumption.ToString();
        }
    }

}