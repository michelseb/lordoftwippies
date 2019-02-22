using TMPro;
using UnityEngine;

public class TextStat : Stat {

    private string _value;
    private int _maxChar;
    private float _fontSize;
    [SerializeField]
    private TextMeshProUGUI _labelField;

    private void Update()
    {
        _labelField.text = _value;
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
    public int MaxChar
    {
        get
        {
            return _maxChar;
        }
        set
        {
            _maxChar = value;
        }
    }

    public void Populate(string value, int maxChar, float fontSize)
    {
        _sType = StatType.Text;
        _name = "texte";
        _value = value;
        _maxChar = maxChar;
        _fontSize = fontSize;
    }
}
