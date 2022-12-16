using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject targetObject;

    public static GameObject PlayerCameraCurrent { get; private set; }

    [SerializeField]
    private float _Distance;
    [SerializeField]
    private float _MouseSensitivity = 0.1f;
    [SerializeField]
    public Vector3 _PosOffset = new Vector3(1.0f, 2.0f, 0.0f);
    [SerializeField]
    private float _ZOffset = 2.0f;

    private float _horiParam;
    private float _vertParam;

    // à⁄ìÆé≤ÇÃåˆäJ
    public Vector3 FrontXZ => Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    public Vector3 RightXZ => Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

    private void Awake()
    {
        PlayerCameraCurrent = this.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerSystem.Current.IsCameraLocked)
        {
            // LockÇ≥ÇÍÇƒÇ»Ç¢èÍçáÇÕëÄçÏÇ™îjí]Ç∑ÇÈÇΩÇﬂ
            return;
        }

        var wMouseDelta = -Input.GetAxisRaw("Mouse X");
        var hMouseDelta = -Input.GetAxisRaw("Mouse Y");

        _horiParam += wMouseDelta * _MouseSensitivity;
        _vertParam += hMouseDelta * _MouseSensitivity;

        _vertParam = Mathf.Clamp(_vertParam, -Mathf.PI / 2.1f, Mathf.PI / 2.1f);

        var pos = Orbit();

        transform.position = pos * _Distance + targetObject.transform.position;
        transform.LookAt(targetObject.transform.position + transform.forward * _ZOffset);
        transform.position += transform.rotation * _PosOffset;
    }

    private Vector3 Orbit()
    {
        var horiOrbit = new Vector3(Mathf.Cos(_horiParam), 0.0f, Mathf.Sin(_horiParam));
        horiOrbit *= Mathf.Cos(_vertParam);
        return horiOrbit + Vector3.up * Mathf.Sin(_vertParam);
    }
}
