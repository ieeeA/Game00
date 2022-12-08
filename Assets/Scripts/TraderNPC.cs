using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderNPC : MonoBehaviour, IInteract
{
    [SerializeField]
    private int AddMoney = 10;

    public void Interact(GameObject target)
    {
        var mgr = target.GetComponent<ItemManager>();
        if (mgr == null) return;

        // �Ƃ肠�����v���g�^�C�v�Ȃ̂ŁAItem�𔄂邾���ɂ���B��ЂƂ����悤�ɂ���

        if (mgr.Containers.Count == 0)
        {
            EventDebugger.Current.AppendEventDebug("[Trade]Fail:No item");
            return;
        }

        if (mgr.TryRemoveItem(0, 1) == true)
        {
            mgr.AddMoney(AddMoney);
            EventDebugger.Current.AppendEventDebug($"[Trade]Get: {AddMoney} money");
        }
    }
}
