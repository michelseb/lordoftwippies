using UnityEngine;
using UnityEngine.UI;

public class Tab : GraphicElement
{

    [SerializeField]
    private StatPanel _panel;
    private Vector3 _startPos;
    private Vector3 _startScale;
    private bool _focus;

    protected virtual void Start()
    {
        _startPos = transform.localPosition;
        _startScale = transform.localScale;
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        _panel.SetActive(active);
    }

    public void SetFocus(bool focus)
    {
        if (focus != _focus)
        {
            if (focus)
            {
                foreach (StatPanel panel in _panel.MainStatPanel.StatPanels)
                {
                    if (panel != this)
                    {
                        panel.Tab.SetFocus(false);
                    }
                }
                transform.localScale += Vector3.one * .1f;
                _panel.transform.SetAsLastSibling();
                _image.color = _panel.StatManager.Color;
                _panel.SetVisible(true);

            }
            else
            {
                transform.localScale = _startScale;
                _image.color = _panel.StatManager.Color * .8f;
                _panel.transform.SetSiblingIndex(_panel.MainStatPanel.StatPanels.IndexOf(_panel));
                _panel.SetVisible(false);
            }
            _focus = focus;
        }
    }

    public Vector3 StartPos { get { return _startPos; } }
    public Vector3 StartScale { get { return _startScale; } }
}
