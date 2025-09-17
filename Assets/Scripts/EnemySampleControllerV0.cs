using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemySampleControllerV0 : MonoBehaviour
{
    [SerializeField]
    private float _distanceOfAttack;
    [SerializeField]
    private int _attackPower;
    [SerializeField]
    private float _attackingIntervalSecond;


    private DateTime _timeAttacked;
    private BasicMovement _basicMove;
    private NavMeshAgent _agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _basicMove = GetComponent<BasicMovement>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        // ‚ ‚Ù
        var pl = GameObject.Find("Player");
        if (pl != null)
        {
            // ‚Î‚©
            _agent.destination = pl.transform.position;
        }

        // ‚Ü‚Ê‚¯
        float distance = Vector3.Distance(this.transform.position, pl.transform.position);
        if (distance < _distanceOfAttack && (DateTime.Now - _timeAttacked).TotalSeconds > _attackingIntervalSecond)
        {
            _timeAttacked = DateTime.Now;
            pl.GetComponent<ParameterBumdleV1>().Status.Damaged(_attackPower);
        }

    }
}
