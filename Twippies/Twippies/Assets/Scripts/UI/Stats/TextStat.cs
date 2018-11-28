public class TextStat : Stat {

    private string _value;
    private int _maxChar;


    public TextStat(string value, int maxChar)
    {
        _sType = StatType.Text;
        _name = "texte";
        _value = value;
        _maxChar = maxChar;
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
