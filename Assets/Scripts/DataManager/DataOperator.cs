using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataOperator : MonoBehaviour
{
    [SerializeField]
    private GameObject _DropItemPrefab;
    [SerializeField]
    private Vector3 _DropOffset;
    [SerializeField]
    private float _KnockoutTime;

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ��莞�ԓ����Ȃ�����
    /// </summary>
    public void Knockout()
    {
        var tem = gameObject.GetComponent<TimerEffectManager>();
        if (tem != null)
        {
            // OwnerScale��0�ɂ���
            var modifier = new ParameterModifier(ParameterType.MoveOwnerScale, -1.0f);
            tem.ApplyParameterModifier(
                _KnockoutTime,
                modifier,
                "Knockout");
        }
    }

    /// <summary>
    /// TimerEffectManager��StateMachine���Q�Ƃ��āA_NoFixedSuffix�̂���Effect���L�����Z������
    /// ���Ƃ̏�Ԃɖ߂��B
    /// </summary>
    public void CancelAction()
    {

    }

    public void DropItem()
    {
        var obj = Instantiate(_DropItemPrefab);
        obj.transform.position = transform.position + _DropOffset;
    }
}
