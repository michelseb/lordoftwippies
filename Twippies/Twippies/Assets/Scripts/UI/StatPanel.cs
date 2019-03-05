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
        base.SetActive(active);
        if (_active)
        {
            List<StatPanel> panels = _mainStatPanel.StatPanels.FindAll(x => x._active);
            for (int a = 0; a< panels.Count; a++)
            {
                if (panels[a] == this)
                {
                    _tab.transform.position += Vector3.right * a * 12 * _canvas.scaleFactor;
                }
            }
        }
        else
        {
            _tab.transform.position = _mainStatPanel.StatPanels[0].Tab.transform.position;
        }
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
