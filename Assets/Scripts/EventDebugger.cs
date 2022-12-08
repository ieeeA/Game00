using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEntry
{
    public float Time = 5.0f;
    public string Text = "";
}


public class EventDebugger : MonoBehaviour
{
    private TextHandle _Handle;

    public static EventDebugger Current { get; private set; }

    private void Awake()
    {
        Current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // プライオリティ10にして末尾に表示する
        _Handle = StandardTextPlane.Current.CreateTextHandle(10);
    }

    List<EventEntry> _Entries = new List<EventEntry>();

    public void AppendEventDebug(string text, float time = 5.0f)
    {
        _Entries.Add(new EventEntry() { Text = text, Time = time });
    }

    // Update is called once per frame
    void Update()
    {
        List<EventEntry> remove = new List<EventEntry>();
        string text = "[PresentEvents]" + Environment.NewLine;
        foreach(var e in _Entries)
        {
            e.Time -= Time.deltaTime;

            if (e.Time < 0)
            {
                remove.Add(e);
            }
            text += e.Text + $"({e.Time})" + Environment.NewLine;
        }
        _Handle.Text = text;


        foreach (var r in remove)
        {
            _Entries.Remove(r);
        }
    }
}
