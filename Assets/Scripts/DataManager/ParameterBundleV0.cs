using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ParameterType
{
    Attack = 0, // �U����
    MaxHP, // �ő�HP
    AutoRegenerate, // �����񕜗�(1sec)
    SlowPower, // ���x�ቺ����
    CastSpeed, // �X�y�������X�s�[�h
    CoolDownRecovery, // �N�[���_�E����
    MineHervestRate, // �z�Ή������
    AoEScale, // AoE�̃X�P�[��
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
/// �L�����N�^�[�̑����p�����[�^�Ǘ��N���X
/// </summary>
public class ParameterBundleV0 : MonoBehaviour
{
    private List<ParameterModifier> _Modifiers = new List<ParameterModifier>();
    private List<ParameterModifierGroup> _ModifierGroup = new List<ParameterModifierGroup>();
    private Dictionary<ParameterType, int> _Parameter = new Dictionary<ParameterType, int>();

    private const int FloatDivider = 100;

    // �����p�����[�^�w��
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