using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class BathyscapheEnergyControl : MonoBehaviour
{
    public static event Action Finished;

    [SerializeField]
    private TextMeshProUGUI energyValueText;

    [SerializeField]
    private TextMeshProUGUI energyConsumptionValueText;

    public float EnergyConsumption
    {
        get
        {
            var stats = UserPreferences.Instance.playerData.GetAllStatData();
            return stats.Where(x => x.active).Sum(x => x.energyConsumption);
        }
    }

    private void Start()
    {
        ResetEnergy();
    }

    private void OnEnable()
    {
        StatisticUI.OnChange += OnStatChange;
    }

    private void OnDisable()
    {
        StatisticUI.OnChange -= OnStatChange;
    }

    public void StartEnergyConsumption()
    {
        StartCoroutine(EnergyConsumptionLoop());
    }

    private void ResetEnergy()
    {
        Bathyscaphe.Instance.data.energyValue = Bathyscaphe.Instance.data.energyStartMultiply * Bathyscaphe.Instance.data.statEnergy.value;
        EnergyStatsShow(0.0f);
    }

    private void OnStatChange(string statName, float arg2)
    {
        if (statName != "statEnergy")
            return;

        Bathyscaphe.Instance.data.energyValue = Bathyscaphe.Instance.data.energyStartMultiply * Bathyscaphe.Instance.data.statEnergy.value;
    }

    private IEnumerator EnergyConsumptionLoop()
    {
        while (true)
        {
            float energyConsumption = EnergyConsumption;
            Bathyscaphe.Instance.data.energyValue -= energyConsumption / 10;

            if (Bathyscaphe.Instance.data.energyValue <= 0)
            {
                ResetEnergy();
                Finished?.Invoke();
                break;
            }

            yield return new WaitForSecondsRealtime(0.1f);

            EnergyStatsShow(energyConsumption);
        }
    }

    private void EnergyStatsShow(float energyConsumption)
    {
        energyValueText.text = ((int)Bathyscaphe.Instance.data.energyValue).ToString();
        energyConsumptionValueText.text = energyConsumption.ToString();
    }

}