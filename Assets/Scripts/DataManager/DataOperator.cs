using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataOperator : MonoBehaviour
{
    [SerializeField]
    private GameObject _DropItemPrefab;
    [SerializeField]
    private Vector3 _DropOffset;
    [SerializeField]
    private float _KnockoutTime;
    [SerializeField]
    public TimerEffectType[] _CancelableTypes;

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 一定時間動けなくする
    /// </summary>
    public void Knockout()
    {
        var tem = gameObject.GetComponent<TimerEffectManager>();
        if (tem != null)
        {
            // OwnerScaleを0にする
            var modifier = new ParameterModifier(ParameterType.MoveOwnerScale, -100);
            tem.ApplyParameterModifier(
                _KnockoutTime,
                modifier,
                "Knockout");
        }
        var status = gameObject.GetComponent(typeof(IStatusManager)) as IStatusManager;
        // KnockoutされたらResitanceは回復しておく
        status.ChangeResistance(new ResistanceChangeInfo() { ModifyValue = 1000, Sender = gameObject });
    }

    /// <summary>
    /// TimerEffectManagerやStateMachineを参照して、_NoFixedSuffixのついたEffectをキャンセルして
    /// もとの状態に戻す。
    /// </summary>
    public void CancelEffect()
    {
        var tem = gameObject.GetComponent<TimerEffectManager>();
        if (tem != null)
        {
            TimerEffectType searchBits = 0;
            foreach(var t in _CancelableTypes)
            {
                searchBits |= t;
            }
            foreach(var eff in tem.SearchEffect(searchBits))
            {
                tem.Remove(eff);
            }
        }
    }

    public void DropItem()
    {
        var obj = Instantiate(_DropItemPrefab);
        obj.transform.position = transform.position + _DropOffset;
    }
}
