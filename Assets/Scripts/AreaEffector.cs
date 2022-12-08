using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AreaEffector : MonoBehaviour
{
    public enum AreaType
    {
        Sphere,
        Box
    }

    [SerializeField]
    public bool _Once;
    [SerializeField]
    public UnityEvent<Collider> OnHit;
    [SerializeField]
    public UnityEvent OnEffect;
    [SerializeField]
    public float _Interval;

    [SerializeField]
    public AreaType _AreaType;

    [SerializeField]
    public float _Range;
    [SerializeField]
    public Vector3 _HalfExts;
    [SerializeField]
    public Vector3 _LocalOffset;

    private bool isEffectFrame = false;
    private bool isEffected = false;
    public float Timer { get; protected set; }
    public float LifeTimer { get; protected set; }

    public void OnDrawGizmos()
    {
        switch (_AreaType)
        {
            case AreaType.Sphere:
                Gizmos.DrawWireSphere(transform.position, _Range);
                break;
            case AreaType.Box:
                GizmoHelper.DrawOBB(
                    transform.position + transform.rotation * _LocalOffset,
                    _HalfExts,
                    transform.rotation);
                break;
            default:
                break;
        }
    }

    public void InitEffector()
    {
        Timer = _Interval;
        isEffected = false;
    }

    protected void Update()
    {
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            if (_Once && isEffected)
            {
                return;
            }
            if (!_Once) 
            {
                Timer = _Interval;
            }
            isEffected = true;
            foreach (var col in FieldUtil.SearchCollider(transform.position, _Range))
            {
                OnHit?.Invoke(col);
            }
            OnEffect?.Invoke();
        }
        else
        {
            isEffectFrame = false;
        }
    }

    protected virtual Collider[] GetColliders()
    {
        switch (_AreaType)
        {
            case AreaType.Sphere:
                return FieldUtil.SearchCollider(transform.position, _Range);

            case AreaType.Box:
                return FieldUtil.SearchBoxCollider(
                    transform.position + transform.rotation * _LocalOffset, 
                    _HalfExts, 
                    transform.rotation);

            default:
                return null;
        }
    }

    // TODO‚ ‚Æ‚ÅŽÀ‘•
    public void SetLayerMask()
    {

    }
}
