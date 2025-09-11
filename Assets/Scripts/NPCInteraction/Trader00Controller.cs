using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trader00Controller : MonoBehaviour, IInteract
{
    // TODO:
    // とりあえず売値は固定しとく
    // あとで適当にアイテムに売値を付けとく
    [SerializeField]
    private int _SaleMoney = 100;
    [SerializeField]
    private List<BuyOption> _BuyOptions = new List<BuyOption>();

    private GameObject _Target;

    // Start is called before the first frame update
    public void Interact(GameObject target)
    {
        // コミュニケーション開始
        _Target = target;
        StartConv();
    }

    #region CommAPIs
    public void StartConv(string mgr = "モノを売ったり買ったりできるよ。")
    {
        // プレイヤーの移動ロック、カーソル復活など

        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "商人";
        PlayerGUIManager.Current.Dialog.SetOptions(
                3,
                mgr,
                new string[] {
                "買いたい",
                "売りたい",
                "やめておく"
                },
                (x) =>
                {
                    switch(x)
                    {
                        case 0:
                            StartBuy();
                            break;
                        case 1:
                            StartSale();
                            break;
                        default:
                            CloseConv();
                            break;
                    }
                }
            );
    }

    public void StartBuy()
    {
        PlayerGUIManager.Current.ListSelecter.Show();
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "買い物する";

        var optionNames = _BuyOptions.Select(x => x.Stringify())
            .Concat(new string[] { "やめておく" })
            .ToArray();

        PlayerGUIManager.Current.ListSelecter.SetOptions(
                 optionNames.Length,
                 "",
                optionNames,
                 (x) =>
                 {
                     if (x < _BuyOptions.Count)
                     {
                         BuyTradeAgreement(_BuyOptions[x]);
                     }
                     else
                     {
                         CloseConv();
                     }
                 }
            );
    }

    public void StartSale()
    {
        PlayerGUIManager.Current.ListSelecter.Show();
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "持ってるものを売る";
        
        var containers = GetItemContainers();
        containers = containers ?? new List<ItemContainer>();

        var optionNames =
            containers.Select(x => StringifyItemOption(x))
            .Concat(new string[] { "やめておく" })
            .ToArray();

        PlayerGUIManager.Current.ListSelecter.SetOptions(
                optionNames.Length,
                "",
                optionNames,
                (x) =>
                {
                    if (x <= containers.Count)
                    {
                        SaleTradeAggreement(containers[x]);
                    }
                    else
                    {
                        CloseConv();
                    }
                }
            );
    }

    public void BuyTradeAgreement(BuyOption opt)
    {
        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "商人";
        PlayerGUIManager.Current.Dialog.SetOptions(
                2,
                opt.Stringify() + " を買うかい？",
                new string[] {
                "はい",
                "やめておく"
                },
                (x) =>
                {
                    switch (x)
                    {
                        case 0:
                            // 購入処理
                            ExecuteBuy(opt);
                            break;
                        default:
                            CloseConv();
                            break;
                    }
                }
            );
    }

    private void ExecuteBuy(BuyOption opt)
    {
        var itemMgr = _Target.GetComponent<PlayerControllerVer0>().Inventory;
        if (itemMgr != null)
        {
            // 購入実行処理
            if (itemMgr.TryToRemoveMoney(opt._Money))
            {
                itemMgr.AddItem(opt._ItemData, opt._Count);
                StartConv("まだなにか買うかい？それとも売るかい？");
            }
            else
            {
                CloseConv("お金が足りないね。お金を持ってまた来てね。");
            }
        }
        else
        {
            Debug.LogError("対象がItemManagerを持っていません");
        }
    }

    public void SaleTradeAggreement(ItemContainer itemContainer)
    {
        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "商人";
        PlayerGUIManager.Current.Dialog.SetOptions(
                5,
                $"{itemContainer.Data.Name}(所持数: {itemContainer.Count})" + " をいくつ売るんだい？",
                new string[] {
                "1個",
                "5個",
                "10個",
                "100個",
                "やめておく"
                },
                (x) =>
                {
                    switch (x)
                    {
                        // TODO: この辺そのうちきれいにする。
                        case 0:
                            // 購入処理
                            ExecuteSale(itemContainer, 1);
                            break;
                        case 1:
                            // 購入処理
                            ExecuteSale(itemContainer, 5);
                            break;
                        case 2:
                            // 購入処理
                            ExecuteSale(itemContainer, 10);
                            break;
                        case 3:
                            // 購入処理
                            ExecuteSale(itemContainer, 100);
                            break;
                        default:
                            CloseConv();
                            break;
                    }
                }
            );
    }

    private void ExecuteSale(ItemContainer itemContainer, int count)
    {
        var itemMgr = _Target.GetComponent<PlayerControllerVer0>().Inventory;
        if (itemMgr != null)
        {
            int index = itemMgr.Containers.IndexOf(itemContainer);

            if (itemMgr.TryRemoveItem(index, count))
            {
                itemMgr.AddMoney(_SaleMoney * count);
                StartConv("まだなにか買うかい？それとも売るかい？");
            }
            else
            {
                CloseConv("個数が足りないね。そろえてまた来てね。");
            }
        }
        else
        {
            Debug.LogError("対象がItemManagerを持っていません");
        }
    }

    public void CloseConv(string mgr = "また来てね")
    {
        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "商人";
        PlayerGUIManager.Current.Dialog.SetOptions(
                1,
                mgr,
                new string[] {
                "閉じる"
                },
                (x) =>
                {
                    PlayerGUIManager.Current.Dialog.Close();
                }
            );
    }

    private List<ItemContainer> GetItemContainers()
    {
        var itemMgr = _Target.GetComponent<PlayerControllerVer0>().Inventory;
        if (itemMgr != null)
        {
            return _Target.GetComponent<PlayerControllerVer0>().Inventory.Containers;
        }
        else
        {
            Debug.LogError("対象がItemManagerを持っていません");
        }
        return null;
    }
    
    private string StringifyItemOption(ItemContainer item)
    {
        return $"{item.Data.Name}x{item.Count}";
    }
    #endregion
}


[Serializable]
public class BuyOption
{
    public ItemData _ItemData;
    public int _Money;
    public int _Count;

    public string Stringify()
    {
        return $"{_ItemData.Name}x{_Count}  {_Money}G";
    }
}

