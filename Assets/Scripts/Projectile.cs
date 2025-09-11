using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{
    public float Speed { get; set; }
    public Vector3 Direction { get; set; }

    // Update is called once per frame
    void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;    
    }
}
