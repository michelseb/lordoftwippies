using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour {

    [SerializeField]
    private List<Stat> _statsList;
    private UIContent _specificStatsPanel;
    private ManageableObjet _mObjet;
    private ObjectGenerator _og;

    private void Awake()
    {
        _mObjet = GetComponent<ManageableObjet>();
        _og = ObjectGenerator.Instance;
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

    public T GenerateStat<T>(ManageableObjet owner = null, bool mainStat = false, string statType = "") where T:Stat
    {
        
        GameObject obj = Instantiate(_og.GetStat<T>(statType != ""?statType:null), mainStat?_specificStatsPanel.transform.parent:_specificStatsPanel.transform.GetChild(0));
        if (mainStat)
        {
            obj.transform.SetAsFirstSibling();
        }
        
        T stat = obj.GetComponent<T>();
        stat.Main = mainStat;
        if (statType != "")
        {
            stat.SpecificType = statType;
        }
        if (owner != null)
        {
            stat.ManageableObjet = owner;
        }
        StatsList.Add(stat);
        SetStatActiveState(stat, false);
        return stat;
    }

    public void SetStatsActiveState(bool active)
    {
        if (_statsList != null && _statsList.Count > 0)
        {
            if (_statsList.Exists(x=>x.Main == true))
            {
                _specificStatsPanel.gameObject.SetActive(active);
            }
            foreach (Stat stat in _statsList)
            {
                stat.gameObject.SetActive(active);
            }
        }
    }

    public void SetStatActiveState(Stat stat, bool active)
    {
        stat.gameObject.SetActive(active);
    }

    public List<Stat> StatsList
    {
        get { return _statsList; }
        set { _statsList = value; }
    }

    public UIContent SpecificStatPanel
    {
        get { return _specificStatsPanel; }
    }
}
