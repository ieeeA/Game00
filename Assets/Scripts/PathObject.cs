using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathObject : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _CheckPoints;

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _CheckPoints.Length; i++)
        {
            Gizmos.DrawSphere(_CheckPoints[i].transform.position, 0.5f);
            Gizmos.DrawLine(_CheckPoints[i].transform.position, _CheckPoints[(i + 1) % (_CheckPoints.Length)].transform.position);
        }
    }

    public Vector3? ChoiseNextDest(Vector3 pos, float reachRange, bool inverse)
    {
        var list = _CheckPoints.Select(x => x.transform.position)
            .Select((v, i) => new { v = v, i = i })
            .OrderBy(x => Vector3.Distance(x.v, pos))
            .Select(x => x.i)
            .ToList();

        if (list.Count == 0)
        {
            return null;
        }
        var i = list.FirstOrDefault();
        return _CheckPoints[(i + 1) % (_CheckPoints.Length)].transform.position;
    }
}
