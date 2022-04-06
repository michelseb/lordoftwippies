using UnityEngine;
using UnityEngine.UI;

public class DescriptionStat : Stat
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextStat _textStat;

    public void Populate(string statName, Sprite icon, string description, int maxChar, float fontSize, bool readOnly)
    {
        _icon.sprite = icon;
        _textStat.Populate(statName, description, maxChar, fontSize, readOnly);
        _specificName = statName;
    }
}