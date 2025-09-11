using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TraderNPC : MonoBehaviour, IInteract
{
    [SerializeField]
    private int AddMoney = 10;

    public void Interact(GameObject target)
    {
        var mgr = target.GetComponent<PlayerControllerVer0>();
        if (mgr == null) return;

        // とりあえずプロトタイプなので、Itemを売るだけにする。一つひとつ売れるようにする

        if (mgr.Inventory.Containers.Count == 0)
        {
            EventDebugger.Current.AppendEventDebug("[Trade]Fail:No item");
            return;
        }

        if (mgr.Inventory.TryRemoveItem(0, 1) == true)
        {
            mgr.Inventory.AddMoney(AddMoney);
            EventDebugger.Current.AppendEventDebug($"[Trade]Get: {AddMoney} money");
        }
    }
}
