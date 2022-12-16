using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵味方で
public interface IFireModuleUser
{
    Vector3 GetFocusVector();
}

public abstract class FireModuleImplBaseV0 : FireTypeModule
{
    // DONE??:
    // そのうちNPCとPlayer両用にするので、なんかのInterfaceに_lockerを変える
    // と思ったけど、移動制限ならデバフとしてParameterBundleに移動力0のエフェクトを付加すればいいだけだった
    //protected PlayerSystem _Locker;
    
    protected ParameterBundleV0 _Param;
    private ParameterModifier _MoveLockModifier;
    protected IFireModuleUser _User;

    public override void OnFire(GameObject owner)
    {
        var mgr = owner.GetComponent<TimerEffectManager>();
        _Param = owner.GetComponent<ParameterBundleV0>();
        _User = owner.GetComponent(typeof(IFireModuleUser)) as IFireModuleUser;
        //_Locker = owner.GetComponent<PlayerSystem>();

        var eff = new TimerEffect("ChargeAndFire_NoFixed")
        {
            ApplyMode = TimerEffect.TimerEffectApplyMode.Overwrite,
            EffectType = TimerEffectType.EasyCancelableAction,
            IsDistinct = true,
            LifeTime = _CurrentState._ChargeTime,
            IsIterative = true,
            Interval = _CurrentState._ChargeEffInterval,
            Owner = owner,
            OnStart = OnStart, // startはここでやるのでいらない
            OnInterval = null,
            OnCancel = (owner, target, c) =>
            {
                OnFinalize();
            },
            OnEnd = (owner, target, c) =>
            {
                // TODO: 発射処理
                OnFired(owner);
            },
        };
        Debug.Log("StartCharge...");
        mgr.Apply(eff);
    }

    protected virtual void OnStart(GameObject owner, GameObject target, object context)
    {
        // チャージエフェクトを出す
        Debug.Log("Charging!");
        var eff = VFXManager.Current.Instantiate(_CurrentState._ChargeEffId);
        eff.transform.position = owner.transform.position;
        var t = _User.GetFocusVector();
        t.y = 0;
        t.Normalize();
        eff.transform.LookAt(_User.GetFocusVector() + eff.transform.position);

        // 移動のLock処理
        // -10を入れるのは絶対動けないようにするため
        _MoveLockModifier = new ParameterModifier(ParameterType.MoveHoriSpeedScale, -_CurrentState._MoveSpeedDecreaseRatio);
        _Param.Register(_MoveLockModifier);
    }

    protected virtual void OnFinalize()
    {
        _Param.UnRegister(_MoveLockModifier);
    }

    // 発射中も動けないので
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var pooledObj = ProjectileManager.Current.Instantiate<PoolInitializedBehavior>(_CurrentState._ProjectileId);
        if (pooledObj == null)
        {
            Debug.Log("[FireModuleImplBaseV] proj のインスタンスが足りません。");
            OnFinalize();
            return;
        }
        ConfigureProjectile(owner, pooledObj);
        OnFinalize();
    }

    protected abstract void ConfigureProjectile(GameObject owner, PoolInitializedBehavior pooledObj);
}
