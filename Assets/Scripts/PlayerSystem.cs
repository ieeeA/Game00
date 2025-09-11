using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�̑����ԂȂǌ��������߂̂��̂��Ǘ�����N���X
/// ����ł��Ȃ��Ȃ����Ȃǂ̃o�O���N������Ƃ肠���������Ƃ��Ƀu���[�N������Ń`�F�b�N����
/// </summary>
public class PlayerSystem : MonoBehaviour
{
    private static PlayerSystem _Current;
    public static PlayerSystem Current => _Current;

    public bool IsLocked => Cursor.lockState != CursorLockMode.Locked;
    public bool IsCameraLocked => IsLocked || IsCameraLockedSelf;
    public bool IsMoveLocked => IsLocked || IsMoveLockedSelf;
    public bool IsCameraLockedSelf { get; set; }
    public bool IsMoveLockedSelf { get; set; }

    private void Awake()
    {
        _Current = _Current ?? this;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// �ړ��iTODO�j�ƃJ�[�\�������Lock�����肵�Ȃ������肷��B
    /// </summary>
    /// <param name="value"></param>
    public void SetLockPlayerControl(bool isLock)
    {
        Cursor.lockState = isLock ? CursorLockMode.Confined : CursorLockMode.Locked;
        // TODO
        // �ړ��𐧌����鏈���������ɏ���
    }
}
