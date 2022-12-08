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
    private string _Name;
    [SerializeField]
    private int _Count;
    [SerializeField]
    private int _MaxStackCount;

    private bool _IsCollected = false;

    public void Interact(GameObject target)
    {
        ItemManager mgr = target.GetComponent<ItemManager>();
        if (mgr)
        {
            Collect(mgr);
        }
    }

    public virtual void Collect(ItemManager mgr)
    {
        if (_IsCollected)
        {
            return;
        }

        mgr.AddItem(new ItemData { _Name = _Name, _StackMaxCount = _MaxStackCount }, _Count);
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
