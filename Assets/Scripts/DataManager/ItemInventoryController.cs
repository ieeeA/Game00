using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemInventoryController : MonoBehaviour
{
    public void OpenInventory()
    {
        var itemMgr = GetComponent<ItemManager>();
        var optionNames =new string[] { "��߂Ă���" }
            .Concat(itemMgr.Containers.Select(x => x.Data.Name + " x" + x.Count))
            .ToArray();

        PlayerGUIManager.Current.ListSelecter.Show();
        PlayerGUIManager.Current.ListSelecter.WindowTitle = "�C���x���g��";
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
                    AccessOption(targetIndex, itemMgr.Containers[targetIndex]);
                }
            });
    }

    public void AccessOption(int index, ItemContainer container)
    {
        // �A�C�e���̌��ʃI�v�V������I������
        var itemData = container.Data._ItemData;
        var options = itemData.GetOptions(gameObject);
        options.Add("Cancel");

        PlayerGUIManager.Current.Dialog.Show();
        PlayerGUIManager.Current.Dialog.WindowTitle = itemData._Name;
        PlayerGUIManager.Current.Dialog.SetOptions(
                options.Count,
                $"{itemData._Name} �ɂ��đ����I�����Ă�������",
                options.ToArray(),
                (x) =>
                {
                    if (x == options.Count - 1)
                    {
                        OpenInventory();
                    }
                    else
                    {
                        // �A�C�e���̌��ʂ𔭓�
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
}
