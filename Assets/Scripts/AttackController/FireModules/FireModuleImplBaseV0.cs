using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �G������
public interface IFireModuleUser
{
    Vector3 GetFocusVector();
}

public abstract class FireModuleImplBaseV0 : FireTypeModule
{
    // DONE??:
    // ���̂���NPC��Player���p�ɂ���̂ŁA�Ȃ񂩂�Interface��_locker��ς���
    // �Ǝv�������ǁA�ړ������Ȃ�f�o�t�Ƃ���ParameterBundle�Ɉړ���0�̃G�t�F�N�g��t������΂�������������
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
            OnStart = OnStart, // start�͂����ł��̂ł���Ȃ�
            OnInterval = null,
            OnCancel = (owner, target, c) =>
            {
                OnFinalize();
            },
            OnEnd = (owner, target, c) =>
            {
                // TODO: ���ˏ���
                OnFired(owner);
            },
        };
        Debug.Log("StartCharge...");
        mgr.Apply(eff);
    }

    protected virtual void OnStart(GameObject owner, GameObject target, object context)
    {
        // �`���[�W�G�t�F�N�g���o��
        Debug.Log("Charging!");
        var eff = VFXManager.Current.Instantiate(_CurrentState._ChargeEffId);
        eff.transform.position = owner.transform.position;
        var t = _User.GetFocusVector();
        t.y = 0;
        t.Normalize();
        eff.transform.LookAt(_User.GetFocusVector() + eff.transform.position);

        // �ړ���Lock����
        // -10������̂͐�Γ����Ȃ��悤�ɂ��邽��
        _MoveLockModifier = new ParameterModifier(ParameterType.MoveHoriSpeedScale, -_CurrentState._MoveSpeedDecreaseRatio);
        _Param.Register(_MoveLockModifier);
    }

    protected virtual void OnFinalize()
    {
        _Param.UnRegister(_MoveLockModifier);
    }

    // ���˒��������Ȃ��̂�
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var pooledObj = ProjectileManager.Current.Instantiate<PoolInitializedBehavior>(_CurrentState._ProjectileId);
        if (pooledObj == null)
        {
            Debug.Log("[FireModuleImplBaseV] proj �̃C���X�^���X������܂���B");
            OnFinalize();
            return;
        }
        ConfigureProjectile(owner, pooledObj);
        OnFinalize();
    }

    protected abstract void ConfigureProjectile(GameObject owner, PoolInitializedBehavior pooledObj);
}
