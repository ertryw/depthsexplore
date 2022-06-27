using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TextSavedStatUI : MonoBehaviour
{
    [SerializeField]
    private string statName;

    [SerializeField]
    private TextMeshProUGUI statText;

    private void Update()
    {
        object stat = UserPreferences.instance.playerData.GetField(statName);
        statText.text = stat.ToString();
    }
}