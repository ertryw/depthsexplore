using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



[Serializable]
public class PlayerData
{
    public StatData statEnergy;
    public StatData statLight;
    public StatData statRadar;
    public StatData statScanner;
    public StatData statSteering;
    public StatData statSteeringDown;

    public string saveDate = "";
    public int points = 0;
    public int deepestLevel = 0;

    public T GetField<T>(string fieldName)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        return (T)field.GetValue(this);
    }

    public List<StatData> GetAllStatData()
    {
        FieldInfo[] fields = this.GetType().GetFields();
        List<FieldInfo> statFields = fields.Where(x => x.Name.Contains("stat")).ToList();
        List<StatData> stats = statFields.Select(x => (StatData)x.GetValue(this)).ToList();
        return stats;
    }

    public void SetStatValueField(string fieldName, float value)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        StatData statData = (StatData)field.GetValue(this);
        statData.value = value;
        field.SetValue(this, statData);
    }

}