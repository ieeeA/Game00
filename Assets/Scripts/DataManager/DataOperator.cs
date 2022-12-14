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
            var modifier = new ParameterModifier(ParameterType.MoveOwnerScale, -1.0f);
            tem.ApplyParameterModifier(
                _KnockoutTime,
                modifier,
                "Knockout");
        }
    }

    /// <summary>
    /// TimerEffectManagerやStateMachineを参照して、_NoFixedSuffixのついたEffectをキャンセルして
    /// もとの状態に戻す。
    /// </summary>
    public void CancelAction()
    {

    }

    public void DropItem()
    {
        var obj = Instantiate(_DropItemPrefab);
        obj.transform.position = transform.position + _DropOffset;
    }
}
