using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoolStat : Stat {

    private bool _value;
    private string _label;

    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private Toggle _toggle;

    private void Update()
    {
        _labelField.text = _label;
        _toggle.isOn = _value;
    }

    public bool Value
    {
        get
        {
            return _value;
        }

        set
        {
            _value = value;
        }
    }

    public string Label
    {
        get
        {
            return _label;
        }

        set
        {
            _label = value;
        }
    }

    public void Populate(bool value, string label)
    {
        _sType = StatType.Bool;
        _name = "bool";
        _label = label;
        _value = value;
    }
}
