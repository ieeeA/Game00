using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParameterBundleV0))]
public class StatusManagerV0 : MonoBehaviour, IStatusManager
{
    [SerializeField]
    public UnityEvent _OnDead;
    [SerializeField]
    public UnityEvent _OnKnockout;
    [SerializeField]
    public UnityEvent<HPChangeInfo> _OnChangeHP; // エフェクトとか出す
    [SerializeField]
    public UnityEvent<ResistanceChangeInfo> _OnChangeResistance; // エフェクトとか出す

    [SerializeField]
    public bool _IsPlayerDebug = false;

    public int MaxHP => _Param.GetParamter(ParameterType.MaxHP);
    public int MaxResistance => _Param.GetParamter(ParameterType.MaxResistance);
    public int HP
    {
        get => _HP;
        private set
        {
            _HP = value;
            OnUpdatePresentation();
        }
    }

    public int Resistance
    {
        get => _Resistance;
        private set
        {
            _Resistance = value;
            OnUpdatePresentation();
        }
    }

    private int _HP;
    private int _Resistance;
    private TextHandle _DebugTextHandle = null;
    private ParameterBundleV0 _Param;

    public void Start()
    {
        _Param = GetComponent<ParameterBundleV0>();
        if (_IsPlayerDebug)
        {
            _DebugTextHandle = StandardTextPlane.Current.CreateTextHandle(-1);
        }
        ResetStatus();
    }

    public void ResetStatus()
    {
        HP = MaxHP;
        Resistance = MaxResistance;
    }

    private void OnUpdatePresentation()
    {
        if (_DebugTextHandle != null)
        {
            _DebugTextHandle.Text =
            $"HP: {_HP}/{MaxHP}" + Environment.NewLine +
            $"Resistance: {_Resistance}/{MaxResistance}";
        }
    }

    public void ChangeHP(HPChangeInfo changeInfo)
    {
        if (HP < 0)
        {
            Debug.Log("AlreadDead");
            return;
        }

        HP = Math.Clamp(HP + changeInfo.ModifyValue, -100, MaxHP);
        _OnChangeHP?.Invoke(changeInfo);
        if (HP < 0)
        {
            _OnDead?.Invoke();
        }
    }

    public void ChangeResistance(ResistanceChangeInfo changeInfo)
    {
        Resistance = Math.Clamp(Resistance + changeInfo.ModifyValue, -100, MaxResistance);
        _OnChangeResistance?.Invoke(changeInfo);
        if (Resistance < 0)
        {
            _OnKnockout?.Invoke();
        }
    }
}
