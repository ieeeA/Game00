using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD
{
    private TextHandle _PlayerHUDHandle;
    private Dictionary<string, string> _Dict = new();

    public PlayerHUD()
    {
        _PlayerHUDHandle = StandardTextPlane.Current.CreateTextHandle(10);
    }

    public void Refresh()
    {
        _PlayerHUDHandle.Text = "[PlayerHUD]" + Environment.NewLine;
        foreach (var pair in _Dict)
        {
            _PlayerHUDHandle.Text += pair.Key + ":" + pair.Value + Environment.NewLine;
        }
    }

    public int Money
    {
        set { _Dict[nameof(Money)] = value.ToString(); Refresh(); }
    }

    public int HP
    {
        set { _Dict[nameof(HP)] = value.ToString(); Refresh(); }
    }

    public object ResourceCount
    {
        set { _Dict[nameof(ResourceCount)] = value.ToString(); Refresh(); }
    }

}
