
using System;

[Serializable]
public class StatData
{
    public bool active;
    public float value;
    public float initValue;
    public float maxValue;
    public float increaseDelta;
    public float energyConsumption;
    public float InitValue() 
    {
        value = initValue;
        return value;
    }
}