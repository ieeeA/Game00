using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlowModule : FireModuleImplBaseV0
{
    public override FireModuleType Type => FireModuleType.Blow;

    protected override void ConfigureProjectile(GameObject owner, PoolInitializedBehavior pooledObj)
    {
        if (pooledObj is BasicPooledBehavior proj)
        {
            proj.transform.position = owner.transform.position;
            proj.transform.rotation = owner.transform.rotation;
            var t = _User.GetFocusVector();
            t.y = 0;
            t.Normalize();
            proj.transform.LookAt(t + proj.transform.position);
        }
    }
}
