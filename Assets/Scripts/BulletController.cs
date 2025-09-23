using UnityEngine;

public class BulletController : MonoBehaviour
{
    private int _hitPoint;

    [SerializeField]
    private int _initialHitPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this._hitPoint = this._initialHitPoint;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void attackedByBullet()
    {


    }
}
