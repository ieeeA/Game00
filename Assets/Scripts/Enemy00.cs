using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BasicMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TimerEffectManager))]
[RequireComponent(typeof(StatusManagerV0))]
[RequireComponent(typeof(DataOperator))]
[RequireComponent(typeof(CharacterController))]
public class Enemy00 : MonoBehaviour, IProjectileHit
{
    // �R���|�[�l���g�̃L���b�V��
    private NavMeshAgent _Nav;
    private BasicMovement _Mover;

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
    [SerializeField]
    private bool _IsIgnoreNav = false;
    [SerializeField]
    private float _SearchRange = 10.0f;
    [SerializeField]
    private float _LostRage = 20.0f;

    private float _VerSpeed = 0.0f;


    public enum Enemy00State {
        Patrol,
        Attack,
        Dead
    }

    public enum Enemy00PatrolState
    {
        ChoiseNext,
        Idle,
        Walk,
    }

    public enum Enemy00AttackState
    {
        Track,
        Attack
    }

    [SerializeField]
    private Enemy00State _State = Enemy00State.Patrol;

    private void OnDrawGizmosSelected()
    {
        if (_Nav == null)
        {
            return;
        }

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
        TransitState(Enemy00State.Patrol);
    }

    void TransitState(Enemy00State state)
    {
        //�J�ڎ��̏�ԏ����������Ƃ�
        switch(state)
        {
            case Enemy00State.Patrol:
                InitPatrolState();
                break;
            case Enemy00State.Attack:
                break;
            case Enemy00State.Dead:
                break;
        }
    }

    #region Patrol�֘A
    #region PatrolState
    // �p�g���[�����̏�ԕϐ�
    [SerializeField]
    private Enemy00PatrolState _PatrolState = Enemy00PatrolState.ChoiseNext;
    private Vector3? _NextDest = null;
    private bool _IsIdling = false;
    [SerializeField]
    private float _IdleTimer = 0.0f;
    #endregion

    void InitPatrolState()
    {
        TransitPatrolState(Enemy00PatrolState.ChoiseNext);
    }

    // ���̂ւ񂷂����V�[�P���X�̃m�[�h���ۂ�...BehaviorTree�ق���...�B

    void TransitPatrolState(Enemy00PatrolState patrolState)
    {
        switch (patrolState)
        {
            case Enemy00PatrolState.ChoiseNext:
                break;
            case Enemy00PatrolState.Idle:
                _IdleTimer = _IdleInterval;
                _Mover.DesiredDirection = Vector3.zero;
                break;
            case Enemy00PatrolState.Walk:
                break;
            default:
                break;
        }
        _PatrolState = patrolState;
    }

    void UpdatePatrol()
    {
        switch (_PatrolState)
        {
            case Enemy00PatrolState.ChoiseNext:
                ChoiseNextPoint();
                TransitPatrolState(Enemy00PatrolState.Walk);
                break;
            case Enemy00PatrolState.Idle:
                UpdateIdle();
                break;
            case Enemy00PatrolState.Walk:
                UpdateWalk();
                break;
            default:
                break;
        }
    }

    void UpdateIdle()
    {
        _IdleTimer -= Time.deltaTime;
        if (_IdleTimer <= 0)
        {
            TransitPatrolState(Enemy00PatrolState.ChoiseNext);
        }
    }

    void UpdateWalk()
    {
        // �S�[���ɂ�����p�X�𖳌��ɂ���
        if (Vector3.Distance(transform.position, _NextDest.Value) <= _GoalRange || _Nav.isStopped)
        {
            TransitPatrolState(Enemy00PatrolState.Idle);
            return;
        }
        MoveDelta();  
    }

    #region Actions
    void ChoiseNextPoint()
    {
        // ReachRange���O���ł��ł��߂����̖ړI�n���p�X�I�u�W�F�N�g����擾����
        _NextDest = _PathObject.ChoiseNextDest(transform.position, _GoalRange, false);
        if (_NextDest.HasValue)
        {
            var dest = _NextDest.Value;
            _Nav.SetDestination(dest);
        }
    }
    public void MoveDelta()
    {
        // �ړ����������̂ւ�ɓK���ɏ����i��X�����X�e�[�g�}�V���ł�邱�ƂɂȂ�Ǝv�����ǁj
        // �X�e�A�����O�x�N�g���Ɍ������Đi�ނ悤�ɂ���B
        _Mover.DesiredDirection = (_NextDest.Value - transform.position).normalized;
        
    }
    #endregion
    #endregion

    #region Attack�֘A
    #region AttackState
    [SerializeField]
    private Enemy00AttackState _AttackState = Enemy00AttackState.Track;
    private GameObject _Target = null;
    private float _AttackRange = 3.0f;
    #endregion

    void InitAttackState()
    {
        // �ǐՂ���J�n����
        _AttackState = Enemy00AttackState.Track;
    }

    void TransitAttackState(Enemy00AttackState attackState)
    {
        switch (attackState)
        {
            case Enemy00AttackState.Track:
                break;
            case Enemy00AttackState.Attack:
                break;
            default:
                break;
        }
    }

    void UpdateAttack()
    {
        switch (_AttackState)
        {
            case Enemy00AttackState.Track:
                UpdateTrack();
                break;
            case Enemy00AttackState.Attack:
                UpdateAttack();
                break;
            default:
                break;
        }
    }

    void UpdateTrack()
    {
        // �˒����ɓ�������Attack�X�e�[�g�֑J��
    }

    void UpdateShoot()
    {
        // �U�����I��������Track�֑J��
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        // ������BehaviorTree�ł����Ƃ���̃��[�g�m�[�h
        // ���񔭌�/�r���`�F�b�N
        // if (cond)

        // if (cond)
        switch (_State)
        {
            case Enemy00State.Patrol:
                UpdatePatrol();
                break;
            case Enemy00State.Attack:
                break;
            case Enemy00State.Dead:
                break;
            default:
                break;
        }
    }

    public void Hit(object bullet, ProjectileHitInfo hitInfo)
    {

    }
}
