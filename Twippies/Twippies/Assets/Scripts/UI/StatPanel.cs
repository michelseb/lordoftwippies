using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatPanel : GraphicElement {

    protected MainPanel _mainStatPanel;
    [SerializeField]
    protected Tab _tab;
    protected StatManager _statManager;
    private string _type;

    protected override void Awake()
    {
        base.Awake();
        _mainStatPanel = MainPanel.Instance;
        _mainStatPanel.StatPanels.Add(this);
    }

    public override void SetActive(bool active)
    {
        if (_active != active)
        {
            if (active)
            {
                List<StatPanel> panels = _mainStatPanel.StatPanels.FindAll(x => x._active);
                panels.Add(this);
                _tab.transform.localPosition += Vector3.right * panels.IndexOf(this) * RectTransformToScreenSpace((RectTransform)_tab.transform).width;
            }
            else
            {
                _tab.transform.localPosition = _tab.StartPos;
            }
        }
        base.SetActive(active);
    }

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

    public MainPanel MainStatPanel { get { return _mainStatPanel; } }
    public Tab Tab { get { return _tab; } }
    public StatManager StatManager { get { return _statManager; } set { _statManager = value; } }
    public string Type { get { return _type; } set { _type = value; } }
}
