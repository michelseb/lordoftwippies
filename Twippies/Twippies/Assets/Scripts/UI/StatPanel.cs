using UnityEngine;

public class StatPanel : GraphicElement {

    protected MainPanel _mainStatPanel;
    protected Tab _tab;

    protected void Awake()
    {
        _mainStatPanel = MainPanel.Instance;
        _mainStatPanel.StatPanels.Add(this);
    }

    public MainPanel MainStatPanel { get { return _mainStatPanel; } }
    public Tab Tab { get { return _tab; } }
}
