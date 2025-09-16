using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Projectile : MonoBehaviour
{

    public float Speed { get; set; }
    public Vector3 Direction { get; set; }

    [SerializeField]
    private float _timeToDeath;

    [SerializeField]
    private int _damageScore;

    public DateTime _InstantiatedTime { get; set; }

    private GameObject _owner { get; set; } // ���ˌ��I�u�W�F�N�g

    public GameObject Owner
    {
        get => _owner;
        set => _owner = value;
    }

    const string _enemyTag = "Enemy";

    // Update is called once per frame
    private void Start()
    {
        this._InstantiatedTime = DateTime.Now;
    }

    void Update()
    {
        transform.position += Direction * Speed * Time.deltaTime;


        // �C���X�^���X������Ă���w�莞�Ԃ��߂�����E��
        if ((DateTime.Now - this._InstantiatedTime).Seconds > this._timeToDeath)
        {
            GameObject.Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        // Enemy�^�O�̃I�u�W�F�N�g�ƐڐG���ĂȂ������m�F
        // �������Ă���Enemy�I�u�W�F�N�g���擾����HP�����炷

        if (collider.gameObject.tag.Equals(_enemyTag))
        {
            collider.gameObject.GetComponent<ParameterBumdleV1>().Status.Damaged(_damageScore);
            GameObject.Destroy(this.gameObject);
        }
    }
}
