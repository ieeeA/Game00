using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ParameterType
{
    MaxHP, // �ő�HP
    MaxResistance, // �ő�Resistance
    AutoRegenerate, // �����񕜗�(1sec)
    MineHervestRate, // �z�Ή������

    Attack, // �U����
    AoEScale, // AoE�̃X�P�[��
    CastSpeed, // �X�y�������X�s�[�h
    CoolDownRecovery, // �N�[���_�E����

    MoveHoriSpeedScale, // �����̈ӎu�œ����邱�Ƃ��ł���ő呬�x�X�P�[��
    MoveOwnerScale, // �ړ��̓X�P�[���i�ړ����x�ቺ�Ƃ��Ɏg���j
    // TODO: ����Parameter�����n���FloatIntVector���܂Ƃ߂Ĉ�����悤��BitValue�݂����\���̂�p�ӂ�����������
    MoveExternalForceX, // �O��X
    MoveExternalForceY, // �O��Y
    MoveExternalForceZ, // �O��Z

    MaxParameterType, // �ő�^�C�v
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
    /// float���w��ł���悤�ɂȂ��Ă��邪ParamterBundleV0�̓s���㊮�S�ɂ��̒l���w�肳���킯�ł͂Ȃ�
    /// �i����\��1-0��100�����ɂȂ�j
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
    [SerializeField, Tooltip("�����_�t���ɂȂ���100�{���Đݒ肵�Ă��������B")]
    public int _Value;
}

/// <summary>
/// �L�����N�^�[�̑����p�����[�^�Ǘ��N���X
/// </summary>
public class ParameterBundleV0 : MonoBehaviour
{
    private List<ParameterModifier> _Modifiers = new List<ParameterModifier>();
    private List<ParameterModifierGroup> _ModifierGroup = new List<ParameterModifierGroup>();
    private Dictionary<ParameterType, int> _Parameter = new Dictionary<ParameterType, int>();

    public static readonly int FloatDivider = 100;

    // �����p�����[�^�w��i���Z�w�肷����́j
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

    // �ݒ肵�Y��Ȃ��悤�A��΂��̒l�ƌ��܂��Ă���҂͂����Őݒ肷��
    // OwnerScale�Ƃ�
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
    /// 0�ȉ��̒l�ɂȂ��Ă����ꍇ0��Ԃ�
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

    // WANT TODO: ���ꑽ�����ɓ��ɏd���Ȃ��Ă���̂ŁA�������������Ƃ��ɍœK����������
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

    // Recalculate�p
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