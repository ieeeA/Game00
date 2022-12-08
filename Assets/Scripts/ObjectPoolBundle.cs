using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �I�u�W�F�N�g�v�[�����
public class PoolInitializedBehavior : MonoBehaviour
{
    public virtual void OnAllocated() { }
    public virtual void OnInstantiated() { }

    public virtual void OnPostInstantiated() { }
    public virtual void OnReleased() { }

    public void Release()
    {
        OnReleased();
        gameObject.SetActive(false);
    }
}

/// <summary>
/// �I�u�W�F�N�g�v�[���̃R���e�i���Ǘ�����N���X
/// ���ꎩ�̂̓V���O���g��������VFX�Ȃǂ̃J�e�S�����ƂɊǗ�����
/// ��X�l�b�g���l���邾�낤���Ǔ����n�̏����͂��̃N���X�ł͍l���Ȃ�
/// �i�������𒼐�Rpc���M���邱�Ƃ��l���n�߂邩��...??
/// �R���|�[�l���g�ŃW�F�l���N�X�����Ă�̂�GetComponent���d������
/// PooInitializedBehavior�͏��������C���X�^���X������܂ŌĂ΂�Ȃ��\�������邽�ߑ�֎�i
/// </summary>
[Serializable]
public class ObjectPoolBundle<T> where T : PoolInitializedBehavior
{
    [SerializeField]
    private List<ObjectPoolConfig> _Configs;
    [SerializeField]
    private GameObject _Folder;

    private Dictionary<string, T[]> _PoolDict = new Dictionary<string, T[]>();

    public void Allocate()
    {
        // �������\�d���Ȃ�̂ŁA����Ȃ�̑Ώ��@���l����i�܂������Ԃ��Ƃł������ǁj
        foreach(var conf in _Configs)
        {
            _PoolDict.Add(conf._Id, new T[conf._Count]);
            for (int i = 0; i < conf._Count; i++)
            {
                var comp = _PoolDict[conf._Id][i] = GameObject.Instantiate(conf._PooledObject).GetComponent<T>();
                if (comp == null)
                {
                    Debug.LogError($"�w�肳�ꂽPooleObject�ɃR���|�[�l���g������܂���B{conf._Id}");
                    return;
                }
                comp.transform.SetParent(_Folder.transform);
                comp.OnAllocated();
                comp.gameObject.SetActive(false);
            }
        }
    }

    public T Instantiate(string id)
    {
        if (!_PoolDict.ContainsKey(id))
        {
            Debug.LogError($"���݂��Ȃ��v�[�����w�肳��܂����Bid: {id}");
        }

        var pool = _PoolDict[id];
        for (int i = 0; i < pool.Length; i++)
        {
            // ����activeSelf�̂ق��������Ȃ̂ł�����Ŕ��肷��B�i��������parent��active=False�ɂȂ邱�Ƃ͂قڂȂ��j
            T comp = pool[i];
            if (comp.gameObject.activeSelf == false)
            {
                comp.OnInstantiated();
                comp.gameObject.SetActive(true);
                comp.OnPostInstantiated();
                return comp;
            }
        }
        return null;
    }
}

[Serializable]
public class ObjectPoolConfig
{
    public string _Id; // ���̃v�[�����������Ă邽�߂�ID
    public GameObject _PooledObject;
    public int _Count;
}

