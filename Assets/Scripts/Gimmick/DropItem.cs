using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField]
    public int Count;

    [SerializeField]
    public ItemDataBaseV0 _Item;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var mgr = other.GetComponent<PlayerControllerVer0>();
        if(mgr != null)
        {
            mgr.Inventory.AddItem(new ItemData(_Item), Count);
            EventDebugger.Current.AppendEventDebug($"[GetItem]{_Item._Name}({Count})");
            GameObject.Destroy(gameObject);
            return;
        }
    }
}
