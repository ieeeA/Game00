using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusManager
{
    void ChangeHP(HPChangeInfo changeInfo);
}

public class HPChangeInfo
{
    public int ModifyValue { get; set; }
    public GameObject Sender { get; set; }
}
