using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using System.Linq;
using UnityEngine.UI;
using System;

public class TextHandle
{
    public string Text { get; set; } = "";
    public int Priority { get; set; } = 0;
}

public class StandardTextPlane : MonoBehaviour
{
    public static StandardTextPlane Current { get; private set; }

    private TextMeshProUGUI _Text;

    // Start is called before the first frame update
    void Awake()
    {
        Current = this;
    }

    private void Start()
    {
        _Text = GetComponent<TextMeshProUGUI>();
        _Text.text = "";
    }

    private List<TextHandle> _Handles = new List<TextHandle>();

    public TextHandle CreateTextHandle(int priority = 0)
    {
        var h = new TextHandle() { Priority = priority};
        _Handles.Add(h);
        _Handles.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        return h;
    }

    // Update is called once per frame
    void Update()
    {
        _Text.text = _Handles.Select(x => x.Text + Environment.NewLine).Aggregate((s, x) => s += x );
    }
}


