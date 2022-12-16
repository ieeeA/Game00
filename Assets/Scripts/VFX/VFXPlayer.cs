using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField]
    public string _VfxId;

    [SerializeField]
    public UnityEvent _OnPlay;

    public void Play()
    {
        var vfx = VFXManager.Current.Instantiate(_VfxId);
        vfx.transform.position = transform.position;
    }
}
