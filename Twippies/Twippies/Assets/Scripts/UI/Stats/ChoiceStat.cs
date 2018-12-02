public class ChoiceStat : Stat {

    private string[] _values;
    private int _value;

    public ChoiceStat(string[] values, int value)
    {
        _sType = StatType.Choice;
        _name = "choice";
        _values = values;
        _value = value;
    }

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

}
