using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BasicPooledBehavior : PoolInitializedBehavior
{
    [SerializeField]
    public float _Lifetime;
    [SerializeField]
    public UnityEvent OnInit;
    [SerializeField]
    public float _SubLifetime;
    [SerializeField]
    public UnityEvent OnSubTimerEnd;

    public float _Timer;
    public float _SubTimer;

    public override void OnInstantiated()
    {
        _Timer = _Lifetime;
        _SubTimer = _SubLifetime;
    }

    public override void OnPostInstantiated()
    {
        OnInit?.Invoke();
    }

    public virtual void SubTimerEnd()
    {
        OnSubTimerEnd?.Invoke();
    }

    void Update()
    {
        _Timer -= Time.deltaTime;
        _SubTimer -= Time.deltaTime;
        if (_Timer < 0)
        {
            Release();
        }
        if (_SubTimer < 0)
        {
            SubTimerEnd();
        }
    }
}
