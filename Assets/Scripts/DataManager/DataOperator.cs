using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataOperator : MonoBehaviour
{
    [SerializeField]
    private GameObject _DropItemPrefab;
    [SerializeField]
    private Vector3 _DropOffset;

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void DropItem()
    {
        var obj = Instantiate(_DropItemPrefab);
        obj.transform.position = transform.position + _DropOffset;
    }
}
