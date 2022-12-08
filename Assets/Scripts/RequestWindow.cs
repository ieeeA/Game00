using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メッセージを出して応答を待つようなUIつくるためのBehavior
/// </summary>
public class RequestWindow : WindowBaseBehavior
{
    [SerializeField]
    protected GameObject _ButtonParent;
    [SerializeField]
    protected GameObject _MessageText;

    protected NumberedButton[] _Buttons;
    protected TMP_Text _Message;

    protected bool _CloseOnSelect;
    protected Action<int> _OnSelect;

    protected override void OnInitialized()
    {
        if (_Buttons == null)
        {
            _Buttons = _ButtonParent.GetComponentsInChildren<NumberedButton>();
            int i = 0;
            foreach (var b in _Buttons)
            {
                b.gameObject.SetActive(false);
                b.Init(i, OnSelect);
                i++;
            }
        }
        if (_MessageText != null && _Message == null)
        {
            _Message = _MessageText.GetComponent<TMP_Text>();
        }
    }

    public void SetOptions(int optionCount, string message, string[] optionName, Action<int> onSelect, bool closeOnSelect = true)
    {
        _CloseOnSelect = closeOnSelect;
        if (_Message != null)
        {
            _Message.text = message;
        }
        DisableAllButton();
        for (int i = 0; i < optionCount; i++)
        {
            _Buttons[i].gameObject.SetActive(true);
            _Buttons[i].GetComponentInChildren<TMP_Text>().text = optionName[i];
        }
        _OnSelect = onSelect;
    }

    protected void DisableAllButton()
    {
        foreach(var b in _Buttons)
        {
            b.gameObject.SetActive(false);
        }
    }

    public void OnSelect(int no)
    {
        if (_CloseOnSelect)
        {
            Close();
        }
        _OnSelect?.Invoke(no);
    }
}
