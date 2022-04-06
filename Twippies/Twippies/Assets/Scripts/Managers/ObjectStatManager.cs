using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectStatManager : MonoBehaviour
{
    [SerializeField] private List<Stat> _statFactory;
    [SerializeField] private Color _color;

    private ObjectGenerator _objectGenerator;
    private ObjetManager _objectManager;

    private List<Stat> _stats;

    public Color Color { get { return _color; } set { _color = value; } }
    public string Type { get; set; }

    private void Awake()
    {
        _objectGenerator = ObjectGenerator.Instance;
        _objectManager = ObjetManager.Instance;
        _stats = new List<Stat>();
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

    public T GenerateRadialAction<T>(ManageableObjet obj) where T : UserAction
    {
        var action = Instantiate(_objectGenerator.GetAction<T>());

        action.RadialButton.Parent = _objectManager.MainPlanet.MainRadial.transform;
        action.Type = obj.Type;
        action.transform.SetParent(action.RadialButton.Parent, false);
        action.RadialButton.Init();
        action.gameObject.SetActive(action.RadialButton.Active);
        action.SubMenu.SetActive(action.RadialButton.Active);
        action.RadialButton.Image.color = obj.Stats.Color;
        _objectManager.MainPlanet.MainRadial.Elements.Add(action);

        return action;
    }

    public T GenerateWorldStat<T>() where T : Stat
    {
        var stat = Instantiate(ObjectGenerator.Instance.GetStat<T>());
        _stats.Add(stat);

        return stat;
    }

    public void LinkStatsToAction(string type)
    {
        foreach (var stat in _stats)
        {
            var actions = _objectManager.MainPlanet.MainRadial.Elements.FindAll(x => x.Type == type);
            RadialButton button = null;
            foreach (UserAction act in actions)
            {
                if (act.AssociatedAction == stat.AssociatedAction)
                {
                    button = act.RadialButton;
                    button.Type = act.Type;
                    button.SubMenu.Type = button.Type;
                    break;
                }
            }
            if (button == null)
                continue;
            stat.Parent = button.SubMenu.transform;
            stat.transform.SetParent(stat.Parent, false);
            button.SubMenu.Elements.Add(stat);
            stat.RadialButton = button;
            if (stat.Image != null)
            {
                stat.Image.color = button.Image.color;
            }
        }

        foreach (UserAction action in _objectManager.MainPlanet.MainRadial.Elements)
        {
            action.RadialButton.SubMenu?.Arrange();
        }
    }

    public Stat GetStat(string name)
    {
        return _stats.FirstOrDefault(x => x.SpecificName == name);
    }

    public void SetStatActiveState(Stat stat, bool active)
    {
        stat.gameObject.SetActive(active);
    }

    public void Init()
    {
        _objectGenerator = ObjectGenerator.Instance;
        _objectManager = ObjetManager.Instance;
        _stats = new List<Stat>();
    }
}
