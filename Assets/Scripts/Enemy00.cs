using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

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
    [SerializeField]
    private string _TargetTag;

    private float _VerSpeed = 0.0f;


    public enum Enemy00State
    {
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
        Debug.Log($"[Enemy00][TransitState] transit to {state}");
        //�J�ڎ��̏�ԏ����������Ƃ�
        switch (state)
        {
            case Enemy00State.Patrol:
                InitPatrolState();
                break;
            case Enemy00State.Attack:
                break;
            case Enemy00State.Dead:
                break;
        }
        _State = state;
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
        if (Vector3.Distance(transform.position, _NextDest.Value) <= _GoalRange)
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
    #endregion
    #endregion

    #region Attack�֘A
    #region AttackState
    [SerializeField]
    private Enemy00AttackState _AttackState = Enemy00AttackState.Track;
    [SerializeField]
    private GameObject _Target = null;
    [SerializeField]
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
        if (_Target == null)
        {
            Debug.LogError("[Enemy00][UpdateTrack] AttackState��_Target��null�ł��B");
            TransitState(Enemy00State.Patrol); // �G���[�ی�����
            return;
        }

        // �\���߂Â�����U����ԂɑJ�ڂ���
        if (Vector3.Distance(transform.position, _Target.transform.position) < _AttackRange)
        {
            TransitAttackState(Enemy00AttackState.Attack);
            return;
        }

        // ����ǐՂ̂��߂Ɉʒu�����X�V����B
        _Nav.SetDestination(_Target.transform.position);
        MoveDelta();
    }

    void UpdateShoot()
    {
        // �U�����I��������Track�֑J��
        Debug.LogWarning("[Enemy00][UpdateTrack] UpdateShoot���������ł��B");
        TransitAttackState(Enemy00AttackState.Track);
    }
    #endregion

    #region Util
    private float _SearchInterval = 1.0f;
    private float _SearchTimer = 0.0f;

    void UpdateSearchTarget()
    {
        if (_Target != null && _Target.activeInHierarchy)
        {
            if (Vector3.Distance(_Target.transform.position, transform.position) > _LostRage)
            {
                _Target = null;
            }
            else
            {
                return;
            }
        }

        _SearchTimer -= Time.deltaTime;
        if (_SearchTimer > 0)
        {
            return;
        }
        _SearchTimer = _SearchInterval + Random.Range(0, 0.1f); // �X�p�C�N�΍��ĂɒT������������̂�h��
        var objs = FieldUtil.Search(transform.position, _SearchRange, _TargetTag);
        if (objs.Length == 0)
        {
            _Target = null;
            return;
        }
        _Target = objs[0];
    }

    public void MoveDelta()
    {
        // �ړ����������̂ւ�ɓK���ɏ����i��X�����X�e�[�g�}�V���ł�邱�ƂɂȂ�Ǝv�����ǁj
        // �X�e�A�����O�x�N�g���Ɍ������Đi�ނ悤�ɂ���B
        _Mover.DesiredDirection = (_Nav.steeringTarget - transform.position).normalized;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        UpdateSearchTarget();
        switch (_State)
        {
            case Enemy00State.Patrol:
                if (_Target != null)
                {
                    TransitState(Enemy00State.Attack);
                }
                UpdatePatrol();
                break;
            case Enemy00State.Attack:
                if (_Target == null)
                {
                    TransitState(Enemy00State.Patrol);
                }
                UpdateAttack();
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
