using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolStat : Stat {
    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private Toggle _toggle;

    public bool Value { get; set; }
    public string Label { get; set; }

    private void Start()
    {
        _toggle.isOn = Value;
    }

    private void Update()
    {
        _labelField.text = Label;
        Value = _toggle.isOn;
    }

    public void Populate(bool value, string label, string statName)
    {
        _statType = StatType.Bool;
        _name = "bool";
        Label = label;
        Value = value;
        _specificName = statName;
    }
}
