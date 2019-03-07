using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : GraphicElement
{

    [SerializeField]
    private StatPanel _panel;
    [SerializeField]
    private TextMeshProUGUI _textField;
    private Vector3 _startPos;
    private Vector3 _startScale;
    private bool _focus;

    public void SetFocus(bool focus)
    {
        if (_startScale.magnitude == 0)
        {
            Init();
        }
        if (focus)
        {
            foreach (StatPanel panel in _panel.MainStatPanel.StatPanels)
            {
                if (panel != this)
                {
                    panel.Tab.SetFocus(false);
                }
            }
            //transform.localScale += Vector3.right * .1f;
            _panel.transform.SetAsLastSibling();
            _image.color = _panel.StatManager.Color;
        }
        else
        {
            //transform.localScale = _startScale;
            _image.color = _panel.StatManager.Color * .8f;
            _panel.transform.SetSiblingIndex(_panel.MainStatPanel.StatPanels.Count - 1 - _panel.MainStatPanel.StatPanels.IndexOf(_panel));
        }
        _panel.SetVisible(focus);
        _focus = focus;
    }

    public void SetPosition(int index, int nbTabs)
    {
        if (_startScale.magnitude == 0)
        {
            Init();
        }
        float width = RectTransformToScreenSpace((RectTransform)transform).width;
        float height = RectTransformToScreenSpace((RectTransform)transform).height;
        transform.localPosition = _startPos + ((nbTabs * Vector3.left * width / 2) + (width / 2 * Vector3.right) + (Vector3.right * index * width)) * _canvas.scaleFactor;
    }

    public void SetScale(int nbTabs)
    {
        if (_startScale.magnitude == 0)
        {
            Init();
        }

        transform.localScale = new Vector3(_startScale.x / nbTabs, _startScale.y, 1) * _canvas.scaleFactor;
        _textField.transform.localScale = new Vector3(1, 1 /(float)nbTabs, 1);
    }

    public override void Init()
    {
        base.Init();
        _startPos = transform.localPosition;
        _startScale = transform.localScale;
        _textField.text = _panel.name;
    }

    public bool Focus { get { return _focus; } }
}
