using UnityEngine;
using UnityEngine.UI;

public class DescriptionStat : Stat
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextStat _textStat;

    private void Start()
    {
        
    }
    public void Populate(Sprite icon, string description, int maxChar, float fontSize)
    {
        _icon.sprite = icon;
        _textStat.Populate(description, maxChar, fontSize);
    }
}