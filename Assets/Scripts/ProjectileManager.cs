using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolBundle<PoolInitializedBehavior> _Pool = new ObjectPoolBundle<PoolInitializedBehavior>();
    public static ProjectileManager Current { get; private set; }

    private void Awake()
    {
        Current = Current ?? this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _Pool.Allocate();
    }

    public T Instantiate<T>(string id) where T : PoolInitializedBehavior
    {
        return _Pool.Instantiate(id) as T;
    }
}
