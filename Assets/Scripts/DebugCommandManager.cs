using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCommandManager : MonoBehaviour
{
    [SerializeField]
    public bool isInfinityBullet;

    void Update()
    {
        if (Input.GetKey(KeyCode.RightControl))
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                //PlayerControllerVer0.Current.gameObject.GetComponent<FireController>()?.AddBullet(99);
            }
        }
    }
}
