using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletItem : MonoBehaviour
{
    [SerializeField]
    private int BulletCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //var fc = other.GetComponent<Collider>().GetComponent<FireController>();
        //if (fc != null)
        //{
        //    if (fc.AddBullet(BulletCount))
        //    {
        //        GameObject.Destroy(gameObject);
        //        EventDebugger.Current.AppendEventDebug("AddBullet");
        //    }
        //}
    }

}
