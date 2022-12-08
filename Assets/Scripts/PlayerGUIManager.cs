using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGUIManager : MonoBehaviour
{
    private static PlayerGUIManager _Current;
    public static PlayerGUIManager Current => _Current;

    [SerializeField]
    private RequestWindow _Dialog;
    [SerializeField]
    private RequestWindow _ListSelecter;

    public RequestWindow Dialog => _Dialog;
    public RequestWindow ListSelecter => _ListSelecter;

    private void Awake()
    {
        _Current = _Current ?? this;
    }

    private void Start()
    {
        Dialog.OnShow += () => PlayerSystem.Current.SetLockPlayerControl(true);
        ListSelecter.OnShow += () => PlayerSystem.Current.SetLockPlayerControl(true);
        Dialog.OnEnd += () => PlayerSystem.Current.SetLockPlayerControl(false);
        ListSelecter.OnEnd += () => PlayerSystem.Current.SetLockPlayerControl(false);

    }
}
