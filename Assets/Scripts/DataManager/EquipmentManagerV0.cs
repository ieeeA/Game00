using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ëïîıÉXÉçÉbÉg
public enum EquipmentPartType
{
    SlotBody,
    SlotHead,
    SlotRightArm,
    SlotLeftArm,
    SlotWaist,
    SlotLeg,
    SlotMax,
}

public class EquipmentManagerV0 : MonoBehaviour
{
    private ParameterBundleV0 _paramBundle;
    private Dictionary<EquipmentPartType, EquipmentDataV0> _slotDictionary = new Dictionary<EquipmentPartType, EquipmentDataV0>();
    private Dictionary<EquipmentPartType, ParameterModifierGroup> _modifierGroupDict = new Dictionary<EquipmentPartType, ParameterModifierGroup>();

    public EquipmentDataV0 GetEquipmentData(EquipmentPartType type) => _slotDictionary[type];

    // Start is called before the first frame update
    void Start()
    {
        _paramBundle = GetComponent<ParameterBundleV0>();
        for (EquipmentPartType i = 0; i < EquipmentPartType.SlotMax; i++)
        {
            _slotDictionary.Add(i, null);
        }
    }

    public void Equip(EquipmentDataV0 equipment)
    {
        var targetSlot = equipment._AttachableType;
        if (_slotDictionary[targetSlot] != null)
        {
            TakeOff(targetSlot);
        }
        _slotDictionary[targetSlot] = equipment;
        var mdg = equipment.GetModifierGroup();

        _paramBundle.Register(mdg);
        _modifierGroupDict[targetSlot] = mdg;
    }

    public void TakeOff(EquipmentPartType slotType)
    {
        if (_slotDictionary[slotType] == null)
        {
            return;
        }

        var eq = _slotDictionary[slotType];
        var mdg = _modifierGroupDict[slotType];

        _paramBundle.UnRegister(mdg);
        _modifierGroupDict[slotType] = null;
    }
}
