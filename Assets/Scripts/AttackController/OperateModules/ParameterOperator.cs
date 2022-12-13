using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IStatusOperator
{
    void SetStatusDamage(int damage, object sender);
}

/// <summary>
/// Operator‚Í•Ê‚ÌUnityEvent‚Æ‚©‚É‚­‚Á‚Â‚¯‚ÄŒÄ‚Ño‚·‚à‚Ì
/// AreaEffector‚Æ‚©
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
                    _OnStart = null, // start‚Í‚±‚±‚Å‚â‚é‚Ì‚Å‚¢‚ç‚È‚¢
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

    public void SetExternalForce(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            // ˆêu‚¾‚¯‚ ‚½‚Á‚½‚Æ‚«‚É—Í‚ğ‰Á‚¦‚éTimerEffect‚ğ‚Â‚¯‚éB
            var tem = col.GetComponent<TimerEffectManager>();
            if (tem != null)
            {
                var bm = tem.GetComponent<BasicMovement>();
                var eff = new TimerEffect("ExternalForce")
                {
                    _ApplyMode = TimerEffect.ApplyMode.Overwrite,
                    _IsDistinct = false,
                    _LifeTimer = _Time,
                    _Owner = col.gameObject,
                    _OnStart = null, // start‚Í‚±‚±‚Å‚â‚é‚Ì‚Å‚¢‚ç‚È‚¢
                    _Context = bm,
                    _OnEnd = (target, owner, c) =>
                    {
                        bm.ResetCrowdControl();
                        Debug.Log("[ParameterOperator] stop externalforce");
                    },
                };
                Debug.Log("[ParameterOperator] externalforce");
                tem.Apply(eff);
                bm.ExternalForce = _Value * transform.forward;
            }
        }
    }

    public void AddDamage(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            var status = col.GetComponent(typeof(IStatusManager)) as IStatusManager;
            status.ChangeHP(new HPChangeInfo()
            {
                ModifyValue = (int)_Value,
                Sender = gameObject
            });
            Debug.Log("[ParameterOperator] damage!");
        }
    }
}
