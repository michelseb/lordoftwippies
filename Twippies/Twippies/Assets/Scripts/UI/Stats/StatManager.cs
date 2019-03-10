using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour {
    [SerializeField]
    private List<Stat> _statsList;
    private StatPanel _statPanel;
    private UIContent _specificStatsPanel;
    private ObjectGenerator _og;
    [SerializeField]
    private Color _color;

    private void Awake()
    {
        _og = ObjectGenerator.Instance;
        _statsList = new List<Stat>();
        enabled = false;
    }

    public ValueStat StatToValue(Stat s)
    {
        return (ValueStat)s;
    }
    public BoolStat StatToBool(Stat s)
    {
        return (BoolStat)s;
    }
    public LabelStat StatToLabel(Stat s)
    {
        return (LabelStat)s;
    }
    public TextStat StatToText(Stat s)
    {
        return (TextStat)s;
    }
    public ChoiceStat StatToChoice(Stat s)
    {
        return (ChoiceStat)s;
    }

    public void CreateSpecificPanel(Transform parent)
    {
        _specificStatsPanel = Instantiate(_og.SpecificStatPanel, parent);
        _specificStatsPanel.name = "Specific stat panel";
    }

    public T GenerateStat<T>(string statType, bool mainStat = false, string name = "") where T:Stat
    {
        GameObject obj = Instantiate(_og.GetStat<T>(name != ""?name:null), mainStat?_statPanel.transform.Find("Mask").Find("Panel"):_specificStatsPanel.transform.GetChild(0));
        if (mainStat)
        {
            obj.transform.SetAsFirstSibling();
        }
        
        T stat = obj.GetComponent<T>();
        stat.Main = mainStat;
        if (name != "")
        {
            stat.SpecificName = name;
        }
        _statsList.Add(stat);
        return stat;
    }

    public Stat GetStat(string name)
    {
        return _statsList.FirstOrDefault(x => x.SpecificName == name);
    }

    public void SetStatActiveState(Stat stat, bool active)
    {
        stat.gameObject.SetActive(active);
    }

    public void Init()
    {
        _og = ObjectGenerator.Instance;
        _statsList = new List<Stat>();
        enabled = false;
    }

    public List<Stat> StatsList { get { return _statsList; } set { _statsList = value; } }
    public UIContent SpecificStatPanel { get { return _specificStatsPanel; } }
    public Color Color { get { return _color; } set { _color = value; } }
    public StatPanel StatPanel { get { return _statPanel; } set { _statPanel = value; } }
}
