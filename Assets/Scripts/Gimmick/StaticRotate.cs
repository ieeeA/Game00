using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticRotate : MonoBehaviour
{
    [SerializeField]
    private float _Speed;
    [SerializeField]
    private Vector3 _Axis = Vector3.up;

    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(_Speed * Time.deltaTime, _Axis);
    }
}
