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

    public T GenerateRadialAction<T>(ManageableObjet obj) where T: UserAction
    {
        GameObject buttonObj = Instantiate(ObjectGenerator.Instance.GetActionGO<T>());
        T action = buttonObj.GetComponent<T>();
        action.RadialButton.Parent = _om.ActivePlanet.MainRadial.transform;
        action.RadialButton.Type = obj.Type;
        action.RadialButton.Init();
        action.transform.SetParent(action.RadialButton.Parent);
        action.RadialButton.Image.color = obj.Stats.Color;
        _om.ActivePlanet.MainRadial.Elements.Add(action.RadialButton);
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
            var buttons = _om.ActivePlanet.MainRadial.Elements.FindAll(x => x.Type == type);
            RadialButton button = null;
            foreach (RadialButton butt in buttons.OfType<RadialButton>())
            {
                var action = butt.GetComponentInParent<UserAction>();
                if (action != null) { 
                    if (action.AssociatedAction == stat.AssociatedAction)
                    {
                        button = butt;
                        break;
                    }
                }
            }
            if (button == null)
                continue;
            stat.Parent = _om.ActivePlanet.MainRadial.transform;
            stat.transform.SetParent(stat.Parent, false);
            button.RadialMenu = _om.ActivePlanet.MainRadial;
            button.RadialMenu.Elements.Add(stat);
            stat.RadialButton = button;
            if (stat.Image != null)
            {
                stat.Image.color = button.Image.color;
            }
        }

        foreach (RadialButton button in _om.ActivePlanet.MainRadial.Elements.OfType<RadialButton>())
        {
            button.RadialMenu.Arrange();
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
