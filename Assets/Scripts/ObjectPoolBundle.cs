using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// オブジェクトプール上で
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
/// オブジェクトプールのコンテナを管理するクラス
/// これ自体はシングルトン化せずVFXなどのカテゴリごとに管理する
/// 後々ネットを考えるだろうけど同期系の処理はこのクラスでは考えない
/// （同期情報を直接Rpc送信することを考え始めるかも...??
/// コンポーネントでジェネリクス化してるのはGetComponentが重いから
/// PooInitializedBehaviorは初期化がインスタンス化初回まで呼ばれない可能性があるため代替手段
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
        // 多分結構重くなるので、それなりの対処法を考える（まあだいぶあとでいいけど）
        foreach(var conf in _Configs)
        {
            _PoolDict.Add(conf._Id, new T[conf._Count]);
            for (int i = 0; i < conf._Count; i++)
            {
                var comp = _PoolDict[conf._Id][i] = GameObject.Instantiate(conf._PooledObject).GetComponent<T>();
                if (comp == null)
                {
                    Debug.LogError($"指定されたPooleObjectにコンポーネントがありません。{conf._Id}");
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
            Debug.LogError($"存在しないプールが指定されました。id: {id}");
        }

        var pool = _PoolDict[id];
        for (int i = 0; i < pool.Length; i++)
        {
            // 多分activeSelfのほうが高速なのでこちらで判定する。（多分だがparentがactive=Falseになることはほぼない）
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
    public string _Id; // このプールを引き当てるためのID
    public GameObject _PooledObject;
    public int _Count;
}

