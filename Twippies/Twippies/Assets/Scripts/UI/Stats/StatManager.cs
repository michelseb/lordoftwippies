using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour {
    [SerializeField]
    private List<Stat> _statsList;
    private ObjectGenerator _og;
    private ObjetManager _om;
    
    [SerializeField]
    private Color _color;

    public List<Stat> StatsList { get { return _statsList; } set { _statsList = value; } }
    public Color Color { get { return _color; } set { _color = value; } }
    public string Type { get; set; }

    private void Awake()
    {
        _og = ObjectGenerator.Instance;
        _om = ObjetManager.Instance;
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

    public T GenerateAction<T>(ManageableObjet obj) where T: UserAction
    {
        GameObject actionObj = Instantiate(ObjectGenerator.Instance.GetAction<T>(), RadialPanel.Instance.transform);
        T action = actionObj.GetComponent<T>();
        action.Type = obj.Type;
        action.Init();
        action.Button.Init();
        action.Button.Image.color = obj.Stats.Color;
        RadialPanel.Instance.UserActions.Add(action);
        obj.GenerateStatsForAction(action, this);
        return action;
    }

    public T GenerateWorldStat<T>(UserAction action) where T : Stat
    {
        GameObject obj = Instantiate(ObjectGenerator.Instance.GetStat<T>(), action.SubMenu.transform);
        T stat = obj.GetComponent<T>();
        stat.Init();
        if (stat.Image != null)
        {
            stat.Image.color = action.Button.Image.color;
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
