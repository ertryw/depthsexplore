using UnityEngine;

public static class MathExtensions
{
    public static bool IsBetweenRange(this float thisValue, float value1, float value2)
    {
        return thisValue >= Mathf.Min(value1, value2) && thisValue <= Mathf.Max(value1, value2);
    }

    public static bool IsBetweenRange(this int thisValue, int value1, int value2)
    {
        return thisValue >= Mathf.Min(value1, value2) &&
        thisValue <= Mathf.Max(value1, value2);
    }
}
