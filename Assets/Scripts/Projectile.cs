using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public float Speed { get; set; }
    public Vector3 Direction { get; set; }

    [SerializeField]
    public float _TimeToDeath;
    public DateTime _InstantiatedTime { get; set; }

    // Update is called once per frame
    private void Start()
    {
        this._InstantiatedTime = DateTime.Now;
    }

    void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;
        if ((DateTime.Now - this._InstantiatedTime).Seconds > this._TimeToDeath)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
