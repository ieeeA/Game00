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
            ApplyMode = TimerEffect.TimerEffectApplyMode.Overwrite,
            EffectType = TimerEffectType.CancelableParameterEffect,
            IsDistinct = isDestinct,
            LifeTime = timer,
            Owner = tem.gameObject,
            OnStart = (owner, target, c) =>
            {
                param.Register(mod);
                Debug.Log($"[ParameterOperator] EffectStart: {id}");
            },
            Context = null,
            OnCancel = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop(Canceled): {id}");
            },
            OnEnd = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop: {id}");
            }
        };
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
            ApplyMode = TimerEffect.TimerEffectApplyMode.Overwrite,
            EffectType = TimerEffectType.CancelableParameterEffect,
            IsDistinct = isDestinct,
            LifeTime = timer,
            Owner = tem.gameObject,
            OnStart = (owner, target, c) =>
            {
                param.Register(mod);
                Debug.Log($"[ParameterOperator] EffectStart: {id}");
            },
            Context = null,
            OnCancel = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop(Canceled): {id}");
            },
            OnEnd = (owner, target, c) =>
            {
                param.UnRegister(mod);
                Debug.Log($"[ParameterOperator] EffectStop: {id}");
            }
        };
        tem.Apply(eff);
    }
}

