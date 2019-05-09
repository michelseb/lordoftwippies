using TMPro;
using UnityEngine;

public class TextStat : Stat {

    private float _fontSize;
    [SerializeField]
    private TMP_InputField _textField;

    public string Value { get; set; }
    public int MaxChar { get; set; }

    private void Start()
    {
        _textField.text = Value;
    }

    private void Update()
    {
        Value = _textField.text;
    }

    public void Populate(string value, int maxChar, float fontSize, bool readOnly)
    {
        _statType = StatType.Text;
        _name = "texte";
        Value = value;
        MaxChar = maxChar;
        ReadOnly = readOnly;
        _fontSize = fontSize;
        AssociatedAction = AssociatedAction.Description;
    }
}
