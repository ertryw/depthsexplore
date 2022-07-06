using UnityEngine;

public static class TransformExtensions
{
    public static void RotateTo2D(this Transform transform, Vector3 target, float speed)
    {
        Vector3 dir = target - transform.position;
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90.0f;
        Quaternion qAngle = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, qAngle, Time.deltaTime * speed);
    }
}