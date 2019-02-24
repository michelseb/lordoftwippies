using TMPro;
using UnityEngine;

public class TextStat : Stat {

    private string _value;
    private int _maxChar;
    private float _fontSize;
    [SerializeField]
    private TMP_InputField _textField;

    private void Start()
    {
        _textField.text = _value;
    }

    private void Update()
    {
        _value = _textField.text;
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
