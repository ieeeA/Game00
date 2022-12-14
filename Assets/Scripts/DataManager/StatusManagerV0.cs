using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusManagerV0 : MonoBehaviour, IStatusManager
{
    [SerializeField]
    private int _MaxHP;
    [SerializeField]
    private int _MaxResistance;
    [SerializeField]
    private int _MaxRegistance; // ノックアウト状態までの抵抗値

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

    public int MaxHP => _MaxHP;
    public int MaxResistance => _MaxRegistance;
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

    public void Start()
    {
        if (_IsPlayerDebug)
        {
            _DebugTextHandle = StandardTextPlane.Current.CreateTextHandle(-1);
        }
        ResetStatus();
    }

    public void ResetStatus()
    {
        HP = MaxHP;
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

        HP += changeInfo.ModifyValue;
        _OnChangeHP?.Invoke(changeInfo);
        if (HP < 0)
        {
            _OnDead?.Invoke();
        }
    }

    public void ChangeResistance(ResistanceChangeInfo changeInfo)
    {
        if (Resistance < 0)
        {
            Debug.Log("AlreadDead");
            return;
        }

        Resistance += changeInfo.ModifyValue;
        _OnChangeResistance?.Invoke(changeInfo);
        if (Resistance < 0)
        {
            _OnDead?.Invoke();
        }
    }
}
