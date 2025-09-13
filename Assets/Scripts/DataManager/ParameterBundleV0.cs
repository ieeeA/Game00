using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ParameterType
{
    MaxHP, // 最大HP
    MaxResistance, // 最大Resistance
    AutoRegenerate, // 自動回復量(1sec)
    MineHervestRate, // 鉱石回収効率

    Attack, // 攻撃力
    AoEScale, // AoEのスケール
    CastSpeed, // スペル発動スピード
    CoolDownRecovery, // クールダウン回復

    MoveHoriSpeedScale, // 自分の意志で動けることができる最大速度スケール
    MoveOwnerScale, // 移動力スケール（移動速度低下とかに使う）
    // TODO: このParameter処理系絶対FloatIntVectorかまとめて扱えるようにBitValueみたい構造体を用意した方がいい
    MoveExternalForceX, // 外力X
    MoveExternalForceY, // 外力Y
    MoveExternalForceZ, // 外力Z

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

    /// <summary>
    /// floatを指定できるようになっているがParamterBundleV0の都合上完全にその値が指定されるわけではない
    /// （分解能が1-0で100分割になる）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <param name="tag"></param>
    public ParameterModifier(ParameterType type, float value, string tag = "")
    {
        Type = type;
        Value = (int)(value * ParameterBundleV0.FloatDivider);
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

[Serializable]
public class ParameterBundleV0InitialParamter
{
    [SerializeField]
    public ParameterType _ParameterType;
    [SerializeField, Tooltip("小数点付きになるやつは100倍して設定してください。")]
    public int _Value;
}

/// <summary>
/// キャラクターの総合パラメータ管理クラス
/// </summary>
public class ParameterBundleV0 : MonoBehaviour
{
    private List<ParameterModifier> _Modifiers = new List<ParameterModifier>();
    private List<ParameterModifierGroup> _ModifierGroup = new List<ParameterModifierGroup>();
    private Dictionary<ParameterType, int> _Parameter = new Dictionary<ParameterType, int>();

    public static readonly int FloatDivider = 100;

    // 初期パラメータ指定（加算指定するもの）
    [SerializeField]
    public ParameterBundleV0InitialParamter[] _InitialParameter;

    private void Update()
    {
        
    }


    private void Awake()
    {
        for (ParameterType i = 0; i < ParameterType.MaxParameterType; i++)
        {
            _Parameter.Add(i, GetDefaultParameter(i));
        }
        ApplyInitialParameters();
    }

    // 設定し忘れないよう、絶対その値と決まっている者はここで設定する
    // OwnerScaleとか
    private int GetDefaultParameter(ParameterType type)
    {
        switch (type)
        {
            case ParameterType.MoveOwnerScale:
                return 100;
            case ParameterType.MaxHP:
                return 100;
            case ParameterType.MaxResistance:
                return 100;
            case ParameterType.MoveHoriSpeedScale:
                return 100;
            default:
                return 0;
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

    /// <summary>
    /// 0以下の値になっていた場合0を返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetParamterFloatOrZero(ParameterType type)
    {
        if (_Parameter.ContainsKey(type) && _Parameter[type] > 0)
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
            _Parameter[i] = GetDefaultParameter(i);
        }
    }
    private void ApplyInitialParameters()
    {
        foreach(var p in _InitialParameter)
        {
            var m = new ParameterModifier(p._ParameterType, p._Value, tag = "initial");
            Register(m);
        }
    }
}