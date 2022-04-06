using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoolStat : Stat {
    [SerializeField]
    private TextMeshProUGUI _labelField;
    [SerializeField]
    private GameObject _cover;
    public bool Value { get; set; }
    public string Label { get; set; }

    private void Start()
    {
        if (_cover != null)
        {
            _cover.SetActive(Value);
        }
    }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Label;
        if ((Time.frameCount % 10) == 0)
        {
            if (_cover != null)
            {
                //_cover.SetActive((Random.value < .05f) ? true : Value); //Blink
            }
        }
    }

    public void Populate(string statName, bool value, string label, bool readOnly)
    {
        _statType = StatType.Bool;
        _name = "bool";
        Label = label;
        Value = value;
        ReadOnly = readOnly;
        _specificName = statName;
        if (!ReadOnly)
        {
            AssociatedAction = AssociatedAction.Modification;
        }
        else
        {
            AssociatedAction = AssociatedAction.Description;
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!ReadOnly)
        {
            Value = !Value;
            _cover.SetActive(Value);
        }
    }
}
