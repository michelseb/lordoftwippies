using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour {
    [SerializeField]
    private List<Stat> _statsList;
    private ObjectGenerator _og;
    [SerializeField]
    private Color _color;

    public List<Stat> StatsList { get { return _statsList; } set { _statsList = value; } }
    public Color Color { get { return _color; } set { _color = value; } }
    public UIContent SpecificStatPanel { get; private set; }
    public StatPanel StatPanel { get; set; }

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
        SpecificStatPanel = Instantiate(_og.SpecificStatPanel, parent);
        SpecificStatPanel.name = "Specific stat panel";
    }
    public T GenerateAction<T>(ManageableObjet obj) where T: UserAction
    {
        GameObject actionObj = Instantiate(_og.GetAction<T>(), RadialPanel.Instance.transform);
        T action = actionObj.GetComponent<T>();
        action.Type = obj.Type;
        action.Init();
        action.Button.Init();
        action.Button.Image.color = obj.Stats.Color;
        RadialPanel.Instance.UserActions.Add(action);
        obj.GenerateStatsForAction(action, this);
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

    public T GenerateWorldStat<T>(RadialSubMenu subMenu) where T : Stat
    {
        GameObject obj = Instantiate(_og.GetStat<T>(), subMenu.transform);
        T stat = obj.GetComponent<T>();
        //stat.Fill.color = button.Image.color;
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
