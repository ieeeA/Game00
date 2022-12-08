using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : PoolInitializedBehavior
{
    [SerializeField]
    private float Lifetime = 0.2f;
    private float _timer = 0.0f;

    private ParticleSystem _Particle;

    public override void OnAllocated()
    {
        _Particle = GetComponent<ParticleSystem>();
    }

    // Start is called before the first frame update
    public override void OnInstantiated()
    {
        _timer = Lifetime;
    }

    public override void OnPostInstantiated()
    {
        _Particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer < 0)
        {
            Release();
        }
    }
}
