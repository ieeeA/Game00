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
    // �o���l�Ƃ����̕ӗp
    public GameObject Sender { get; set; }
}

public class ResistanceChangeInfo
{
    public int ModifyValue { get; set; }
    public GameObject Sender { get; set; }
}