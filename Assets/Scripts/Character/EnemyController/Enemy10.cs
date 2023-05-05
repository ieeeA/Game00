using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FSM;

[RequireComponent(typeof(BasicMovement))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(TimerEffectManager))]
[RequireComponent(typeof(StatusManagerV0))]
[RequireComponent(typeof(DataOperator))]
[RequireComponent(typeof(CharacterController))]
public class Enemy10 : MonoBehaviour
{
    [Header("Combat/Patrol遷移条件")]
    public float _LostRange = 20.0f;
    public float _SearchRange = 10.0f;
    public RangeValue _SearchTime;
    public string _TargetTag;

    [Header("Patrolパラメータ")]
    public float _GoalRange = 5.0f;
    public RangeValue _IdleTime;
    public PathObject _Path; // 巡回ルート定義

    [Header("Attackパラメータ")]
    public float _AttackRange = 2.0f;

    private NavMeshAgent _Nav;
    private BasicMovement _Mover;

    private FSM<Enemy10> _PatrolFSM;
    private FSM<Enemy10> _CombatFSM;
    private FSM<Enemy10> _RootFSM;

    private Vector3? _NextDest;
    private GameObject _Target = null;
    private float _TransitionTimer = 0.0f;
    private float _SearchTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        _Nav = GetComponent<NavMeshAgent>();
        _Nav.updatePosition = false;
        _Mover = GetComponent<BasicMovement>();

        _PatrolFSM = new FSM<Enemy10>(this);
        var patrolIdleState = new FSMState<Enemy10>("patrolIdle");
        patrolIdleState.OnEnter += (c) => _TransitionTimer = _IdleTime.GetValue();
        patrolIdleState.OnUpdate += (c) => _TransitionTimer -= Time.deltaTime;
        patrolIdleState.AddTransition("walk", c => _TransitionTimer < 0.0f);

        var walkState = new FSMState<Enemy10>("walk");
        walkState.OnEnter += (c) => ChoiseNextPoint();
        walkState.OnUpdate += (c) => MoveDelta();
        walkState.AddTransition("patrolIdle", 
            c => _NextDest == null || Vector3.Distance(transform.position, _NextDest.Value) <= _GoalRange
        );
        
        _PatrolFSM.AddState(patrolIdleState);
        _PatrolFSM.AddState(walkState);

        _CombatFSM = new FSM<Enemy10>(this);
        var dodgeState = new FSMState<Enemy10>("dodge");
        var trackState = new FSMState<Enemy10>("track");
        trackState.OnUpdate += (c) =>
        {
            _Nav.SetDestination(_Target.transform.position);
            MoveDelta();
        };
        trackState.AddTransition("attack", c => Vector3.Distance(transform.position, _Target.transform.position) < _AttackRange);
        var attackState = new FSMState<Enemy10>("attack");
        attackState.OnEnter += (c) =>
        {
            Debug.LogWarning("[Enemy01][Attack] Attack状態が未実装です。");
        };
        
        _CombatFSM.AddState(trackState);
        _CombatFSM.AddState(dodgeState);
        _CombatFSM.AddState(attackState);
        
        _RootFSM = new FSM<Enemy10>(this);
        var patrolState = new FSMState<Enemy10>("patrol");
        _RootFSM.AddState(patrolState);
        patrolState.OnEnter += (c) => _PatrolFSM.SetCurrentState("partolIdle", true);
        patrolState.OnUpdate += (c) => _PatrolFSM.Update();
        patrolState.AddTransition("combat", _ => _Target != null);

        var combatState = new FSMState<Enemy10>("combat");
        _RootFSM.AddState(combatState);
        combatState.OnEnter += (c) => _CombatFSM.SetCurrentState("track", true);
        combatState.OnUpdate += (c) => _CombatFSM.Update();
        combatState.AddTransition("patrol", _ => _Target == null);

        _RootFSM.SetCurrentState("patrol", true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSearchTarget();
        _RootFSM.Update();
    }

    #region 
    private void ChoiseNextPoint()
    {
        // ReachRangeより外側でかつ最も近い次の目的地をパスオブジェクトから取得する
        _NextDest = _Path.ChoiseNextDest(transform.position, _GoalRange, false);
        if (_NextDest.HasValue)
        {
            var dest = _NextDest.Value;
            _Nav.SetDestination(dest);
        }
    }

    private void UpdateSearchTarget()
    {
        if (_Target != null && _Target.activeInHierarchy)
        {
            if (Vector3.Distance(_Target.transform.position, transform.position) > _LostRange)
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
        _SearchTimer = _SearchTime.GetValue(); // スパイク対策一斉に探索処理が入るのを防ぐ
        var objs = FieldUtil.Search(transform.position, _SearchRange, _TargetTag);
        if (objs.Length == 0)
        {
            _Target = null;
            return;
        }
        _Target = objs[0];
    }

    private void MoveDelta()
    {
        // 移動処理をこのへんに適当に書く（後々多分ステートマシンでやることになると思うけど）
        // ステアリングベクトルに向かって進むようにする。
        _Mover.DesiredDirection = (_Nav.steeringTarget - transform.position).normalized;
    }
    #endregion

}
