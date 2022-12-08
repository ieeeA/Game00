using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtil
{
    public static Vector3 RandomSphere(float radius)
    {
        var theta = Random.Range(0, 1.0f) * Mathf.PI * 2;
        var phi = Random.Range(0, 1.0f) * Mathf.PI;
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(phi), Mathf.Sin(theta)).normalized * radius * Random.Range(0, 1.0f);
    }

    public static Quaternion RandomYRot()
    {
        return Quaternion.Euler(0, Random.Range(0, 1.0f) * Mathf.PI * 2, 0);
    }

    public static Vector2 Orbit2(float theta)
    {
        return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)).normalized;
    }

    public static Vector2 Orbit3(float theta, float phi)
    {
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(phi), Mathf.Sin(theta)).normalized;
    }
}
