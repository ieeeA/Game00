using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの操作状態など権限が強めのものを管理するクラス
/// 操作できなくなったなどのバグが起きたらとりあえずこことかにブレークを挟んでチェックする
/// </summary>
public class PlayerSystem : MonoBehaviour
{
    private static PlayerSystem _Current;
    public static PlayerSystem Current => _Current;

    public bool IsLocked => Cursor.lockState != CursorLockMode.Locked;
    public bool IsCameraLocked => IsLocked || IsCameraLockedSelf;
    public bool IsMoveLocked => IsLocked || IsMoveLockedSelf;
    public bool IsCameraLockedSelf { get; set; }
    public bool IsMoveLockedSelf { get; set; }

    private void Awake()
    {
        _Current = _Current ?? this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 移動（TODO）とカーソル操作をLockしたりしなかったりする。
    /// </summary>
    /// <param name="value"></param>
    public void SetLockPlayerControl(bool isLock)
    {
        Cursor.lockState = isLock ? CursorLockMode.Confined : CursorLockMode.Locked;
        // TODO
        // 移動を制限する処理をここに書く
    }
}
