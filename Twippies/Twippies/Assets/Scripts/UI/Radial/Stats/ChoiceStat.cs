using TMPro;
using UnityEngine;

public class ChoiceStat : Stat
{

    private string _label;

    public string[] Choices { get; set; }
    public int Value { get; set; }
    [SerializeField]
    private TextMeshProUGUI _labelField;


    public virtual void Populate(string statName, string label, string[] choices, int value, bool readOnly)
    {
        _name = "choice";
        ReadOnly = readOnly;
        _statType = StatType.Choice;

        AssociatedAction = ReadOnly ? AssociatedAction.Description : AssociatedAction.Modification;

        foreach (var choice in choices)
        {
            var stat = Instantiate(_objectGenerator.GetStat<ImageStat>(), _subMenu.transform);
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
