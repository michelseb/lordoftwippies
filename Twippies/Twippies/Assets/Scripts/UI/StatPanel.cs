using UnityEngine;

public class StatPanel : GraphicElement {

    protected MainPanel _mainStatPanel;
    protected Tab _tab;
    protected StatManager _statManager;
    private string _type;

    protected override void Awake()
    {
        base.Awake();
        _mainStatPanel = MainPanel.Instance;
        _mainStatPanel.StatPanels.Add(this);
    }

    public MainPanel MainStatPanel { get { return _mainStatPanel; } }
    public Tab Tab { get { return _tab; } }
    public StatManager StatManager { get { return _statManager; } set { _statManager = value; } }
    public string Type { get { return _type; } set { _type = value; } }
}
