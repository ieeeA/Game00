using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireModuleType
{
    Blow, // 目の前に効果フィールドを出す
    EffectToGround, // プレイヤーの目の前の地面に効果フィールドを出す
    Missile,
    ShootBullet,
    PointCanon,
}

/// <summary>
/// 技発動の起点クラス
/// 各技タイプモジュールの制御をする程度
/// </summary>
public class FireControllerV2 : MonoBehaviour
{
    // V2実装用
    // 複数技の検証を行うので、技切り替え機能など検証用機能を実装する
    // 1, 2, 3...で切り替え
    // 右クリックで発射
    [SerializeField]
    public int _Current = 0;
    [SerializeField]
    public List<ModuleState> _ModuleStates = new List<ModuleState>();
    [SerializeField]
    private bool _IsPlayer = false;

    protected Dictionary<FireModuleType, FireTypeModule> _ModuleDict = new Dictionary<FireModuleType, FireTypeModule>();
    private TextHandle _handle;

    public T GetModule<T>(FireModuleType type) where T : FireTypeModule
    {
        if (!_ModuleDict.ContainsKey(type))
        {
            Debug.LogError("実装されていないFireTypeModuleへアクセスしようとしました。");
            return null;
        }
        return _ModuleDict[type] as T;
    }

    public FireTypeModule GetModule(FireModuleType type)
    {
        if (!_ModuleDict.ContainsKey(type))
        {
            Debug.LogError("実装されていないFireTypeModuleへアクセスしようとしました。");
            return null;
        }
        return _ModuleDict[type];
    }

    public void Fire()
    {
        var targetState = _ModuleStates[_Current];
        var module = GetModule(targetState._Type);
        module.Configure(targetState);

        if (targetState.CanFire() && module.CanFire(gameObject))
        {
            targetState.Fire();
            module.OnFire(gameObject);
        }
    }

    private void InitializeModules()
    {
        // =============================================
        // モジュールを実装するたびにここに追加していく
        // =============================================
        _ModuleDict.Add(FireModuleType.Blow, new BlowModule());
        _ModuleDict.Add(FireModuleType.EffectToGround, new EffectToGroundModule());
        _ModuleDict.Add(FireModuleType.ShootBullet, new ShootBulletModule());
    }

    private void Start()
    {
        InitializeModules();

        if (_IsPlayer)
        {
            _handle = StandardTextPlane.Current.CreateTextHandle();
        }
    }

    private void Update()
    {
        foreach (var state in _ModuleStates)
        {
            state.Update();
        }

        if (_IsPlayer)
        {
            // これ一応デバッグ用だから許して
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _Current = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _Current = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _Current = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _Current = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _Current = 4;
            }

            _handle.Text = "[FireControllerV2]" + Environment.NewLine;
            for (int i = 0; i < _ModuleStates.Count; i++)
            {
                var s = i == _Current ? "@" : " ";
                var state = _ModuleStates[i];
                _handle.Text += $"{s}[{i}]{state._Type}({state._Time.ToString("F1")})" + Environment.NewLine;
            }
        }
    }
}

public abstract class FireTypeModule
{
    // ここに各々のプロパティを設定して、CanFireでチェック、Fireで効果発動
    // という感じにする。

    public abstract FireModuleType Type { get; }
    protected ModuleState _CurrentState;

    public virtual void Configure(ModuleState state)
    {
        _CurrentState = state;
    }

    // FireControllerが発射可能な状態か聞くためのもの
    public virtual bool CanFire(GameObject owner)
    {
        return true;
    }

    // トリガーを引いたときの処理
    public virtual void OnFire(GameObject owner)
    {

    }
}
