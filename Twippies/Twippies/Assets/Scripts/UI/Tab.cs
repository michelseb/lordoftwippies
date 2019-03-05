using UnityEngine;
using UnityEngine.UI;

public class Tab : GraphicElement
{

    [SerializeField]
    private StatPanel _panel;

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        _panel.SetActive(active);
    }

    public void SetFocus(bool focus)
    {
        if (focus)
        {
            foreach(StatPanel panel in _panel.MainStatPanel.StatPanels)
            {
                panel.Tab.SetFocus(false);
            }
            _panel.transform.SetAsLastSibling();
            _image.color = _panel.StatManager.Color;
            _panel.SetVisible(true);
        }
        else
        {
            _image.color = _panel.StatManager.Color * .8f;
            _panel.SetVisible(false);
        }
    }
}
