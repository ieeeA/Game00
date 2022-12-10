using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/Item/HPRecoveryItem")]
public class HPRecoveryItem : ItemDataBaseV0
{
    [SerializeField]
    private int _RecoveryValue;

    // Start is called before the first frame update
    public override List<string> GetOptions(GameObject user)
    {
        var list = base.GetOptions(user);
        list.Add("Use");
        return list;
    }

    public override void Execute(string option, GameObject user, object arg = null)
    {
        var status = user.GetComponent<StatusManagerV0>();
        if (status == null)
        {
            Debug.Log("userゲームオブジェクトがEquipmentManagerを持っていません。");
            return;
        }
        switch (option)
        {
            case "Use":
                status.ChangeHP(new HPChangeInfo()
                {
                   ModifyValue = _RecoveryValue,
                   Sender = user
                });
                break;
            default:
                break;
        }
        base.Execute(option, user, arg);
    }
}
