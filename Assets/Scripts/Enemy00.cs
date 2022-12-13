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
    // コンポーネントのキャッシュ
    private NavMeshAgent _Nav;
    private BasicMovement _Mover;

    // 巡回/発見/接近/攻撃（ショットガン）を行うエネミ
    // BehaviorTreeっぽい分岐構造で実装する

    [SerializeField]
    private float _IdleInterval = 3.0f;
    [SerializeField]
    private float _ReachRange = 2.0f;
    [SerializeField]
    private float _GoalRange = 1.0f;
    [SerializeField]
    private PathObject _PathObject = null; // パスが設定されているオブジェクト
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
        //遷移時の状態初期化処理とか
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

    #region Patrol関連
    #region PatrolState
    // パトロール中の状態変数
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

    // このへんすごいシーケンスのノードっぽい...BehaviorTreeほしい...。

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
        // ゴールについたらパスを無効にする
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
        // ReachRangeより外側でかつ最も近い次の目的地をパスオブジェクトから取得する
        _NextDest = _PathObject.ChoiseNextDest(transform.position, _GoalRange, false);
        if (_NextDest.HasValue)
        {
            var dest = _NextDest.Value;
            _Nav.SetDestination(dest);
        }
    }
    public void MoveDelta()
    {
        // 移動処理をこのへんに適当に書く（後々多分ステートマシンでやることになると思うけど）
        // ステアリングベクトルに向かって進むようにする。
        _Mover.DesiredDirection = (_NextDest.Value - transform.position).normalized;
        
    }
    #endregion
    #endregion

    #region Attack関連
    #region AttackState
    [SerializeField]
    private Enemy00AttackState _AttackState = Enemy00AttackState.Track;
    private GameObject _Target = null;
    private float _AttackRange = 3.0f;
    #endregion

    void InitAttackState()
    {
        // 追跡から開始する
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
        // 射程内に入ったらAttackステートへ遷移
    }

    void UpdateShoot()
    {
        // 攻撃が終了したらTrackへ遷移
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        // ここがBehaviorTreeでいうところのルートノード
        // 毎回発見/喪失チェック
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
