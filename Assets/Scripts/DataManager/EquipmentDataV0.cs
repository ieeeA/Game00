using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
[CreateAssetMenu(menuName = "ScriptableObjects/EquipmentDataV0")]
public class EquipmentDataV0 : ScriptableObject
{
    [SerializeField]
    public EquipmentPartType _AttachableType;
    [SerializeField]
    public ParameterModiferData[] _ParameterModifiers;
    [SerializeField]
    public GameObject _ImagePrefab;
    [SerializeField]
    public GameObject _EquipPrefab;

    public ParameterModifierGroup GetModifierGroup()
    {
        var t = _ParameterModifiers.Select(x => x.CreateModifier()).ToList();
        return new ParameterModifierGroup(t);
    }
}
