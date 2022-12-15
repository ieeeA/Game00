using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBulletModule : FireModuleImplBaseV0
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.ShootBullet;

    private Vector3 CalculateDirection()
    {
        var t = _PrevCameraForward;
        t += _CurrentState._Offset0;
        return t.normalized;
    }

    protected override void ConfigureProjectile(GameObject owner, PoolInitializedBehavior pooledObj)
    {
        if (pooledObj is Projectile proj)
        {
            proj.transform.position = owner.transform.position;
            proj.transform.rotation = owner.transform.rotation;
            var dir = CalculateDirection();
            proj.Configure(_CurrentState._Speed0, dir);
        }
    }
}
