using UnityEngine;

public class LabelStat : Stat {

    private string _value;

    public LabelStat(string value, GameObject stat)
    {
        _sType = StatType.Label;
        _name = "label";
        _value = value;
        _stat = stat;
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
