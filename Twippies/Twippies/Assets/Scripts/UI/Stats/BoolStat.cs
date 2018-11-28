public class BoolStat : Stat {

    private bool _value;
    private string _label;

    public BoolStat(bool value, string label)
    {
        _sType = StatType.Bool;
        _name = "bool";
        _label = label;
        _value = value;
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
}
