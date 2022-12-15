using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireModuleType
{
    Blow, // �ڂ̑O�Ɍ��ʃt�B�[���h���o��
    EffectToGround, // �v���C���[�̖ڂ̑O�̒n�ʂɌ��ʃt�B�[���h���o��
    Missile,
    ShootBullet,
    PointCanon,
}

/// <summary>
/// �Z�����̋N�_�N���X
/// �e�Z�^�C�v���W���[���̐����������x
/// </summary>
public class FireControllerV2 : MonoBehaviour
{
    // V2�����p
    // �����Z�̌��؂��s���̂ŁA�Z�؂�ւ��@�\�Ȃǌ��ؗp�@�\����������
    // 1, 2, 3...�Ő؂�ւ�
    // �E�N���b�N�Ŕ���
    [SerializeField]
    public int _Current = 0;
    [SerializeField]
    public List<ModuleState> _ModuleStates = new List<ModuleState>();
    [SerializeField]
    private bool _IsPlayer = false;

    protected Dictionary<FireModuleType, FireTypeModule> _ModuleDict = new Dictionary<FireModuleType, FireTypeModule>();
    private TextHandle _handle;

    public T GetModule<T>(FireModuleType type) where T : FireTypeModule
    {
        if (!_ModuleDict.ContainsKey(type))
        {
            Debug.LogError("��������Ă��Ȃ�FireTypeModule�փA�N�Z�X���悤�Ƃ��܂����B");
            return null;
        }
        return _ModuleDict[type] as T;
    }

    public FireTypeModule GetModule(FireModuleType type)
    {
        if (!_ModuleDict.ContainsKey(type))
        {
            Debug.LogError("��������Ă��Ȃ�FireTypeModule�փA�N�Z�X���悤�Ƃ��܂����B");
            return null;
        }
        return _ModuleDict[type];
    }

    public void Fire()
    {
        var targetState = _ModuleStates[_Current];
        var module = GetModule(targetState._Type);
        module.Configure(targetState);

        if (targetState.CanFire() && module.CanFire(gameObject))
        {
            targetState.Fire();
            module.OnFire(gameObject);
        }
    }

    private void InitializeModules()
    {
        // =============================================
        // ���W���[�����������邽�тɂ����ɒǉ����Ă���
        // =============================================
        _ModuleDict.Add(FireModuleType.Blow, new BlowModule());
        _ModuleDict.Add(FireModuleType.EffectToGround, new EffectToGroundModule());
        _ModuleDict.Add(FireModuleType.ShootBullet, new ShootBulletModule());
    }

    private void Start()
    {
        InitializeModules();

        if (_IsPlayer)
        {
            _handle = StandardTextPlane.Current.CreateTextHandle();
        }
    }

    private void Update()
    {
        foreach (var state in _ModuleStates)
        {
            state.Update();
        }

        if (_IsPlayer)
        {
            // ����ꉞ�f�o�b�O�p�����狖����
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _Current = 0;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _Current = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _Current = 2;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _Current = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                _Current = 4;
            }

            _handle.Text = "[FireControllerV2]" + Environment.NewLine;
            for (int i = 0; i < _ModuleStates.Count; i++)
            {
                var s = i == _Current ? "@" : " ";
                var state = _ModuleStates[i];
                _handle.Text += $"{s}[{i}]{state._Type}({state._Time.ToString("F1")})" + Environment.NewLine;
            }
        }
    }
}

public abstract class FireTypeModule
{
    // �����Ɋe�X�̃v���p�e�B��ݒ肵�āACanFire�Ń`�F�b�N�AFire�Ō��ʔ���
    // �Ƃ��������ɂ���B

    public abstract FireModuleType Type { get; }
    protected ModuleState _CurrentState;

    public virtual void Configure(ModuleState state)
    {
        _CurrentState = state;
    }

    // FireController�����ˉ\�ȏ�Ԃ��������߂̂���
    public virtual bool CanFire(GameObject owner)
    {
        return true;
    }

    // �g���K�[���������Ƃ��̏���
    public virtual void OnFire(GameObject owner)
    {

    }
}
