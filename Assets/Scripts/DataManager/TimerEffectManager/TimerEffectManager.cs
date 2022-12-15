using Microsoft.Build.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// タグを
public enum TimerEffectType
{
    None = 0x00,
    Permanent = 0x01,
    EasyCancelableAction = 0x02,
    HardCancelableAction = 0x04,
    CancelableParameterEffect = 0x08,
}

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
        var appliedEff = SearchEffect(effect.Id);
        if (appliedEff != null && effect.IsDistinct)
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
            effect.Start(gameObject, this);
        }
    }

    public void Remove(TimerEffect effect)
    {
        effect.Cancel(gameObject);
        _Effects.Remove(effect);
    }

    public TimerEffect SearchEffect(string id)
    {
        return _Effects.Find(x => x.Id == id);
    }

    public IEnumerable<TimerEffect> SearchEffect(TimerEffectType typeBitArray)
    {
        // yield でやるとforeachしながらRemoveを呼ばれて、Collection was Changedを起こす可能性があるので
        // リストを作っておく
        return _Effects.Where(x => (typeBitArray & x.EffectType) != 0).ToList();
    }
}

public class TimerEffect
{
    public enum TimerEffectApplyMode
    {
        Additive, // 加算
        Overwrite, // 上書き
    }

    public string Id { get; init; } // 重複登録回避用のID
    public bool IsDistinct { get; init; } = true; // 重複登録を許すかどうか
    public TimerEffectApplyMode ApplyMode { get; init; } = TimerEffectApplyMode.Overwrite; // 追加付与モード
    public bool IsIterative { get; init; } = false; // Intervalによる繰り返し効果を許容するか
    public TimerEffectType EffectType { get; init; } = TimerEffectType.Permanent;

    /// <summary>
    /// 引数構成: 効果先オブジェクト、効果元オブジェクト（任意, owner）, Action間で情報をやり取りするためのもの
    /// </summary>
    public System.Action<GameObject, GameObject, object> OnStart { get; init; }
    /// <summary>
    /// 引数構成: 効果先オブジェクト、効果元オブジェクト（任意, owner）, Action間で情報をやり取りするためのもの
    /// </summary>
    public System.Action<GameObject, GameObject, object> OnInterval { get; init; }
    /// <summary>
    /// 引数構成: 効果先オブジェクト、効果元オブジェクト（任意, owner）, Action間で情報をやり取りするためのもの
    /// </summary>
    public System.Action<GameObject, GameObject, object> OnCancel { get; init; } // キャンセルされたときに呼ばれる
    /// <summary>
    /// 引数構成: 効果先オブジェクト、効果元オブジェクト（任意, owner）, Action間で情報をやり取りするためのもの
    /// </summary>
    public System.Action<GameObject, GameObject, object> OnEnd { get; init; }

    public float Interval { get; init; }
    public GameObject Owner { get; init; }
    public object Context { get; init; }
    public float LifeTime { get; init; }
    public bool IsEnd => _LifeTimer < 0;

    private bool _IsCanceled;
    private float _Timer;
    private float _LifeTimer;
    private TimerEffectManager _Manager;

    public TimerEffect(string id)
    {
        Id = id;
        _IsCanceled = false;
    }
    public void Start(GameObject subject, TimerEffectManager manager)
    {
        _LifeTimer = LifeTime;
        OnStart?.Invoke(subject, Owner, Context);
        _Manager = manager;
    }

    public void IntervalAction(GameObject subject)
    {
        OnInterval?.Invoke(subject, Owner, Context);
    }
    
    public void End(GameObject subject)
    {
        if (_IsCanceled == false)
        {
            OnEnd?.Invoke(subject, Owner, Context);
        }
    }

    public void Cancel(GameObject subject)
    {
        OnCancel?.Invoke(subject, Owner, Context);
        _IsCanceled = true;
    }

    public void Update(GameObject subject)
    {
        _LifeTimer -= Time.deltaTime;
        if (IsEnd)
        {
            End(subject);
        }
        if (IsIterative) {
            _Timer -= Time.deltaTime;
            if (_Timer < 0)
            {
                IntervalAction(subject);
                _Timer = Interval;
            }
        }
    }

    public void OverlapApply(TimerEffect eff)
    {
        switch (eff.ApplyMode)
        {
            case TimerEffectApplyMode.Additive:
                _LifeTimer += eff._LifeTimer;
                break;
            case TimerEffectApplyMode.Overwrite:
                _LifeTimer = eff._LifeTimer;
                break;
            default:
                break;
        }
    }

    public string Stringify()
    {
        return $"[{Id}] Distinct::{IsDistinct} ApplyMode::{ApplyMode} Iterative::{IsIterative} Interval::{Interval} Lifetime::{_LifeTimer}";
    }
}
