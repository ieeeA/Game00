using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSample : MonoBehaviour, IProjectileHit
{
    private int _HitCount;

    [SerializeField]
    private GameObject dropObject;

    private void Start()
    {
        _HitCount = 10;
    }

    public void Hit(object bullet, ProjectileHitInfo hitInfo)
    {
        _HitCount--;
        Debug.Log($"[TargetSample] Count: {_HitCount}");
        if (_HitCount < 0)
        {
            Debug.Log("[TargetSample] Destoryed Target");
            GameObject.Destroy(gameObject);

            var newItem = GameObject.Instantiate(dropObject);
            newItem.transform.position = transform.position;
        }
    }
}
