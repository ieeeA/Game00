using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームオブジェクトに紐づく時間発動系の処理を管理
/// </summary>
public class TimerEffectManager : MonoBehaviour
{
    private List<TimerEffect> _Effects = new List<TimerEffect>();

    // Update is called once per frame
    void Update()
    {
        var removal = new List<TimerEffect>();
        foreach(var eff in _Effects)
        {
            eff.Update(gameObject);
            if (eff.IsEnd)
            {
                removal.Add(eff);
            }
        }
        foreach(var r in removal)
        {
            _Effects.Remove(r);
        }
    }

    public void Apply(TimerEffect effect)
    {
        Debug.Log($"[TimerEffectManager]{effect.Stringify()}");
        var appliedEff = SearchEffect(effect._Id);
        if (appliedEff != null && effect._IsDistinct)
        {
            return;
        }

        if (appliedEff != null)
        {
            appliedEff.OverlapApply(effect);
        }
        else
        {
            _Effects.Add(effect);
            effect.OnStart(gameObject);
        }
    }

    public TimerEffect SearchEffect(string id)
    {
        return _Effects.Find(x => x._Id == id);
    }
}

public class TimerEffect
{
    public enum ApplyMode
    {
        Additive, // 加算
        Overwrite, // 上書き
    }
    public string _Id = ""; // 重複登録回避用のID
    public bool _IsDistinct = true; // 重複登録を許すかどうか
    public ApplyMode _ApplyMode = ApplyMode.Overwrite; // 追加付与モード
    public bool _IsIterative; // Intervalによる繰り返し効果を許容するか
    public System.Action<GameObject, GameObject, object> _OnStart; // 効果先オブジェクト、効果元オブジェクト（任意, owner）, Action間で情報をやり取りするためのもの
    public System.Action<GameObject, GameObject, object> _OnInterval;
    public System.Action<GameObject, GameObject, object> _OnEnd;
    public float _Interval;

    public GameObject _Owner;
    public object _Context;

    public float _LifeTimer;

    private float _Timer;

    public bool IsEnd => _LifeTimer < 0;

    public TimerEffect(string id)
    {
        _Id = id;
    }

    public void OnStart(GameObject subject)
    {
        _OnStart?.Invoke(subject, _Owner, _Context);
    }

    public void OnInterval(GameObject subject)
    {
        _OnInterval?.Invoke(subject, _Owner, _Context);
    }
    public void OnEnd(GameObject subject)
    {
        _OnEnd?.Invoke(subject, _Owner, _Context);
    }

    public void Update(GameObject subject)
    {
        _LifeTimer -= Time.deltaTime;
        if (IsEnd)
        {
            OnEnd(subject);
        }
        if (_IsIterative) {
            _Timer -= Time.deltaTime;
            if (_Timer < 0)
            {
                OnInterval(subject);
                _Timer = _Interval;
            }
        }
    }

    public void OverlapApply(TimerEffect eff)
    {
        switch (eff._ApplyMode)
        {
            case ApplyMode.Additive:
                _LifeTimer += eff._LifeTimer;
                break;
            case ApplyMode.Overwrite:
                _LifeTimer = eff._LifeTimer;
                break;
            default:
                break;
        }
    }

    public string Stringify()
    {
        return $"[{_Id}] Distinct::{_IsDistinct} ApplyMode::{_ApplyMode} Iterative::{_IsIterative} Interval::{_Interval} Lifetime::{_LifeTimer}";
    }
}

// 待って効果発動とか、そういう簡単なEffectを短いコードで作れるようにしておく
public static class TimerEffectUtility
{
    public static TimerEffect CreateWaitEffect(
            string id,
            float lifetime,
            System.Action<GameObject, GameObject, object> acton
        )
    {
        return new TimerEffect(id)
        {
        };
    }
}