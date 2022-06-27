using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TextStatUI : MonoBehaviour
{
    [SerializeField]
    private string statName;

    [SerializeField]
    private BathyscapheData controlData;
    
    [SerializeField]
    private TextMeshProUGUI statText;

    private void Update()
    {
        object stat = controlData.GetField<object>(statName);
        statText.text = stat.ToString();
    }
}