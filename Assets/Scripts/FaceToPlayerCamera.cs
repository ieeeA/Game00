using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToPlayerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraController.PlayerCameraCurrent != null)
        {
            var pos = CameraController.PlayerCameraCurrent.transform.position;
            transform.LookAt(- (pos - transform.position) + transform.position);
        }
    }
}
