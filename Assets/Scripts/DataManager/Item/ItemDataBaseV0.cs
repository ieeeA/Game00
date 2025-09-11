using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Item/PlainItem")]
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
        if (option == "Throw")
        {
            int idx = (int)arg;
            var itemMgr = user.GetComponent<PlayerControllerVer0>();
            if (itemMgr.Inventory.Containers.Count <= idx)
            {
                Debug.Log("[ItemDataBaseV0] ŽÌ‚Ä‚æ‚¤‚Æ‚µ‚Ä‚¢‚éitemIdx‚ª•s³‚Å‚·");
                return;
            }
            var con = itemMgr.Inventory.Containers[idx];
            itemMgr.Inventory.TryRemoveItem(idx, con.Count);
        }
    }
}
