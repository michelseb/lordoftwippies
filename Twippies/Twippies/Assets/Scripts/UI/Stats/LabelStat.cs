using TMPro;
using UnityEngine;

public class LabelStat : Stat {

    private string _value;

    [SerializeField]
    private TextMeshProUGUI _labelField;

    private void Update()
    {
        _labelField.text = _value;
    }

    public string Value
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

    public void Populate(string value)
    {
        _sType = StatType.Label;
        _value = value;
    }
}
