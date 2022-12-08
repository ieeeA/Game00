using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class SampleEnemyController : MonoBehaviour
{
    //private FireController _fireCon;

    [SerializeField]
    private float fireRate = 2.0f;

    [SerializeField]
    private float shootRadius = 10.0f;

    private float _timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        //_fireCon = GetComponent<FireController>();
        //_fireCon.IsInfinityBullet = true;
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            _timer = fireRate;

            var pl = FieldUtil.Search(transform.position, shootRadius)
                .Where(x => (LayerMask.NameToLayer("Player") & x.layer) != 0)
                .FirstOrDefault();

            if (pl != null)
            {
                var dir = (pl.transform.position - transform.position).normalized;
                //_fireCon.Fire(transform.position, dir);
            }
        }
    }
}
