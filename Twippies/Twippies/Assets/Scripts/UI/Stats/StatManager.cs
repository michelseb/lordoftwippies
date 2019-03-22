using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour {
    [SerializeField]
    private List<Stat> _statsList;
    [SerializeField]
    private List<UserAction> _actions;
    private ObjectGenerator _og;
    [SerializeField]
    private Color _color;

    public List<Stat> StatsList { get { return _statsList; } set { _statsList = value; } }
    public List<UserAction> Actions { get { return _actions; } set { _actions = value; } }
    public Color Color { get { return _color; } set { _color = value; } }
    public UIContent SpecificStatPanel { get; private set; }
    public StatPanel StatPanel { get; set; }

    private void Awake()
    {
        _og = ObjectGenerator.Instance;
        _statsList = new List<Stat>();
        _actions = new List<UserAction>();
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
        SpecificStatPanel = Instantiate(_og.SpecificStatPanel, parent);
        SpecificStatPanel.name = "Specific stat panel";
    }
    public T GenerateAction<T>() where T: UserAction
    {
        GameObject actionObj = Instantiate(_og.GetAction<T>(), RadialPanel.Instance.transform);
        T action = actionObj.GetComponent<T>();
        _actions.Add(action);
        return action;
    }
    public T GenerateStat<T>(bool mainStat = false, string name = "") where T:Stat
    {
        GameObject obj = Instantiate(_og.GetStat<T>(name != ""?name:null), mainStat?StatPanel.transform.Find("Mask").Find("Panel"):SpecificStatPanel.Content.transform);
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
}
