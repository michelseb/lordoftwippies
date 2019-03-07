using UnityEngine;
using UnityEngine.UI;

public class StatPanel : GraphicElement {

    protected MainPanel _mainStatPanel;
    [SerializeField]
    protected Tab _tab;
    protected StatManager _statManager;
    private string _type;

    public override void SetVisible(bool visible)
    {
        base.SetVisible(visible);
        var graphics = transform.GetComponentsInChildren<GraphicElement>(true);
        for (int a = 0; a < graphics.Length; a++)
        {
            if (!(graphics[a] is Tab) && !(graphics[a] is StatPanel))
            {
                graphics[a].SetActive(visible);
            }
        }
    }

    public override void Init()
    {
        base.Init();
        _mainStatPanel = MainPanel.Instance;
        _mainStatPanel.StatPanels.Add(this);
    }

    public MainPanel MainStatPanel { get { return _mainStatPanel; } }
    public Tab Tab { get { return _tab; } }
    public StatManager StatManager { get { return _statManager; } set { _statManager = value; } }
    public string Type { get { return _type; } set { _type = value; } }
}
