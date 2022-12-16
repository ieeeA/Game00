using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IProjectileHit
{
    //void Hit(object bullet, ProjectileHitInfo hitInfo);
}

public class Projectile : PoolInitializedBehavior
{
    [SerializeField]
    public string hitEffectId;
    [SerializeField]
    public float LifeTime = 5.0f;
    [SerializeField]
    public float radius = 0.1f;
    [SerializeField]
    public string[] hitLayer;
    [SerializeField]
    public float _Gravity = 0.0f;
    [SerializeField]
    public bool _IsEnableHitExplode = false;
    [SerializeField]
    public float _EffectRadius = 5.0f;
    [SerializeField]
    public UnityEvent<DataOperationInfo> _OnHit;
    [SerializeField]
    public UnityEvent<DataOperationInfo> _OnExplodeHit;
    [SerializeField]
    public string _Tag;

    private Vector3 _Vel;
    private float _Timer;

    private Vector3 _prevPos;

    private bool _IsConfigured = false;

    public GameObject Sender { get; set; }

    public void Configure(float speed, Vector3 dir)
    {
        _Vel = dir * speed;
        _prevPos = transform.position;
        transform.LookAt(_Vel + transform.position);
        _IsConfigured = true;
    }

    // Start is called before the first frame update
    public override void OnInstantiated()
    {
        _Timer = LifeTime;
        _IsConfigured = false;
    }

    private void OnHit(RaycastHit hit)
    {
        Debug.Log("[Bullet] Hit!");
        var t = new DataOperationInfo() { Collider = hit.collider, Sender = Sender };
        _OnHit?.Invoke(t);
        var eff = VFXManager.Current.Instantiate(hitEffectId);
        eff.transform.position = hit.point;
        
        foreach (var collider in FieldUtil.SearchCollider(hit.point, _EffectRadius, _Tag))
        {
            _OnExplodeHit?.Invoke(t);    
        }

        Release();
    }

    // Update is called once per frame
    void Update()
    {
        if (_IsConfigured == false)
        {
            return;
        }

        transform.position += _Vel * Time.deltaTime;
        _Timer -= Time.deltaTime;
        if (_Timer < 0)
        {
            Release();
        }

        // d—Íˆ—
        _Vel += Vector3.down * _Gravity * Time.deltaTime;
        
        RaycastHit hit;
        if (Physics.SphereCast(
            new Ray(_prevPos, (transform.position - _prevPos).normalized),
            radius,
            out hit,
            Vector3.Distance(transform.position, _prevPos),
            LayerMask.GetMask(hitLayer)))
        {
            OnHit(hit);
        }
    }
}
