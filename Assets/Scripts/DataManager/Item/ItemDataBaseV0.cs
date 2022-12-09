using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBaseV0 : ScriptableObject
{
    [SerializeField]
    public string _Name;
    [SerializeField]
    public bool _IsStackable;
    [SerializeField]
    public int _MaxCount;
    [SerializeField]
    public string _Description;

    public virtual List<string> GetOptions(GameObject user)
    {
        return new List<string>() { "Throw" };
    }
    public virtual void Execute(string option, GameObject user, object arg = null)
    {
        int idx = (int)arg;
        var itemMgr = user.GetComponent<ItemManager>();
        
    }
}
