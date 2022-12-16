using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:
/// ������FireTypeModule���Ƃɐ�������悤�ɂ�����������...�B
/// �o�g���̕��������ł܂����烊�t�@�N�^���͂�����
/// </summary>

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ModuleState")]
// ���W���[���̏�Ԃ�
public class ModuleState : ScriptableObject
{
    public float _Time;

    [SerializeField]
    public float _Interval; // �N�[���_�E���^�C��
    [SerializeField]
    public FireModuleType _Type; // �w��FireModuleType
    [SerializeField]
    public string _ProjectileId; // �U���I�u�W�F�N�g�����o�����߂�ID
    [SerializeField]
    public float _MoveSpeedDecreaseRatio; // ���x������
    [SerializeField]
    public float _ChargeTime; // ���˂܂ł̎���
    [SerializeField]
    public float _ChargeEffInterval; // �`���[�W�G�t�F�N�g�̃G�~�b�g���o
    [SerializeField]
    public string _ChargeEffId; // �`���[�W�G�t�F�N�g��ID
    [SerializeField]
    public string[] _LayerNameList0; // Module���ő����ɓ����蔻������邽�߂̃��C���[�����X�g
    [SerializeField]
    public Vector3 _Offset0; // ���炩�̃I�t�Z�b�g�w��p
    [SerializeField]
    public float _Range0; // ���炩�͈͎̔w��p
    [SerializeField]
    public float _Speed0; // ���炩�̃X�s�[�h�w��

    public virtual bool CanFire()
    {
        return _Time <= 0.0f;
    }

    public virtual void Fire()
    {
        _Time = _Interval;
    }

    public virtual void Update()
    {
        if (_Time > 0.0f)
        {
            _Time -= Time.deltaTime;
        }
    }
}
