using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public interface IInteract
{
    void Interact(GameObject target);
}

public class InteractManager : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private bool drawGizmo;

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var col = Physics.OverlapSphere(transform.position, radius);
            var target = col.Select(x => x.GetComponent(typeof(IInteract)) as IInteract)
                .Where(x => x != null)
                .FirstOrDefault();
            target?.Interact(gameObject);
        }
    }
}
