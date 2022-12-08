using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �Q�[���I�u�W�F�N�g�ɕR�Â����Ԕ����n�̏������Ǘ�
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
        Additive, // ���Z
        Overwrite, // �㏑��
    }
    public string _Id = ""; // �d���o�^���p��ID
    public bool _IsDistinct = true; // �d���o�^���������ǂ���
    public ApplyMode _ApplyMode = ApplyMode.Overwrite; // �ǉ��t�^���[�h
    public bool _IsIterative; // Interval�ɂ��J��Ԃ����ʂ����e���邩
    public System.Action<GameObject, GameObject, object> _OnStart; // ���ʐ�I�u�W�F�N�g�A���ʌ��I�u�W�F�N�g�i�C��, owner�j, Action�Ԃŏ�������肷�邽�߂̂���
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

// �҂��Č��ʔ����Ƃ��A���������ȒP��Effect��Z���R�[�h�ō���悤�ɂ��Ă���
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