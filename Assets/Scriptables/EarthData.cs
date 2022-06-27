using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EarthData", order = 1)]
public class EarthData : ScriptableObject
{
    public float depthOnEarth;
    public float psiMultiply;
    public float depthWaterLevel;

}