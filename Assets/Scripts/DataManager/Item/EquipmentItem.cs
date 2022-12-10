using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Item/EquipmentItem")]
public class EquipmentItem : ItemDataBaseV0
{
    [SerializeField]
    public EquipmentDataV0 _EquipmentData;

    public override List<string> GetOptions(GameObject user)
    {
        var list = base.GetOptions(user);
        list.Add("Equip");
        list.Add("TakeOff");
        return list;
    }

    public override void Execute(string option, GameObject user, object arg = null)
    {
        var eqMgr = user.GetComponent<EquipmentManagerV0>();
        if (eqMgr == null)
        {
            Debug.Log("userゲームオブジェクトがEquipmentManagerを持っていません。");
            return;
        }
        switch (option)
        {
            case "Equip":
                eqMgr.Equip(_EquipmentData);
                break;
            case "TakeOff":
                var currentData = eqMgr.GetEquipmentData(_EquipmentData._AttachableType);
                if (currentData != null && currentData.name == _EquipmentData.name)
                {
                    eqMgr.TakeOff(_EquipmentData._AttachableType);
                }
                else
                {
                    Debug.Log("対象のアイテムを装備していません。");
                }
                break;
            default:
                break;
        }
        base.Execute(option, user, arg);
    }
}
