using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowBaseBehavior : MonoBehaviour
{
    [SerializeField]
    protected Button _CloseButton;
    [SerializeField]
    protected GameObject _WindowTitleText;

    protected TMPro.TMP_Text _WindowTitle;
    protected bool _Initialized = false;

    public event Action OnShow;
    public event Action OnEnd;

    public string WindowTitle
    {
        set
        {
            _WindowTitle.text = value;
        }
    }

    public virtual void Close()
    {
        OnEnd?.Invoke();
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if(_Initialized == false)
        {
            _WindowTitle = _WindowTitleText.GetComponent<TMP_Text>();
            _CloseButton.onClick.AddListener(Close);
            OnInitialized();
        }

        OnShowWindow();
        OnShow?.Invoke();

        _Initialized = true;
    }

    protected virtual void OnInitialized()
    {

    }

    protected virtual void OnShowWindow()
    {

    }
}
