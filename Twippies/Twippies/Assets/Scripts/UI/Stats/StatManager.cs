using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour {

    [SerializeField]
    private int _nbStats;

    [SerializeField]
    private List<Stat> _statsList;

    private UIContent _panel;
    private ManageableObjet _mObjet;
    private ObjectGenerator _og;

    public int NbStats
    {
        get
        {
            return _nbStats;
        }

        set
        {
            _nbStats = value;
        }
    }

    public List<Stat> StatsList
    {
        get
        {
            return _statsList;
        }

        set
        {
            _statsList = value;
        }
    }

    private void Awake()
    {
        _mObjet = GetComponent<ManageableObjet>();
        _og = ObjectGenerator.Instance;
        _panel = UIContent.Instance;
    }

    
    //private void OnGUI()
    //{
    //    GUI.depth = 1;
    //    //GUI.(new Rect(20, 20 + (Screen.height * 2 / 3), 100, 100), _mObjet.Icon);
    //    for (int a = 0; a < _statsList.Count; a++)
    //    {
    //        Rect r = new Rect(Screen.width/5, 10+(Screen.height * 1/2)+(a * (Screen.height / 2) / _statsList.Count), Screen.width/3, (Screen.height/2)/_statsList.Count);
    //        if (_statsList[a] is TextStat)
    //        {
    //            TextStat s = (TextStat)_statsList[a];
    //            s.Value = GUI.TextField(r, s.Value);
    //        }
    //        else if (_statsList[a] is LabelStat)
    //        {
    //            LabelStat s = (LabelStat)_statsList[a];
    //            GUI.Label(r, s.Value);
    //        }
    //        else if (_statsList[a] is ValueStat)
    //        {
    //            ValueStat s = (ValueStat)_statsList[a];
    //            GUI.Label(r, s.Label);
    //            r.position += new Vector2(100, 5);
    //            if (s.ReadOnly)
    //            {
    //                GUI.HorizontalSlider(r, s.Value, s.MinValue, s.MaxValue);
    //            }
    //            else
    //            {
    //                s.Value = GUI.HorizontalSlider(r, s.Value, s.MinValue, s.MaxValue);
    //            }
    //        }
    //        else if (_statsList[a] is BoolStat)
    //        {
    //            BoolStat s = (BoolStat)_statsList[a];
    //            s.Value = GUI.Toggle(r, s.Value, s.Label);
    //        }
    //        else if (_statsList[a] is ChoiceStat)
    //        {
    //            r.size *= 2;
    //            ChoiceStat s = (ChoiceStat)_statsList[a];
    //            s.Value = GUI.SelectionGrid(r, s.Value, s.Values, 4);
    //        }
    //    }
    //}

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

    public T GenerateStat<T>(ManageableObjet owner, bool mainStat = false, string statType = "") where T:Stat
    {
        GameObject obj = Instantiate(_og.GetStat<T>(statType != ""?statType:null), mainStat?_panel.transform.parent.parent.transform:_panel.transform);
        if (mainStat)
        {
            obj.transform.SetAsFirstSibling();
        }
        T stat = obj.GetComponent<T>();
        stat.ManageableObjet = owner;
        _statsList.Add(stat);
        return stat;
    }

    public void SetStatsActiveState(bool active)
    {
        if (_statsList != null && _statsList.Count > 0)
        {
            foreach (Stat stat in _statsList)
            {
                stat.gameObject.SetActive(active);
            }
        }
    }
}
