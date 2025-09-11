using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItemGimmick : MonoBehaviour, IInteract
{
    // �̎悷��ƈ�莞�ԏ����镔��
    [SerializeField]
    private GameObject _CollectHead;
    [SerializeField]
    private float _RecoveryInteraval;

    [SerializeField]
    private ItemDataBaseV0 _Item;
    [SerializeField]
    private int _Count;

    private bool _IsCollected = false;

    public void Interact(GameObject target)
    {
        PlayerControllerVer0 player = target.GetComponent<PlayerControllerVer0>();
        if (player)
        {
            Collect(player);
        }
    }

    public virtual void Collect(PlayerControllerVer0 player)
    {
        if (_IsCollected)
        {
            return;
        }

        player.Inventory.AddItem(new ItemData(_Item), _Count);
        _CollectHead.SetActive(false);

        // ��X�G�t�F�N�g�̏����Ƃ���������

        StartCoroutine("Recovery"); // ��莞�ԂŃ��J�o�������R���[�`�����Ă�
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Recovery()
    {
        yield return new WaitForSeconds(_RecoveryInteraval);
        _IsCollected = false;
        _CollectHead.SetActive(true);
        yield return null;
    }
}
