using TMPro;
using UnityEngine;

public class LabelStat : Stat {
    [SerializeField]
    private TextMeshProUGUI _labelField;

    public string Value { get; set; }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Value;
    }

    public void Populate(string statName, string value)
    {
        _statType = StatType.Label;
        Value = value;
        _specificName = statName;
        AssociatedAction = AssociatedAction.Description;
    }
}
