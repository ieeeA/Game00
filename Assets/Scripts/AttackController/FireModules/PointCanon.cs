using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCanon : FireTypeModule
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.ShootBullet;

    // TODO:
    // そのうちNPCとPlayer両用にするので、なんかのInterfaceに_lockerを変える
    private PlayerSystem _locker;

    public override void OnFire(GameObject owner)
    {
        var mgr = owner.GetComponent<TimerEffectManager>();

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
        _locker?.SetLockPlayerControl(true);
    }

    private Vector3 CalculateDirection()
    {
        var t = CameraController.PlayerCameraCurrent.transform.forward;
        t += _CurrentState._Offset0;
        return t.normalized;
    }

    // 発射中も動けないので
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var bullet = ProjectileManager.Current.Instantiate<BasicPooledBehavior>(_CurrentState._ProjectileId);

        if (bullet == null)
        {
            Debug.Log("[ShootBulletModule] bullet のインスタンスが足りません。");
            _locker?.SetLockPlayerControl(false);
            return;
        }

        bullet.transform.position = owner.transform.position;
        bullet.transform.rotation = owner.transform.rotation;

        var dir = CalculateDirection();
        bullet.transform.LookAt(dir + bullet.transform.position);
        bullet.GetComponent<Projectile>().Configure(_CurrentState._Speed0, dir);

        // TODO
        // 効果時間分移動をロックする。その後解除する。
        _locker?.SetLockPlayerControl(false);
    }
}
