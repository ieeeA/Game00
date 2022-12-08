using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//public class PlayerControllerVer0 : MonoBehaviour, IProjectileHit
public class PlayerControllerVer0 : MonoBehaviour
{
    public static PlayerControllerVer0 Current { get; private set; }

    [SerializeField]
    private CameraController _camera;

    // 一時的にSerialized

    //[SerializeField]
    //private int MaxHP = 100;

    //private CharacterController _charaCon;
    private FireControllerV2 _fireConV2;
    private BasicMovement _basicMove; // 敵の移動ロジックとプレイヤーの移動ロジックをある程度統一

    //private int _HP;
    //private int HP
    //{
    //    get => _HP;
    //    set
    //    {
    //        _HP = value;
    //    }
    //}

    //private TextHandle _DebugTextHandle;

    // Start is called before the first frame update
    void Start()
    {
        //_charaCon = GetComponent<CharacterController>();
        _fireConV2 = GetComponent<FireControllerV2>();
        _basicMove = GetComponent<BasicMovement>();

        //_DebugTextHandle = StandardTextPlane.Current.CreateTextHandle(-1); // 最優先"
        //HP = MaxHP;
    }

    private void Awake()
    {
        Current = this;
    }

    // Update is called once per frame
    void Update()
    {
        var iVec = GetInputVect();
        _basicMove.DesiredDirection = iVec;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _basicMove.Jump();
        }

        if (Input.GetMouseButtonDown(0) && PlayerSystem.Current.IsLocked == false)
        {
            //_fireCon.Fire(transform.position, _camera.transform.forward);
            var f = CameraController.PlayerCameraCurrent.transform.forward;
            f.y = 0.0f;
            transform.LookAt(transform.position + f.normalized);
            _fireConV2.Fire();
        }
    }

    private Vector3 GetInputVect()
    {
        if (PlayerSystem.Current.IsLocked == false)
        {
            var hori = Input.GetAxisRaw("Horizontal");
            var vert = Input.GetAxisRaw("Vertical");

            return _camera.FrontXZ * vert + _camera.RightXZ * hori;
        }
        return Vector3.zero;
    }

    //public void Hit(object bullet, ProjectileHitInfo hitInfo)
    //{
    //    if (HP < 0)
    //    {
    //        return;
    //    }

    //    Debug.Log("Hit");
    //    int damage = hitInfo.Damage;
    //    HP -= damage;
    //    EventDebugger.Current.AppendEventDebug("Damage..!!", 1.0f);

    //    if (HP < 0)
    //    {
    //        EventDebugger.Current.AppendEventDebug("Dead!!", 100.0f);
    //    }
    //}
}
