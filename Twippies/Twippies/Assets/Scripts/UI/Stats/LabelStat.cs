using TMPro;
using UnityEngine;

public class LabelStat : Stat {
    [SerializeField]
    private TextMeshProUGUI _labelField;

    public string Value { get; set; }

    private void Update()
    {
        _labelField.text = Value;
    }

    public void Populate(string value, string statName)
    {
        _statType = StatType.Label;
        Value = value;
        _specificName = statName;
    }
}
