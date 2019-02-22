public class ChoiceStat : Stat {

    private string[] _values;
    private int _value;
    private string _label;

    //[SerializeField]
    //private TextMeshProUGUI _labelField;
    //[SerializeField]
    //private Slider _slider;

    //private void Start()
    //{
    //    _slider.minValue = _minValue;
    //    _slider.maxValue = _maxValue;
    //}

    public string[] Values
    {
        get
        {
            return _values;
        }

        set
        {
            _values = value;
        }
    }

    public int Value
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

    public void Populate(string label, string[] values, int value)
    {
        _name = "choice";
        _sType = StatType.Choice;
        _label = label;
        _values = values;
        _value = value;
    }
}
