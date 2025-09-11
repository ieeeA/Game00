using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

public class TurrentControllerV0 : MonoBehaviour
{

    [SerializeField]
    private GameObject _BulletPrefab;
    [SerializeField]
    private string _TargetTag;
    [SerializeField]
    private float _SearchRadius;
    [SerializeField]
    private Vector3 _Offset = new Vector3(0, 2.0f, 0);
    [SerializeField]
    private float _Duration;

    private float _Timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _Timer = 0.0f;    
    }

    // Update is called once per frame
    void Update()
    {
        _Timer += Time.deltaTime;
        if (_Timer < _Duration) return; 
        _Timer = 0.0f;

        foreach(var col in Physics.OverlapSphere(transform.position, _SearchRadius))
        {
            if (col.gameObject.tag == _TargetTag)
            {
                var obj = GameObject.Instantiate(_BulletPrefab);
                
                var rayOrigin = transform.position + _Offset;
                obj.transform.position = rayOrigin;

                var rel = col.transform.position - rayOrigin;
                if (obj.GetComponent(typeof(Projectile)) is Projectile proj)
                {
                    proj.Direction = rel.normalized;
                    proj.Speed = 500f;
                }
                Debug.DrawRay(rayOrigin, col.transform.position - rayOrigin, Color.green, 100.0f);
            }
        }
    }
}
