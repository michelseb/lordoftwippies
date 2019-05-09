using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueStat : Stat {

    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private Slider _slider;

    public float Value { get; set; }
    public string Label { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    protected override void Start()
    {
        base.Start();
        _slider.minValue = MinValue;
        _slider.maxValue = MaxValue;
        _slider.value = Value;
    }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Label;
        if (ReadOnly)
        {
            _slider.value = Value;
        }
        else
        {
            Value = _slider.value;
        }
    }

    public void Populate(float value, int minValue, int maxValue, string label, bool readOnly, string statName)
    {
        _statType = StatType.Value;
        _name = "value";
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        Label = label;
        ReadOnly = readOnly;
        _specificName = statName;
        if (ReadOnly)
        {
            AssociatedAction = AssociatedAction.Modification;
        }
        else
        {
            AssociatedAction = AssociatedAction.Description;
        }
    }
}
