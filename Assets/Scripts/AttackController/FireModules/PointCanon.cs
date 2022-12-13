using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCanon : FireTypeModule
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.ShootBullet;

    // TODO:
    // ���̂���NPC��Player���p�ɂ���̂ŁA�Ȃ񂩂�Interface��_locker��ς���
    private PlayerSystem _locker;

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

                var t = CameraController.PlayerCameraCurrent.transform.forward;
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
        _locker?.SetLockPlayerControl(true);
    }

    private Vector3 CalculateDirection()
    {
        var t = CameraController.PlayerCameraCurrent.transform.forward;
        t += _CurrentState._Offset0;
        return t.normalized;
    }

    // ���˒��������Ȃ��̂�
    protected void OnFired(GameObject owner)
    {
        Debug.Log("Shoot!!!");
        var bullet = ProjectileManager.Current.Instantiate<BasicPooledBehavior>(_CurrentState._ProjectileId);

        if (bullet == null)
        {
            Debug.Log("[ShootBulletModule] bullet �̃C���X�^���X������܂���B");
            _locker?.SetLockPlayerControl(false);
            return;
        }

        bullet.transform.position = owner.transform.position;
        bullet.transform.rotation = owner.transform.rotation;

        var dir = CalculateDirection();
        bullet.transform.LookAt(dir + bullet.transform.position);
        bullet.GetComponent<Projectile>().Configure(_CurrentState._Speed0, dir);

        // TODO
        // ���ʎ��ԕ��ړ������b�N����B���̌��������B
        _locker?.SetLockPlayerControl(false);
    }
}
