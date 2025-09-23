using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemGimmick : MonoBehaviour, IInteractable
{
    // 採取すると一定時間消える部分
    [SerializeField]
    private GameObject _CollectHead;
    [SerializeField]
    private float _RecoveryInteraval;
    [SerializeField]
    private ItemDataBaseV0 _Item;
    [SerializeField]
    private int _Count;

    private bool _IsCollected = false;
    private float _Timer = 0;

    public void Interact(InteractData data)
    {
        PlayerControllerVer0 player = data.Sender;
        if (player)
        {
            EventDebugger.Current.AppendEventDebug("CollectItemGimmick!!", 1);
            Collect(player);
        }
    }

    public virtual void Collect(PlayerControllerVer0 player)
    {
        if (_IsCollected)
        {
            return;
        }

        player.Inventory.AddItem(new ItemData(_Item), _Count);

        SetActiveHead(false);

        // 後々エフェクトの処理とかをここに

        _IsCollected = true;
        StartCoroutine(nameof(Recovery)); // 一定時間でリカバリされるコルーチンを呼ぶ
    }

    private void SetActiveHead(bool v)
    {
        if (_CollectHead != null)
        {
            _CollectHead.SetActive(v);
        }
    }

    private IEnumerator Recovery()
    {
        EventDebugger.Current.AppendEventDebug("Start GimmickWaitProcess!!!", 1);
        yield return new WaitForSeconds(_RecoveryInteraval);
        _IsCollected = false;
        SetActiveHead(true);
        yield return null;
    }
}
