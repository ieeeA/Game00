using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy00 : MonoBehaviour, IProjectileHit
{
    // ����/����/�ڋ�/�U���i�V���b�g�K���j���s���G�l�~
    // BehaviorTree���ۂ�����\���Ŏ�������

    [SerializeField]
    private float _IdleInterval = 3.0f;
    [SerializeField]
    private float _ReachRange = 2.0f;
    [SerializeField]
    private float _GoalRange = 1.0f;
    [SerializeField]
    private PathObject _PathObject = null; // �p�X���ݒ肳��Ă���I�u�W�F�N�g
    [SerializeField]
    private float _Gravity = 5.0f;

    private float _VerSpeed = 0.0f;

    public enum Enemy00State {
        Patrol,
        Attack,
        Dead
    }

    private Enemy00State _State = Enemy00State.Patrol;
    private NavMeshAgent _Nav;
    private BasicMovement _Mover;

    // �p�g���[�����̏�ԕϐ�
    private Vector3? _NextDest = null;
    private bool _IsIdling = false;
    private float _IdleTimer = 0.0f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, transform.position + _Nav.desiredVelocity * 10.0f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _Nav.destination);
    }

    // Start is called before the first frame update
    void Start()
    {
        _Nav = GetComponent<NavMeshAgent>();
        _Nav.updatePosition = false;
        //_Nav.updateRotation = false;
        _Mover = GetComponent<BasicMovement>();
    }

    void TransitState(Enemy00State state)
    {
        //�J�ڎ��̏�ԏ����������Ƃ�
        switch(state)
        {
            case Enemy00State.Patrol:
                break;
            case Enemy00State.Attack:
                break;
            case Enemy00State.Dead:
                break;
        }
    }

    void UpdatePatrol()
    {
        if (_NextDest == null)
        {
            ChoiseNextPoint();
            _Mover.DesiredDirection = Vector3.zero;
        }
        else
        {
            UpdateWalk();
        }
    }

    void ChoiseNextPoint()
    {
        // ReachRange���O���ł��ł��߂����̖ړI�n���p�X�I�u�W�F�N�g����擾����
        _NextDest = _PathObject.ChoiseNextDest(transform.position, _ReachRange, false);
        if (_NextDest.HasValue)
        {
            var dest = _NextDest.Value;
            _Nav.SetDestination(dest);
        }
    }

    void UpdateIdle()
    {
        _IdleTimer -= Time.deltaTime;
        if (_IdleTimer < 0)
        {
            _IsIdling = false;
        }
    }

    void UpdateWalk()
    {
        // �S�[���ɂ�����p�X�𖳌��ɂ���
        if (Vector3.Distance(transform.position, _NextDest.Value) <= _GoalRange || _Nav.isStopped)
        {
            _NextDest = null;
            return;
        }

        // �ړ����������̂ւ�ɓK���ɏ����i��X�����X�e�[�g�}�V���ł�邱�ƂɂȂ�Ǝv�����ǁj
        // �X�e�A�����O�x�N�g���Ɍ������Đi�ނ悤�ɂ���B
        _Mover.DesiredDirection = _Nav.desiredVelocity;
    }

    void UpdateAttack()
    {
        // ��苗���ȓ�
        // if (cond)
        {
            UpdateShoot();
        }
        // �ǐ�
        // if (cond)
        {
            UpdateTrack();
        }
    }

    void UpdateTrack()
    {
        
    }

    void UpdateShoot()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // ������BehaviorTree�ł����Ƃ���̃��[�g�m�[�h
        // ���񔭌�/�r���`�F�b�N
        // if (cond)

        // if (cond)
        UpdatePatrol();
        _Nav.nextPosition = transform.position;
    }

    public void Hit(object bullet, ProjectileHitInfo hitInfo)
    {

    }
}
