﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : MonoBehaviour {

    [SerializeField]
    private int _nbStats;

    [SerializeField]
    private Stat[] _statsList;

    private ManageableObjet _mObjet;

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

    public Stat[] StatsList
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
    }

    
    private void OnGUI()
    {
        GUI.depth = 1;
        //GUI.(new Rect(20, 20 + (Screen.height * 2 / 3), 100, 100), _mObjet.Icon);
        for (int a = 0; a < _statsList.Length; a++)
        {
            Rect r = new Rect(Screen.width/5, 10+(Screen.height * 1/2)+(a * (Screen.height / 2) / _statsList.Length), Screen.width/3, (Screen.height/2)/_statsList.Length);
            if (_statsList[a] is TextStat)
            {
                TextStat s = (TextStat)_statsList[a];
                s.Value = GUI.TextField(r, s.Value);
            }
            else if (_statsList[a] is LabelStat)
            {
                LabelStat s = (LabelStat)_statsList[a];
                GUI.Label(r, s.Value);
            }
            else if (_statsList[a] is ValueStat)
            {
                ValueStat s = (ValueStat)_statsList[a];
                GUI.Label(r, s.Label);
                r.position += new Vector2(100, 5);
                if (s.ReadOnly)
                {
                    GUI.HorizontalSlider(r, s.Value, s.MinValue, s.MaxValue);
                }
                else
                {
                    s.Value = GUI.HorizontalSlider(r, s.Value, s.MinValue, s.MaxValue);
                }
            }
            else if (_statsList[a] is BoolStat)
            {
                BoolStat s = (BoolStat)_statsList[a];
                s.Value = GUI.Toggle(r, s.Value, s.Label);
            }
            else if (_statsList[a] is ChoiceStat)
            {
                r.size *= 2;
                ChoiceStat s = (ChoiceStat)_statsList[a];
                s.Value = GUI.SelectionGrid(r, s.Value, s.Values, 4);
            }
        }
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
}
