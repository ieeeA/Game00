using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlowModule : FireTypeModule
{
    public override FireModuleType Type => FireModuleType.Blow;

    // TODO:
    // ���̂���NPC��Player���p�ɂ���̂ŁA�Ȃ񂩂�Interface��_locker��ς���
    private PlayerSystem _locker;
    private Vector3 _PrevCameraForward;

    public override void OnFire(GameObject owner)
    {
        var mgr = owner.GetComponent<TimerEffectManager>();

        // TODO
        // _NoFixed �����Ă鎞�͓r���ōU�����󂯂���Cancel�����悤�ɂ��Ă���
        var eff = new TimerEffect("ChargeAndFire_NoFixed") 
        {
            _ApplyMode = TimerEffect.ApplyMode.Overwrite,
            _IsDistinct = true,
            _LifeTimer = _CurrentState._ChargeTime,
            _IsIterative = true,
            _Interval = _CurrentState._ChargeEffInterval,
            _Owner = owner,
            _OnStart = null, // start�͂����ł��̂ł���Ȃ�
            _OnInterval = (owner, target, c) =>
            {
                // �`���[�W�G�t�F�N�g���o��
                Debug.Log("Charging!");
                var eff = VFXManager.Current.Instantiate(_CurrentState._ChargeEffId);
                eff.transform.position = owner.transform.position;

                var t = _PrevCameraForward = CameraController.PlayerCameraCurrent.transform.forward;
                t.y = 0;
                t.Normalize();
                eff.transform.LookAt(t + eff.transform.position);
            },
            _OnEnd = (owner, target, c) =>
            {
                // TODO: ���ˏ���
                OnFired(owner);
            },
        };
        Debug.Log("StartCharge...");

        mgr.Apply(eff);

        // TODO: �ړ������b�N����
        _locker = owner.GetComponent<PlayerSystem>();
        if (_locker != null)
        {
            _locker.IsMoveLockedSelf = true;
        }
    }

    // ���˒��������Ȃ��̂�
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var blow = ProjectileManager.Current.Instantiate<BasicPooledBehavior>(_CurrentState._ProjectileId);
        blow.transform.position = owner.transform.position;
        blow.transform.rotation = owner.transform.rotation;

        var t = _PrevCameraForward;
        t.y = 0;
        t.Normalize();
        blow.transform.LookAt(t + blow.transform.position);

        // TODO
        // ���ʎ��ԕ��ړ������b�N����B���̌��������B
        _locker = owner.GetComponent<PlayerSystem>();
        if (_locker != null)
        {
            _locker.IsMoveLockedSelf = false;
        }
    }
}
