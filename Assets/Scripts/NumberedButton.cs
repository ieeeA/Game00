using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberedButton : MonoBehaviour
{
    protected int _No;
    protected Button _Button;
    protected Action<int> _OnClick;

    protected bool _Initalized = false;

    public void Init(int no, Action<int> onClick)
    {
        if (_Initalized == false)
        {
            _Button = GetComponent<Button>();
            _Button.onClick.AddListener(Notify);
        }

        _No = no;
        _OnClick = onClick;
    }
    
    private void Notify()
    {
        _OnClick?.Invoke(_No);
    }
}
