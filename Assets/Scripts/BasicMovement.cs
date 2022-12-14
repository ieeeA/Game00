using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������TimerEffectManager����OwnerScale�ɃA�N�Z�X�����ƃn�}�����肷��̂�
// OwnerScale/ExternalForce��ParameterBundle�ˑ��ɂ���

[RequireComponent(typeof(ParameterBundleV0))] 
// ��b�I�ȕ���������s���X�N���v�g
public class BasicMovement : MonoBehaviour
{
    CharacterController _CharaCon;

    [SerializeField]
    private float _HoriAcc = 10.0f;

    [SerializeField]
    private float _Speed = 10.0f;
    [SerializeField]
    private float _MaxHoriSpeed = 10.0f;

    [SerializeField]
    private float _Gravity = 5.0f;

    [SerializeField]
    private float _JumpSpeed = 1.0f;
    [SerializeField]
    private float _HoriFrict = 5.0f;
    [SerializeField]
    private float _Threshold = 0.1f;

    // �ꎞ�I��Serialized
    [SerializeField]
    private bool _isGroundMonitor;

    public Vector2 HoriSpeed { get; private set; }
    public float VerSpeed { get; private set; }

    // �ړ��������͗p�p�����[�^
    public Vector3 DesiredDirection { get; set; }

    // CC�p�ړ��̓X�P�[���W��
    public float OwnerScale => _ParameterBundle.GetParamterFloatOrZero(ParameterType.MoveOwnerScale);
    // CC�p�O��
    public Vector3 ExternalForce
    {
        get
        {
            Vector3 t;
            t.x = _ParameterBundle.GetParamterFloat(ParameterType.MoveExternalForceX);
            t.y = _ParameterBundle.GetParamterFloat(ParameterType.MoveExternalForceY);
            t.z = _ParameterBundle.GetParamterFloat(ParameterType.MoveExternalForceZ);
            return t;
        }
    }

    private ParameterBundleV0 _ParameterBundle;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(DesiredDirection.x, 0, DesiredDirection.y));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(HoriSpeed.x, 0, HoriSpeed.y));
    }

    public void Jump()
    {
        if (_CharaCon.isGrounded)
        {
            Debug.Log("Jump");
            VerSpeed = _JumpSpeed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _CharaCon = GetComponent<CharacterController>();
        _ParameterBundle = GetComponent<ParameterBundleV0>();
    }

    // Update is called once per frame
    void Update()
    {
        var iVec = new Vector2(DesiredDirection.x, DesiredDirection.z) * OwnerScale;
        HoriSpeed += iVec * _HoriAcc;

        // �W�����v�Əd�͂̌v�Z
        _isGroundMonitor = _CharaCon.isGrounded;

        // �d�͂ƌ����̌v�Z
        Decelerate();
        _CharaCon.Move(CalcMoveVelocity() * Time.deltaTime);
    }

    private Vector3 CalcMoveVelocity()
    {
        var horiVel = HoriSpeed * _Speed;

        if (_Threshold < horiVel.magnitude && _Threshold < OwnerScale)
        {
            transform.LookAt(transform.position + new Vector3(horiVel.x, 0, horiVel.y));
        }

        return new Vector3(horiVel.x, VerSpeed * _Speed, horiVel.y) + ExternalForce;
    }

    private void Decelerate()
    {
        // �ړ������x�N�g��
        var _horiNormDir = HoriSpeed.normalized;
        var _horiNorm = HoriSpeed.magnitude;
        _horiNorm = Mathf.Clamp(_horiNorm - _HoriFrict * Time.deltaTime, 0, _MaxHoriSpeed);
        HoriSpeed = _horiNormDir * _horiNorm;

        if (!_CharaCon.isGrounded)
        {
            VerSpeed -= _Gravity * Time.deltaTime;
        }
    }
}
