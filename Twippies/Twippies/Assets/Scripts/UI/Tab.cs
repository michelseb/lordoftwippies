using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : GraphicElement
{

    [SerializeField]
    private StatPanel _panel;
    [SerializeField]
    private TextMeshProUGUI _textField;
    public bool Focus { get; private set; }

    public void SetFocus(bool focus)
    {
        if (_textField.text != _panel.name)
        {
            Init();
        }
        Color col = _panel.StatManager.Color;
        if (focus)
        {
            foreach (StatPanel panel in _panel.MainStatPanel.StatPanels)
            {
                if (panel != this)
                {
                    panel.Tab.SetFocus(false);
                }
            }
            _panel.transform.SetAsLastSibling();
            _image.color = col;
            _panel.StatManager.SpecificStatPanel.Image.color = col;
            _textField.faceColor = _panel.StatManager.Color + new Color(.8f, .8f, .8f);
        }
        else
        {

            _image.color = new Color(col.r * .4f, col.g * .4f, col.b * .4f, 1);
            _panel.StatManager.SpecificStatPanel.Image.color = _image.color;
            _textField.faceColor = _panel.StatManager.Color + new Color(.3f, .3f, .3f);
            _panel.transform.SetSiblingIndex(_panel.MainStatPanel.StatPanels.Count - 1 - _panel.MainStatPanel.StatPanels.IndexOf(_panel));
        }
        _panel.SetVisible(focus);
        Focus = focus;
    }

    public override void Init()
    {
        base.Init();
        _textField.text = _panel.name;
        _textField.faceColor = _panel.StatManager.Color + new Color(.3f, .3f, .3f);
        _textField.outlineColor = _panel.StatManager.Color - new Color(.5f, .5f, .5f);
    }


}
