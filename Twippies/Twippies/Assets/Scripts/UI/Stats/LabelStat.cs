public class LabelStat : Stat {

    private string _value;

    public LabelStat(string value)
    {
        _sType = StatType.Label;
        _name = "label";
        _value = value;
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
}
