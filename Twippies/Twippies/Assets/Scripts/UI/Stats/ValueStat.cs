using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueStat : Stat {

    private float _value;
    private int _minValue;
    private int _maxValue;
    private string _label;
    private bool _readOnly;
    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private Slider _slider;

    private void Start()
    {
        _slider.minValue = _minValue;
        _slider.maxValue = _maxValue;
    }

    private void Update()
    {
        _labelField.text = _label;
        _slider.value = _value;
    }

    public float Value
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

    public int MinValue
    {
        get
        {
            return _minValue;
        }

        set
        {
            _minValue = value;
        }
    }

    public int MaxValue
    {
        get
        {
            return _maxValue;
        }

        set
        {
            _maxValue = value;
        }
    }

    public bool ReadOnly
    {
        get
        {
            return _readOnly;
        }

        set
        {
            _readOnly = value;
        }
    }

    public void Populate(int value, int minValue, int maxValue, string label, bool readOnly)
    {
        _sType = StatType.Value;
        _name = "value";
        _value = value;
        _minValue = minValue;
        _maxValue = maxValue;
        _label = label;
        _readOnly = readOnly;
    }
}
