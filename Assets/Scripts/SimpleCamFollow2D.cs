//using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class SimpleCamFollow2D : MonoBehaviour
{
    public Transform target;

    [Range(0.0f, 5.0f)]
    public float laziness;
    public Vector3 generalOffset;
    private Vector3 whereCameraShouldBe;

    private void Update()
    {
        if (target != null)
        {
            whereCameraShouldBe = new Vector3(target.position.x, target.position.y, transform.position.z) + generalOffset;
            transform.position = Vector3.Lerp(transform.position, whereCameraShouldBe, Time.deltaTime * laziness);
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
