using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectToGroundModule : FireModuleImplBaseV0
{
    // Start is called before the first frame update
    public override FireModuleType Type => FireModuleType.EffectToGround;
    private Vector3 _placement; // 効果フィールドを配置する場所

    private Vector3 GetRaycastOrigin(GameObject owner)
    {
        var pos = owner.transform.position;
        pos += Quaternion.AngleAxis(owner.transform.rotation.eulerAngles.y, Vector3.up) * _CurrentState._Offset0;
        return pos;
    }

    public override void OnFire(GameObject owner)
    {
        var mgr = owner.GetComponent<TimerEffectManager>();
        if (false == FieldUtil.GetPositionRaycastToTerrain(GetRaycastOrigin(owner), _CurrentState._Range0, out _placement))
        {
            Debug.Log("[EffectToGroundModule] Effectフィールドを配置できません。");
            return;
        }
        base.OnFire(owner);
    }

    protected override void ConfigureProjectile(GameObject owner, PoolInitializedBehavior pooledObj)
    {
        if (pooledObj is BasicPooledBehavior proj)
        {
            proj.transform.position = _placement;
        }
    }
}
