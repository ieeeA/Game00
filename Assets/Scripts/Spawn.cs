using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System;

public class Spawn : MonoBehaviour
{

    [SerializeField]
    private GameObject prefab;

    [SerializeField]
    private int spawnCount = 5;

    [SerializeField]
    private float radius = 5.0f;

    [SerializeField]
    private float limitRadius = 15.0f;

    [SerializeField]
    private float spawnInterval = 10.0f;

    private float _timer;

    private void Start()
    {
        _timer = 0.0f;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, limitRadius);
    }

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _timer = spawnInterval;
            var cols = Physics.OverlapSphere(transform.position, limitRadius);
            if (spawnCount >= cols.Where(x => x.gameObject.name.StartsWith(prefab.name)).Count())
            {
                var newObj = GameObject.Instantiate(prefab);
                newObj.name = prefab.name + "_" + Guid.NewGuid().ToString();
                newObj.transform.position = MathUtil.RandomSphere(radius) + transform.position;
                newObj.transform.rotation = MathUtil.RandomYRot();
            }
        }
    }
}
