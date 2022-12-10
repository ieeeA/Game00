using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/ParameterModiferData")]
public class ParameterModiferData : ScriptableObject
{
    [SerializeField]
    public ParameterType _Type;
    [SerializeField]
    public int _Value;

    public ParameterModifier CreateModifier(string tag = "")
    {
        return new ParameterModifier(_Type, _Value, tag);
    }
}
