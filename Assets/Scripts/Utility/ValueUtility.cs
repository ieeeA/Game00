using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RangeValue
{
    public float minValue;
    public float maxValue;
    
    public float GetValue()
    {
        return Random.Range(minValue, maxValue);
    }
}
