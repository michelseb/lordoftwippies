using UnityEngine;

public enum StatType
{
    Label,
    Text,
    Bool,
    Choice,
    Value
}

public abstract class Stat {
    protected StatManager _statManager;
    protected StatType _sType;
    protected string _name;
    protected GameObject _stat;

}
