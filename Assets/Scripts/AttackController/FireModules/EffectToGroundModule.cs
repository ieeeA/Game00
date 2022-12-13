using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectToGroundModule : FireTypeModule
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.EffectToGround;

    // TODO:
    // そのうちNPCとPlayer両用にするので、なんかのInterfaceに_lockerを変える
    private PlayerSystem _locker;
    private Vector3 _placement; // 効果フィールドを配置する場所

    private Vector3 GetRaycastOrigin(GameObject owner)
    {
        var pos = owner.transform.position;
        pos += Quaternion.AngleAxis(owner.transform.rotation.eulerAngles.y, Vector3.up) * _CurrentState._Offset0;
        return pos;
    }

    public override void OnFire(GameObject owner)
    {
        var mgr = owner.GetComponent<TimerEffectManager>();
        if (false == FieldUtil.GetPositionRaycastToTerrain(GetRaycastOrigin(owner), _CurrentState._Range0, out _placement))
        {
            Debug.Log("[EffectToGroundModule] Effectフィールドを配置できません。");
            return;
        }

        // TODO
        // _NoFixed がついてる時は途中で攻撃を受けたらCancelされるようにしておく
        var eff = new TimerEffect("ChargeAndFire_NoFixed")
        {
            _ApplyMode = TimerEffect.ApplyMode.Overwrite,
            _IsDistinct = true,
            _LifeTimer = _CurrentState._ChargeTime,
            _IsIterative = true,
            _Interval = _CurrentState._ChargeEffInterval,
            _Owner = owner,
            _OnStart = null, // startはここでやるのでいらない
            _OnInterval = (owner, target, c) =>
            {
                // チャージエフェクトを出す
                Debug.Log("Charging!");
                var eff = VFXManager.Current.Instantiate(_CurrentState._ChargeEffId);
                eff.transform.position = owner.transform.position;

                var t = CameraController.PlayerCameraCurrent.transform.forward;
                t.y = 0;
                t.Normalize();
                eff.transform.LookAt(t + eff.transform.position);
            },
            _OnEnd = (owner, target, c) =>
            {
                // TODO: 発射処理
                OnFired(owner);
            },
        };
        Debug.Log("StartCharge...");

        mgr.Apply(eff);

        // TODO: 移動をロックする
        _locker = owner.GetComponent<PlayerSystem>();
        if (_locker != null)
        {
            _locker.IsMoveLockedSelf = true;
        }
    }

    // 発射中も動けないので
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");

        var blow = ProjectileManager.Current.Instantiate<BasicPooledBehavior>(_CurrentState._ProjectileId);
        blow.transform.position = _placement;

        // TODO
        // 効果時間分移動をロックする。その後解除する。
        _locker = owner.GetComponent<PlayerSystem>();
        if (_locker != null)
        {
            _locker.IsMoveLockedSelf = false;
        }
    }
}
