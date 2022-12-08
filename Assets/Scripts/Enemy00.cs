using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy00 : MonoBehaviour, IProjectileHit
{
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

    private float _VerSpeed = 0.0f;

    public enum Enemy00State {
        Patrol,
        Attack,
        Dead
    }

    private Enemy00State _State = Enemy00State.Patrol;
    private NavMeshAgent _Nav;
    private BasicMovement _Mover;

    // パトロール中の状態変数
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
        //遷移時の状態初期化処理とか
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
        // ReachRangeより外側でかつ最も近い次の目的地をパスオブジェクトから取得する
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
        // ゴールについたらパスを無効にする
        if (Vector3.Distance(transform.position, _NextDest.Value) <= _GoalRange || _Nav.isStopped)
        {
            _NextDest = null;
            return;
        }

        // 移動処理をこのへんに適当に書く（後々多分ステートマシンでやることになると思うけど）
        // ステアリングベクトルに向かって進むようにする。
        _Mover.DesiredDirection = _Nav.desiredVelocity;
    }

    void UpdateAttack()
    {
        // 一定距離以内
        // if (cond)
        {
            UpdateShoot();
        }
        // 追跡
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
        // ここがBehaviorTreeでいうところのルートノード
        // 毎回発見/喪失チェック
        // if (cond)

        // if (cond)
        UpdatePatrol();
        _Nav.nextPosition = transform.position;
    }

    public void Hit(object bullet, ProjectileHitInfo hitInfo)
    {

    }
}
