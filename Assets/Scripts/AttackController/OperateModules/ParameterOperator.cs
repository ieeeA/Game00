using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IStatusOperator
{
    void SetStatusDamage(int damage, object sender);
}

/// <summary>
/// OperatorÇÕï ÇÃUnityEventÇ∆Ç©Ç…Ç≠Ç¡Ç¬ÇØÇƒåƒÇ—èoÇ∑Ç‡ÇÃ
/// AreaEffectorÇ∆Ç©
/// </summary>
public class ParameterOperator : MonoBehaviour
{
    [SerializeField]
    public float _Value;
    [SerializeField]
    public float _Time;
    [SerializeField]
    public string[] TargetTags;

    public void SetSlowParameter(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            var tem = col.GetComponent<TimerEffectManager>();
            if (tem != null)
            {
                var bm = tem.GetComponent<BasicMovement>();
                var eff = new TimerEffect("Slow")
                {
                    _ApplyMode = TimerEffect.ApplyMode.Overwrite,
                    _IsDistinct = false,
                    _LifeTimer = _Time,
                    _Owner = col.gameObject,
                    _OnStart = null, // startÇÕÇ±Ç±Ç≈Ç‚ÇÈÇÃÇ≈Ç¢ÇÁÇ»Ç¢
                    _Context = bm,
                    _OnEnd = (target, owner, c) =>
                    {
                        bm.ResetCrowdControl();
                        Debug.Log("[ParameterOperator] stop slow");
                    },
                };
                Debug.Log("[ParameterOperator] slow");
                tem.Apply(eff);
                bm.OwnerScale = _Value;
            }
        }
    }

    public void AddDamage(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            var status = col.GetComponent(typeof(IStatusManager)) as IStatusManager;
            status.ChangeHP(new HPChangeInfo() {
                ModifyValue = (int)_Value,
                Sender = gameObject
            });
            Debug.Log("[ParameterOperator] damage!");
        }
    }
}
