using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;


//public class PlayerControllerVer0 : MonoBehaviour, IProjectileHit
public class PlayerControllerVer0 : MonoBehaviour
{

    public enum PlayerActionMode
    {
        Building,
        Attack
    }
    const string _enemyTag = "Enemy";

    public PlayerActionMode Mode { get; set; }

    public static PlayerControllerVer0 Current { get; private set; }

    [SerializeField]
    private GameObject _Prefab;

    [SerializeField]
    private CameraController _camera;

    // コンポーネントキャッシュ
    private BasicMovement _basicMove; // 多分BasicMovementもClass化してもいいかも（エントリポイントあんまり増やしたくない）

    // プレイヤ構成用クラスたち
    private Inventory _inventry;

    public Inventory Inventory => _inventry;

    void Start()
    {
        _basicMove = GetComponent<BasicMovement>();
        _inventry = new Inventory();
        this.Mode = PlayerActionMode.Building;

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // トグルする
            if (PlayerGUIManager.Current.ListSelecter.isActiveAndEnabled)
            {
                CloseInventory();
            }
            else
            {
                OpenInventry();
            }
        }

        if (Input.GetMouseButtonDown(0) && PlayerSystem.Current.IsLocked == false)
        {
            if (Mode == PlayerActionMode.Building)
            {
                var camTrans = CameraController.PlayerCameraCurrent.transform;
                var f = camTrans.forward;
                var lookf = f;
                lookf.y = 0.0f;
                transform.LookAt(transform.position + lookf.normalized);

                Debug.DrawRay(camTrans.position, f * 100.0f, Color.red, 100.0f);
                if (Physics.Raycast(camTrans.position, f, out RaycastHit hitInfo, 100.0f))
                {
                    var v = hitInfo.point;
                    EventDebugger.Current.AppendEventDebug($"[Rayhit]{v.ToString()})");
                    var newObj = GameObject.Instantiate(_Prefab, v, Quaternion.identity);
                }
            }
            else if (Mode == PlayerActionMode.Attack)
            {
                // 攻撃処置

                var camTrans = CameraController.PlayerCameraCurrent.transform;
                var f = camTrans.forward;
                var lookf = f;
                lookf.y = 0.0f;
                transform.LookAt(transform.position + lookf.normalized);

                Debug.DrawRay(camTrans.position, f * 100.0f, Color.red, 100.0f);
                if (Physics.Raycast(camTrans.position, f, out RaycastHit hitInfo, 100.0f))
                {
                    var v = hitInfo.point;
                    EventDebugger.Current.AppendEventDebug($"[Rayhit]{v.ToString()})");
                    var playerStatus = this.gameObject.GetComponent<ParameterBumdleV1>().Status;
                    if (hitInfo.collider.gameObject.TryGetComponent<ParameterBumdleV1>(out ParameterBumdleV1 parameterBumdle) &&
                        hitInfo.collider.gameObject.tag == _enemyTag)
                    {
                        parameterBumdle.Status.Damaged(playerStatus.AttackPower);
                    }
                }
            }


        }


        // 右クリックが押された瞬間
        if (Input.GetMouseButtonDown(1))
        {
            Mode = Mode == PlayerActionMode.Attack ? PlayerActionMode.Building : PlayerActionMode.Attack;
            Debug.Log("行動モード変更：" + Mode.ToString());
        }

    }

    private Vector3 GetInputVect()
    {
        var hori = 0.0f; var vert = 0.0f;
        if (PlayerSystem.Current.IsMoveLocked == false)
        {
            hori = Input.GetAxisRaw("Horizontal");
            vert = Input.GetAxisRaw("Vertical");
        }
        return _camera.FrontXZ * vert + _camera.RightXZ * hori;
    }

    public Vector3 GetFocusVector()
    {
        return CameraController.PlayerCameraCurrent.transform.forward;
    }

    #region インベントリ系
    public void OpenInventry()
    {
        var optionNames = new string[] { "やめておく" }
            .Concat(_inventry.Containers.Select(x => x.Data.Name + " x" + x.Count))
            .ToArray();

        PlayerGUIManager.Current.ListSelecter.Show();
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "インベントリ";
        PlayerGUIManager.Current.ListSelecter.SetOptions(
            optionNames.Length,
            "",
            optionNames,
            (x) =>
            {
                if (x == 0)
                {
                    CloseInventory();
                }
                else
                {
                    var targetIndex = x - 1;
                    AccessInventoryOption(targetIndex, _inventry.Containers[targetIndex]);
                }
            });
    }

    public void AccessInventoryOption(int index, ItemContainer container)
    {
        // アイテムの効果オプションを選択する
        var itemData = container.Data._ItemData;
        var options = itemData.GetOptions(gameObject);
        options.Add("Cancel");

        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = itemData._Name;
        PlayerGUIManager.Current.Dialog.SetOptions(
                options.Count,
                $"{itemData._Name} について操作を選択してください",
                options.ToArray(),
                (x) =>
                {
                    if (x == options.Count - 1)
                    {
                        OpenInventry();
                    }
                    else
                    {
                        // アイテムの効果を発動
                        Debug.Log($"[ItemInventoryController] {itemData._Name}: {options[x]} ({index})");
                        itemData.Execute(options[x], gameObject, index);
                    }
                }
            );
    }

    public void CloseInventory()
    {
        PlayerGUIManager.Current.ListSelecter.Close();
        PlayerGUIManager.Current.Dialog.Close();
    }
    #endregion
}
