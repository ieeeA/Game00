using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectToGroundModule : FireTypeModule
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.EffectToGround;

    // TODO:
    // ���̂���NPC��Player���p�ɂ���̂ŁA�Ȃ񂩂�Interface��_locker��ς���
    private PlayerSystem _locker;
    private Vector3 _placement; // ���ʃt�B�[���h��z�u����ꏊ

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
            Debug.Log("[EffectToGroundModule] Effect�t�B�[���h��z�u�ł��܂���B");
            return;
        }

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
        blow.transform.position = _placement;

        // TODO
        // ���ʎ��ԕ��ړ������b�N����B���̌��������B
        _locker = owner.GetComponent<PlayerSystem>();
        if (_locker != null)
        {
            _locker.IsMoveLockedSelf = false;
        }
    }
}
