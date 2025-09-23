using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ParameterBumdleV1 : MonoBehaviour
{
    [SerializeField]
    private CharacterStatus _characterStatus;

    public CharacterStatus Status { get => _characterStatus; }

    public void Start()
    {
        this.Status.ResetStatus();
    }

    public void Update()
    {
        this.Status.UpdateStatus();
        if (this.Status.State == CharacterStatus.HealthState.Daed)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

}
