using UnityEngine;

public enum StatType
{
    Label,
    Text,
    Bool,
    Choice,
    Value
}

public abstract class Stat : GraphicElement {
    protected StatManager _statManager;
    protected StatType _statType;
    [SerializeField]
    protected string _name;
    protected bool _main;
    protected ManageableObjet _manageableObjet;
    protected string _specificType;

    public ManageableObjet ManageableObjet
    {
        get { return _manageableObjet; }
        set { _manageableObjet = value; }
    }
    public string Name
    {
        get { return _name; }
    }

    public bool Main
    {
        get { return _main; }
        set { _main = value; }
    }

    public string SpecificType
    {
        get { return _specificType; }
        set { _specificType = value; }
    }
}

