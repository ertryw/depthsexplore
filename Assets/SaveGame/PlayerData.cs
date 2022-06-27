using System;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string saveDate = "";
    public int points = 0;
    public int deepestLevel = 0;
    public float statEnergy = 0.0f;
    public float statLight = 0.0f;
    public float statRadar = 0.0f; 
    public float statScanner = 0.0f;
    public float statSteering = 0.0f;

    public object GetField(string fieldName)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        return field.GetValue(this);
    }

    public void SetFloatField(string fieldName, float value)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        field.SetValue(this, value);

        // PlayerPrefs.SetFloat(fieldName, value);
        // PlayerPrefs.Save();
    }


}