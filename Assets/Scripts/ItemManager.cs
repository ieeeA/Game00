using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System;

[Serializable]
public class ItemData
{
    public int StackMaxCount => _ItemData._MaxCount;
    public string Name => _ItemData._Name;
    public string _Description => _ItemData._Name;
    public ItemDataBaseV0 _ItemData;

    public ItemData(ItemDataBaseV0 data)
    {
        _ItemData = data;
    }
}

public class ItemContainer
{
    public ItemData Data { get; set; }
    public int Count { get; set; }
}

public class Inventory
{
    public List<ItemContainer> Containers => _Containers;
    private List<ItemContainer> _Containers = new List<ItemContainer>();
    private int _Money = 0;

    public void AddMoney(int money)
    {
        _Money += money;
    }

    public bool TryToRemoveMoney(int value)
    {
        int current = _Money;
        current -= value;
        if (current < 0)
        {
            return false; 
        }
        _Money = current;
        return true;
    }

    public void AddItem(ItemData data, int count = 0)
    {
        // Nameが同一ならスタックする（MaxCountは今のところ判定はしない）
        foreach(var c in _Containers)
        {
            if (c.Data.Name == data.Name)
            {
                c.Count += count;
                return;
            }
        }
        var nc = new ItemContainer()
        { 
            Data = data,
            Count = count
        };
        _Containers.Add(nc);
    }

    public bool TryRemoveItem(int index, int count)
    {
        if (_Containers.Count <= index)
        {
            Debug.LogError($"index:{index}は範囲外の数値です。");
            return false;
        }
        int curCount = _Containers[index].Count;
        curCount -= count;
        if (curCount == 0)
        {
            _Containers.RemoveAt(index);
            return true;
        }
        
        if (curCount < 0)
        {
            return false;
        }

        _Containers[index].Count = curCount;
        return true;
    }

    string StringifyMoney()
    {
        return $"Money:{_Money}";
    }
    
    string StrigifyContaiers()
    {
        string res = "[ItemContainers]";
        if (_Containers.Count != 0)
        {
            res += _Containers.Select(x => Environment.NewLine + $"{x.Data.Name}({x.Count})").Aggregate((x, s) => s += x);
        }
        return res;
    }
}
