using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectileHit
{
    void Hit(object bullet, ProjectileHitInfo hitInfo);
}

public class ProjectileHitInfo
{
    public GameObject Attacker { get; set; }
    public int Damage { get; set; }
}

public class Projectile : PoolInitializedBehavior
{
    private float _ConfiguredSpeed = 0.1f;
    private Vector3 _ConfiguredDir = Vector3.up;

    [SerializeField]
    public string hitEffectId;
    [SerializeField]
    public float LifeTime = 5.0f;
    [SerializeField]
    public float radius = 0.1f;
    [SerializeField]
    public string[] hitLayer;

    private Vector3 _Vel;
    private float _Timer;

    private Vector3 _prevPos;

    public void Configure(float speed, Vector3 dir)
    {
        _ConfiguredSpeed = speed;
        _ConfiguredDir = dir;
        transform.LookAt(transform.position + dir);

        _prevPos = transform.position;
    }

    // Start is called before the first frame update
    public override void OnInstantiated()
    {
        _Timer = LifeTime;
    }

    private void OnHit(RaycastHit hit)
    {
        Debug.Log("[Bullet] Hit!");
        var iHit = hit.collider.GetComponent(typeof(IProjectileHit)) as IProjectileHit;
        if (iHit != null)
        {
            // TODO‚Ü‚ ‚ ‚Æ‚Å•Ï‚í‚é‚â‚ë
            iHit.Hit(this, new ProjectileHitInfo() { Damage = 1 });
        }
        var eff = VFXManager.Current.Instantiate(hitEffectId);
        eff.transform.position = hit.point;
        Release();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += _ConfiguredSpeed * _ConfiguredDir * Time.deltaTime;
        _Timer -= Time.deltaTime;
        if (_Timer < 0)
        {
            Release();
        }

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
