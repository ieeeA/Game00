using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GizmoHelper
{
    public static void DrawOBB(Vector3 pos, Vector3 halfExt, Quaternion rot)
    {
        Vector3 minpos = pos - rot * halfExt;
        Vector3 maxpos = pos + rot * halfExt;

        Vector3 p0 = minpos;
        Vector3 p1 = minpos + rot * new Vector3(halfExt.x, 0, 0) * 2;
        Vector3 p2 = minpos + rot * new Vector3(0, halfExt.y, 0) * 2;
        Vector3 p3 = minpos + rot * new Vector3(0, 0, halfExt.z) * 2;

        Vector3 p4 = maxpos;
        Vector3 p5 = maxpos - rot * new Vector3(halfExt.x, 0, 0) * 2;
        Vector3 p6 = maxpos - rot * new Vector3(0, halfExt.y, 0) * 2;
        Vector3 p7 = maxpos - rot * new Vector3(0, 0, halfExt.z) * 2;

        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p0, p2);
        Gizmos.DrawLine(p0, p3);

        Gizmos.DrawLine(p2, p5);
        Gizmos.DrawLine(p2, p7);

        Gizmos.DrawLine(p3, p5);

        Gizmos.DrawLine(p4, p5);
        Gizmos.DrawLine(p4, p6);
        Gizmos.DrawLine(p4, p7);

        Gizmos.DrawLine(p6, p1);
        Gizmos.DrawLine(p6, p3);

        Gizmos.DrawLine(p1, p7);
    }
}
