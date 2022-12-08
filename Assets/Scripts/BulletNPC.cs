using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNPC : MonoBehaviour, IInteract
{
    [SerializeField]
    private int bullet;
    [SerializeField]
    private int money;

    public void Interact(GameObject target)
    {
        //var fc = target.GetComponent<FireController>();
        var im = target.GetComponent<ItemManager>();
        if (im == null) return;
        //if (fc == null) return;

        if (!im.TryToRemoveMoney(money))
        {
            EventDebugger.Current.AppendEventDebug("[Trade]Fail: No money");
            return;
        }

        try
        {
            EventDebugger.Current.AppendEventDebug("[Trade]Add Bullet");
            //fc.AddBullet(bullet);
        }
        catch(Exception e)
        {
            EventDebugger.Current.AppendEventDebug("[Trade]Fail: Something wrong");
            im.AddMoney(money);   
        }

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
