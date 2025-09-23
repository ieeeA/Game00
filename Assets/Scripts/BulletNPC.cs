using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletNPC : MonoBehaviour
{
    [SerializeField]
    private int bullet;
    [SerializeField]
    private int money;

    public void Interact(GameObject target)
    {
        //var fc = target.GetComponent<FireController>();
        var im = target.GetComponent<PlayerControllerVer0>();
        if (im == null) return;
        //if (fc == null) return;

        if (!im.Inventory.TryToRemoveMoney(money))
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
            im.Inventory.AddMoney(money);   
        }
    }
}
