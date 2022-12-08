using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public static class FieldUtil
{
    public static GameObject[] Search(Vector3 pos, float radius, string tag = null)
    {
        if (tag == null)
        {
            return Physics.OverlapSphere(pos, radius)
                .Select(x => x.gameObject)
                .ToArray();
        }
        else
        {
            return Physics.OverlapSphere(pos, radius)
                .Where(x => x.tag == tag)
                .Select(x => x.gameObject)
                .ToArray();
        }
    }

    public static Collider[] SearchCollider(Vector3 pos, float radius, string tag = null)
    {
        if (tag == null)
        {
            return Physics.OverlapSphere(pos, radius)
                .ToArray();
        }
        else
        {
            return Physics.OverlapSphere(pos, radius)
                .Where(x => x.tag == tag)
                .ToArray();
        }
    }
    public static Collider[] SearchBoxCollider(Vector3 pos, Vector3 halfExts, Quaternion rot, string tag = null)
    {
        if (tag == null)
        {
            return Physics.OverlapBox(pos, halfExts, rot)
                .ToArray();
        }
        else
        {
            return Physics.OverlapBox(pos, halfExts, rot)
                .Where(x => x.tag == tag)
                .ToArray();
        }
    }

    public static bool GetPositionRaycastToTerrain(Vector3 from, float range, out Vector3 pos)
    {
        RaycastHit hit;
        var res = Physics.Raycast(new Ray(from, Vector3.down), out hit, range, LayerMask.GetMask("Terrain"));
        pos = hit.point;
        return res;
    }
}

