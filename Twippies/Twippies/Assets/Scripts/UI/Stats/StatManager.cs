using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour {
    private List<Stat> _statsList;
    private ObjectGenerator _og;
    private ObjetManager _om;
    
    [SerializeField]
    private Color _color;
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
        GameObject actionObj = Instantiate(ObjectGenerator.Instance.GetActionGO<T>(), RadialPanel.Instance.transform);
        T action = actionObj.GetComponent<T>();
        action.Type = obj.Type;
        action.Init();
        action.Button.Init();
        action.Button.Image.color = obj.Stats.Color;
        RadialPanel.Instance.UserActions.Add(action);
        return action;
    }

    public T GenerateWorldStat<T>() where T : Stat
    {
        GameObject obj = Instantiate(ObjectGenerator.Instance.GetStat<T>());
        T stat = obj.GetComponent<T>();
        _statsList.Add(stat);
        return stat;
    }

    public void LinkStatsToAction(string type)
    {
        foreach (var stat in _statsList)
        {
            var action = RadialPanel.Instance.UserActions.FindAll(x => x.Type == type).FirstOrDefault(x => x.AssociatedAction == stat.AssociatedAction);
            if (action == null)
                continue;
            stat.transform.parent = action.SubMenu.transform;
            action.SubMenu.Elements.Add(stat);
            stat.RadialButton = action.Button;
            if (stat.Image != null)
            {
                stat.Image.color = action.Button.Image.color;
            }
        }

        foreach (var action in RadialPanel.Instance.UserActions)
        {
            action.SubMenu.Arrange();
        }
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
        _om = ObjetManager.Instance;
        _statsList = new List<Stat>();
    }
}
