using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusManagerV0 : MonoBehaviour, IStatusManager
{
    [SerializeField]
    private int _MaxHP;
    [SerializeField]
    private int _MaxRegistance; // ノックアウト状態までの抵抗値

    [SerializeField]
    public UnityEvent _OnDead;
    [SerializeField]
    public UnityEvent<HPChangeInfo> _OnChangeHP; // エフェクトとか出す

    [SerializeField]
    public bool _IsPlayerDebug = false;

    public int MaxHP => _MaxHP;
    public int HP
    {
        get => _HP;
        private set
        {
            _HP = value;
            if (_DebugTextHandle != null)
            {
                _DebugTextHandle.Text = $"HP: {_HP}/{MaxHP}";
            }
        }
    }
    
    private int _HP;
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
}
