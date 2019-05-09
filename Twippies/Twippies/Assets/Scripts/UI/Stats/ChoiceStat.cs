public class ChoiceStat : Stat {

    private string _label;

    public string[] Values { get; set; }
    public int Value { get; set; }
    //[SerializeField]
    //private TextMeshProUGUI _labelField;
    //[SerializeField]
    //private Slider _slider;

    //private void Start()
    //{
    //    _slider.minValue = _minValue;
    //    _slider.maxValue = _maxValue;
    //}

    public virtual void Populate(string label, string[] values, int value, string statName)
    {
        _name = "choice";
        _statType = StatType.Choice;
        AssociatedAction = AssociatedAction.Modification;
        _label = label;
        Values = values;
        Value = value;
        _specificName = statName;
    }
}
