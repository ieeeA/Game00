using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField]
    private string ItemName;

    [SerializeField]
    private int Count;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var mgr = other.GetComponent<ItemManager>();
        if(mgr != null)
        {
            mgr.AddItem(new ItemData() { _Name = ItemName }, Count);
            EventDebugger.Current.AppendEventDebug($"[GetItem]{ItemName}({Count})");
            GameObject.Destroy(gameObject);
            return;
        }
    }
}
