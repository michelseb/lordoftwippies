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
            _panel.transform.SetAsFirstSibling();
            _image.color = Color.white;
            _panel.SetVisible(true);
        }
        else
        {
            _image.color = new Color(.3f, .3f, .3f);
            _panel.SetVisible(false);
        }
    }


}
