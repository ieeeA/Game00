using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private ObjectPoolBundle<HitEffect> _Pool = new ObjectPoolBundle<HitEffect>();
    public static VFXManager Current { get; private set; }
    private void Awake()
    {
        Current = Current ?? this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _Pool.Allocate();
    }

    public HitEffect Instantiate(string id)
    {
        return _Pool.Instantiate(id);
    }
}
