using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimerEffectUtility
{
    public static void ApplyParameterModifier(
        this TimerEffectManager tem, 
        float timer,
        ParameterModifier mod, 
        string id = "FreeEffect", bool isDestinct = false)
    {
        var param = tem.gameObject.GetComponent<ParameterBundleV0>();
        if (param == null)
        {
            Debug.LogError("ParameterBundleV0‚ª‚ ‚è‚Ü‚¹‚ñB");
            return;
        }

        var eff = new TimerEffect(id)
        {
            _ApplyMode = TimerEffect.ApplyMode.Overwrite,
            _IsDistinct = isDestinct,
            _LifeTimer = timer,
            _Owner = tem.gameObject,
            _OnStart = (owner, target, c) =>
            {
                param.Register(mod);
            },
            _Context = null,
            _OnEnd = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop: {id}");
            }
        };
        Debug.Log($"[ParameterOperator] EffectStart: {id}");
        tem.Apply(eff);
    }

    public static void ApplyParameterModifier(
        this TimerEffectManager tem, 
        float timer,
        ParameterModifierGroup mod,
        string id = "FreeEffect", bool isDestinct = false)
    {
        var param = tem.gameObject.GetComponent<ParameterBundleV0>();
        if (param == null)
        {
            Debug.LogError("ParameterBundleV0‚ª‚ ‚è‚Ü‚¹‚ñB");
            return;
        }

        var eff = new TimerEffect(id)
        {
            _ApplyMode = TimerEffect.ApplyMode.Overwrite,
            _IsDistinct = isDestinct,
            _LifeTimer = timer,
            _Owner = tem.gameObject,
            _OnStart = (owner, target, c) =>
            {
                param.Register(mod);
            },
            _Context = null,
            _OnEnd = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop: {id}");
            }
        };
        Debug.Log($"[ParameterOperator] EffectStart: {id}");
        tem.Apply(eff);
    }

}
