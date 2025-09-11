using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Trader00Controller : MonoBehaviour, IInteract
{
    // TODO:
    // �Ƃ肠�������l�͌Œ肵�Ƃ�
    // ���ƂœK���ɃA�C�e���ɔ��l��t���Ƃ�
    [SerializeField]
    private int _SaleMoney = 100;
    [SerializeField]
    private List<BuyOption> _BuyOptions = new List<BuyOption>();

    private GameObject _Target;

    // Start is called before the first frame update
    public void Interact(GameObject target)
    {
        // �R�~���j�P�[�V�����J�n
        _Target = target;
        StartConv();
    }

    #region CommAPIs
    public void StartConv(string mgr = "���m�𔄂����蔃������ł����B")
    {
        // �v���C���[�̈ړ����b�N�A�J�[�\�������Ȃ�

        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "���l";
        PlayerGUIManager.Current.Dialog.SetOptions(
                3,
                mgr,
                new string[] {
                "��������",
                "���肽��",
                "��߂Ă���"
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
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "����������";

        var optionNames = _BuyOptions.Select(x => x.Stringify())
            .Concat(new string[] { "��߂Ă���" })
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
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "�����Ă���̂𔄂�";
        
        var containers = GetItemContainers();
        containers = containers ?? new List<ItemContainer>();

        var optionNames =
            containers.Select(x => StringifyItemOption(x))
            .Concat(new string[] { "��߂Ă���" })
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
        PlayerGUIManager.Current.Dialog.WindowTitle = "���l";
        PlayerGUIManager.Current.Dialog.SetOptions(
                2,
                opt.Stringify() + " �𔃂������H",
                new string[] {
                "�͂�",
                "��߂Ă���"
                },
                (x) =>
                {
                    switch (x)
                    {
                        case 0:
                            // �w������
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
            // �w�����s����
            if (itemMgr.TryToRemoveMoney(opt._Money))
            {
                itemMgr.AddItem(opt._ItemData, opt._Count);
                StartConv("�܂��Ȃɂ����������H����Ƃ����邩���H");
            }
            else
            {
                CloseConv("����������Ȃ��ˁB�����������Ă܂����ĂˁB");
            }
        }
        else
        {
            Debug.LogError("�Ώۂ�ItemManager�������Ă��܂���");
        }
    }

    public void SaleTradeAggreement(ItemContainer itemContainer)
    {
        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "���l";
        PlayerGUIManager.Current.Dialog.SetOptions(
                5,
                $"{itemContainer.Data.Name}(������: {itemContainer.Count})" + " ����������񂾂��H",
                new string[] {
                "1��",
                "5��",
                "10��",
                "100��",
                "��߂Ă���"
                },
                (x) =>
                {
                    switch (x)
                    {
                        // TODO: ���̕ӂ��̂������ꂢ�ɂ���B
                        case 0:
                            // �w������
                            ExecuteSale(itemContainer, 1);
                            break;
                        case 1:
                            // �w������
                            ExecuteSale(itemContainer, 5);
                            break;
                        case 2:
                            // �w������
                            ExecuteSale(itemContainer, 10);
                            break;
                        case 3:
                            // �w������
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
                StartConv("�܂��Ȃɂ����������H����Ƃ����邩���H");
            }
            else
            {
                CloseConv("��������Ȃ��ˁB���낦�Ă܂����ĂˁB");
            }
        }
        else
        {
            Debug.LogError("�Ώۂ�ItemManager�������Ă��܂���");
        }
    }

    public void CloseConv(string mgr = "�܂����Ă�")
    {
        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = "���l";
        PlayerGUIManager.Current.Dialog.SetOptions(
                1,
                mgr,
                new string[] {
                "����"
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
            Debug.LogError("�Ώۂ�ItemManager�������Ă��܂���");
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

