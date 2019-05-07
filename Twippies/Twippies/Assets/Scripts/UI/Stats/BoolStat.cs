﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoolStat : Stat {
    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private GameObject _cover;
    public bool Value { get; set; }
    public string Label { get; set; }

    protected override void Start()
    {
        base.Start();
        _cover.SetActive(Value);
    }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Label;
        if ((Time.frameCount % 10) == 0)
        {
            _cover.SetActive((Random.value < .05f) ? true : Value); //Blink
        }
    }

    public void Populate(bool value, string label, string statName)
    {
        _statType = StatType.Bool;
        _name = "bool";
        Label = label;
        Value = value;
        _specificName = statName;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!ReadOnly)
        {
            Value = !Value;
            _cover.SetActive(Value);
        }
    }
}
