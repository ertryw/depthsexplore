using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BathyscapeData", order = 1)]
public class BathyscapheData : ScriptableObject
{
    public int startPoints;
    public int upgradeCostMultiply;

    [Range(0, 100000)]
    public float energyStartMultiply;

    [HideInInspector]
    public float energyValue;

    [HideInInspector]
    public float depth;

    [Space(10)]
    public float rotationSpeed;
    public float rotationDuration;
    public float steeringSpeedMultiply; 
    [Space(10)]
    public float surfaceMaxVelocity;
    public float waterMaxVelocity;
    public float depthWaterMaxVelocity;

    [Space(10)]
    public StatData statEnergy;
    public StatData statLight;
    public StatData statRadar;
    public StatData statScanner;
    public StatData statSteering;
    public StatData statSteeringDown;
    
    public Action<string, float> OnSetFloatStat;

    public T GetField<T>(string fieldName)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        return (T)field.GetValue(this);
    }

    public T SetField<T>(string fieldName, T value)
    {
        Type statType = GetType();
        FieldInfo field = statType.GetField(fieldName);
        field.SetValue(this, value);
        return (T)field.GetValue(this);
    }
}




