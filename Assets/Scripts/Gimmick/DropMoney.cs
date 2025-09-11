using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropMoney : MonoBehaviour
{
    [SerializeField]
    public int _Count;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var mgr = other.GetComponent<PlayerControllerVer0>();
        if (mgr != null)
        {
            mgr.Inventory.AddMoney(_Count);
            EventDebugger.Current.AppendEventDebug($"[GetMoney](Count: {_Count})");
            GameObject.Destroy(gameObject);
            return;
        }
    }
}
