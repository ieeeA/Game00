using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ParameterType
{
    Attack = 0, // 攻撃力
    MaxHP, // 最大HP
    AutoRegenerate, // 自動回復量(1sec)
    SlowPower, // 速度低下効果
    CastSpeed, // スペル発動スピード
    CoolDownRecovery, // クールダウン回復
    MineHervestRate, // 鉱石回収効率
    AoEScale, // AoEのスケール
    MaxParameterType, // 最大タイプ
}

public class ParameterModifier
{
    public ParameterType Type { get; private set; }
    public int Value { get; private set; }
    public string Tag { get; private set; }

    public ParameterModifier(ParameterType type, int value, string tag = "")
    {
        Type = type;
        Value = value;
        Tag = tag;
    }

    public void Apply(Dictionary<ParameterType, int> parameter)
    {
        parameter[Type] += Value;
    }
}

public class ParameterModifierGroup
{
    public string Tag { get; private set; }

    private List<ParameterModifier> _Modifiers;
    public ParameterModifierGroup(IEnumerable<ParameterModifier> mds, string tag = "")
    {
        Tag = tag;
        _Modifiers = mds.ToList(); 
    }

    public void Apply(Dictionary<ParameterType, int> parameter)
    {
        foreach(var m in _Modifiers)
        {
            m.Apply(parameter);
        }
    }
}

/// <summary>
/// キャラクターの総合パラメータ管理クラス
/// </summary>
public class ParameterBundleV0 : MonoBehaviour
{
    private List<ParameterModifier> _Modifiers = new List<ParameterModifier>();
    private List<ParameterModifierGroup> _ModifierGroup = new List<ParameterModifierGroup>();
    private Dictionary<ParameterType, int> _Parameter = new Dictionary<ParameterType, int>();

    private const int FloatDivider = 100;

    // 初期パラメータ指定
    [SerializeField]
    public ParameterModiferData[] _InitialParameter;

    private void Awake()
    {
        for (ParameterType i = 0; i < ParameterType.MaxParameterType; i++)
        {
            _Parameter.Add(i, 0);
        }
    }

    public int GetParamter(ParameterType type)
    {
        if (_Parameter.ContainsKey(type))
        {
            return _Parameter[type];
        }
        return 0;
    }

    public float GetParamterFloat(ParameterType type)
    {
        if (_Parameter.ContainsKey(type))
        {
            return (float)_Parameter[type] / FloatDivider;
        }
        return 0.0f;
    }

    // WANT TODO: これ多分日に日に重くなってくるので、頃合いがいいときに最適化をかける
    public void Recalculate()
    {
        FillZero(); 
        foreach (var m in _Modifiers)
        {
            m.Apply(_Parameter);
        }

        foreach(var mg in _ModifierGroup)
        {
            mg.Apply(_Parameter);
        }
    }

    public void Register(ParameterModifier modifier, bool recalculate = true)
    {
        _Modifiers.Add(modifier);
        if (recalculate)
        {
            Recalculate();
        }
    }
    
    public void UnRegister(ParameterModifier modifier, bool recalculate = true)
    {
        _Modifiers.Remove(modifier);
        if (recalculate)
        {
            Recalculate();
        }
    }

    public void Register(ParameterModifierGroup modifier, bool recalculate = true)
    {
        _ModifierGroup.Add(modifier);
        if (recalculate)
        {
            Recalculate();
        }
    }

    public void UnRegister(ParameterModifierGroup modifier, bool recalculate = true)
    {
        _ModifierGroup.Remove(modifier);
        if (recalculate)
        {
            Recalculate();
        }
    }

    // Recalculate用
    private void FillZero()
    {
        for (ParameterType i = 0; i < ParameterType.MaxParameterType; i++)
        {
            _Parameter[i] = 0;
        }
    }
    private void ApplyInitialParameters()
    {
        foreach(var p in _InitialParameter)
        {
            var m = p.CreateModifier(tag = "initial");
            Register(m);
        }
    }
}