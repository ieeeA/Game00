using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

[Serializable]

public class CharacterStatus
{
    public enum HealthState
    {
        Living,
        KnocuOut,
        Daed,
    }

    private int _hp;
    private int _mp;
    private int _resistance;

    [SerializeField]
    private int _hpMax;

    [SerializeField]
    private int _mpMax;

    [SerializeField]
    private int _resistanceMax;

    [SerializeField]
    private int _attackPower;

    [SerializeField]
    private int _defensePower;

    private int _Speed;

    [SerializeField]
    private int _initialBaseSpeed;

    [SerializeField]
    private bool _isDebugging;

    #region プロパティ


    public int Hp { get => _hp; set => _hp = value; }
    public int Mp { get => _mp; set => _mp = value; }
    public int HpMax { get => _hpMax; set => _hpMax = value; }
    public int MpMax { get => _mpMax; set => _mpMax = value; }
    public int Resistance { get => _resistance; set => _resistance = value; }
    public int ResistanceMax { get => _resistanceMax; set => _resistanceMax = value; }
    public int AttackPower { get => _attackPower; set => _attackPower = value; }
    public int DefensePower { get => _defensePower; set => _defensePower = value; }
    public int Speed { get => _Speed; set => _Speed = value; }
    public int InitialBaseSpeed { get => _initialBaseSpeed; set => _initialBaseSpeed = value; }

    public HealthState State { get; set; }

    public bool IsDebugging { get => _isDebugging; set => _isDebugging = value; }


    #endregion


    public void ResetStatus()
    {
        Hp = HpMax;
        Mp = MpMax;
        Speed = InitialBaseSpeed;
    }

    public void UpdateStatus()
    {
        if (Hp <= 0)
        {
            this.State = HealthState.Daed;
        }
    }

    private void OnUpdatePresentation()
    {
        if (IsDebugging)
        {
            string debugText =
                $"HP: {Hp}/{HpMax}" + Environment.NewLine +
                $"MP: {Mp}/{MpMax}" + Environment.NewLine +
                $"BaseSpeed: {Speed}/{InitialBaseSpeed}" + Environment.NewLine +
                $"Resistance: {Resistance}/{ResistanceMax}";
            UnityEngine.Debug.Log(debugText);
        }
    }



    /// <summary>
    /// 何かしらのダメージを受けた時にHPを減らす処理
    /// </summary>
    /// <param name="damegedHP">ダメージ数値</param>
    public void Damaged(int damegedHP)
    {
        this.Hp -= damegedHP;

        OnUpdatePresentation();
    }

}
