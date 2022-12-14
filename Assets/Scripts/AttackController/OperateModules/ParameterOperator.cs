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
                // OwnerScaleÇ0Ç…Ç∑ÇÈ
                var modifier = new ParameterModifier(ParameterType.MoveOwnerScale, -1.0f);
                tem.ApplyParameterModifier(
                    _Time,
                    modifier,
                    "SlowParameter");
            }
        }
    }

    public void SetExternalForce(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            // àÍèuÇæÇØÇ†ÇΩÇ¡ÇΩÇ∆Ç´Ç…óÕÇâ¡Ç¶ÇÈTimerEffectÇÇ¬ÇØÇÈÅB
            var tem = col.GetComponent<TimerEffectManager>();
            if (tem != null)
            {
                var force = _Value * transform.forward;
                var modifierX = new ParameterModifier(ParameterType.MoveExternalForceX, force.x);
                var modifierY = new ParameterModifier(ParameterType.MoveExternalForceY, force.y);
                var modifierZ = new ParameterModifier(ParameterType.MoveExternalForceZ, force.z);
                var modifierGroup = new ParameterModifierGroup(new[] { modifierX, modifierY, modifierZ });

                Debug.Log("[ParameterOperator] externalforce");
                tem.ApplyParameterModifier(
                    _Time,
                    modifierGroup,
                    "SetExternalForce");
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

    public void AddResistanceDamage(Collider col)
    {
        if (TargetTags.Contains(col.tag))
        {
            var status = col.GetComponent(typeof(IStatusManager)) as IStatusManager;
            status.ChangeResistance(new ResistanceChangeInfo()
            {
                ModifyValue = (int)_Value,
                Sender = gameObject
            });
            Debug.Log("[ParameterOperator] resistance damage!");
        }
    }
}
