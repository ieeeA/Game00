using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InteractData
{
    public PlayerControllerVer0 Sender { get; }

    public InteractData(PlayerControllerVer0 player)
    {
        Sender = player;
    }
}

public interface IInteractable
{
    void Interact(InteractData data);
}

//public class PlayerControllerVer0 : MonoBehaviour, IProjectileHit
public class PlayerControllerVer0 : MonoBehaviour, IInventoryOwner
{
    public static PlayerControllerVer0 Current { get; private set; }

    [SerializeField]
    private float _InteractionRange;

    [SerializeField]
    private GameObject _Prefab;

    [SerializeField]
    private CameraController _camera;

    // コンポーネントキャッシュ
    private BasicMovement _basicMove; // 多分BasicMovementもClass化してもいいかも（エントリポイントあんまり増やしたくない）

    // プレイヤ構成用クラスたち
    private Inventory _inventry;

    private PlayerHUD _hud;

    public Inventory Inventory => _inventry;

    void Start()
    {
        _basicMove = GetComponent<BasicMovement>();
        _inventry = new Inventory(this);
        _hud = new PlayerHUD();

        // HPの初期値を入力してね。あとHPが更新されたら代入してね。

        _hud.HP = -1;
        _hud.ResourceCount = -1;
        _hud.Money = _inventry.Money;
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            ExcecuteInteract();
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

    protected void ExcecuteInteract()
    {
        var cols = Physics.OverlapSphere(transform.position, _InteractionRange);
        foreach (var c in cols) {
            if (c.GetComponent(typeof(IInteractable)) is IInteractable inter)
            {
                inter.Interact(new InteractData(this));
            }
        }
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

    public void Notify(InventoryEventNotify notify)
    {
        _hud.Money = _inventry.Money;
    }
    #endregion
}
