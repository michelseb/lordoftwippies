using TMPro;
using UnityEngine;

public class ChoiceStat : Stat {

    private string _label;

    public string[] Choices { get; set; }
    public int Value { get; set; }
    [SerializeField]
    private RadialSubMenu _subMenu;
    [SerializeField]
    private TextMeshProUGUI _labelField;


    public virtual void Populate(string label, string[] choices, int value, string statName, bool readOnly)
    {
        _name = "choice";
        ReadOnly = readOnly;
        _statType = StatType.Choice;
        if (ReadOnly)
        {
            AssociatedAction = AssociatedAction.Description;
        }
        else
        {
            AssociatedAction = AssociatedAction.Modification;
        }
        foreach(var choice in choices)
        {
            var obj = Instantiate(ObjectGenerator.Instance.GetStat<ImageStat>(), _subMenu.transform);
            var stat = obj.GetComponent<Stat>();
            _subMenu.Elements.Add(stat);
            stat.Init();
        }
        _subMenu.Arrange();
        _subMenu.Init();
        _label = label;
        Choices = choices;
        Value = value;
        _specificName = statName;
    }

    public override void Open()
    {
        if (gameObject.activeSelf)
        {
            _animator.ResetTrigger("Close");
            _animator.SetTrigger("Open");
        }
    }
    public override void Close()
    {
        if (gameObject.activeSelf)
        {
            _animator.ResetTrigger("Open");
            _animator.SetTrigger("Close");
            if (_subMenu != null)
            {
                _subMenu.Close();
            }
        }
    }

    public override void Select()
    {
        base.Select();
        _subMenu.Open();
    }

}
