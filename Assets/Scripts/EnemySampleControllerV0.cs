using UnityEngine;
using UnityEngine.AI;

public class EnemySampleControllerV0 : MonoBehaviour
{
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
    }
}
