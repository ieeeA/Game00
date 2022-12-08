using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlowModule : FireTypeModule
{
    public override FireModuleType Type => FireModuleType.Blow;

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
            _LifeTimer = ChargeTime,
            _IsIterative = true,
            _Interval = ChargeEffInterval,
            _Owner = owner,
            _OnStart = null, // startはここでやるのでいらない
            _OnInterval = (owner, target, c) =>
            {
                // チャージエフェクトを出す
                Debug.Log("Charging!");
                var eff = VFXManager.Current.Instantiate(ChargeEffId);
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

    // 発射中も動けないので
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var blow = ProjectileManager.Current.Instantiate<BasicPooledBehavior>(ProjectileId);
        blow.transform.position = owner.transform.position;
        blow.transform.rotation = owner.transform.rotation;

        var t = CameraController.PlayerCameraCurrent.transform.forward;
        t.y = 0;
        t.Normalize();
        blow.transform.LookAt(t + blow.transform.position);

        // TODO
        // 効果時間分移動をロックする。その後解除する。
        _locker?.SetLockPlayerControl(false);
    }
}
