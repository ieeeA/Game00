using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:
/// これ絶対FireTypeModuleごとに生成するようにした方がいい...。
/// バトルの方向性が固まったらリファクタをはさもう
/// </summary>

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ModuleState")]
// モジュールの状態と
public class ModuleState : ScriptableObject
{
    public float _Time;

    [SerializeField]
    public float _Interval; // クールダウンタイム
    [SerializeField]
    public FireModuleType _Type; // 指定FireModuleType
    [SerializeField]
    public string _ProjectileId; // 攻撃オブジェクトを取り出すためのID
    [SerializeField]
    public float _MoveSpeedDecreaseRatio; // 速度減少率
    [SerializeField]
    public float _ChargeTime; // 発射までの時間
    [SerializeField]
    public float _ChargeEffInterval; // チャージエフェクトのエミット感覚
    [SerializeField]
    public string _ChargeEffId; // チャージエフェクトのID
    [SerializeField]
    public string[] _LayerNameList0; // Module内で即座に当たり判定をするためのレイヤー名リスト
    [SerializeField]
    public Vector3 _Offset0; // 何らかのオフセット指定用
    [SerializeField]
    public float _Range0; // 何らかの範囲指定用
    [SerializeField]
    public float _Speed0; // 何らかのスピード指定

    public virtual bool CanFire()
    {
        return _Time <= 0.0f;
    }

    public virtual void Fire()
    {
        _Time = _Interval;
    }

    public virtual void Update()
    {
        if (_Time > 0.0f)
        {
            _Time -= Time.deltaTime;
        }
    }
}
