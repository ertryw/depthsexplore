using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/RarityData", order = 1)]
public class RarityData : ScriptableObject
{
    public int[] percents;
    public Color[] colors;
    public float[] multiply;
}