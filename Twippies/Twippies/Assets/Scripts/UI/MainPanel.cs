using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainPanel : GraphicElement {

    [SerializeField]
    public GameObject TabContainer;
    public List<StatPanel> StatPanels;
    private static MainPanel _instance;
    public static MainPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainPanel>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void GenerateStatsForPanel(StatPanel panel, ManageableObjet obj)
    {
        Type t = obj.GetType();
        if (panel != null)
        {
            t.GetMethod("GenerateStats").Invoke(obj, new object[] { panel });
            //panel.StatManager.GenerateStat<ValueStat>(mainStat: true, name: "Amount").Populate(0, 0, 100, "Nombre de " + obj.Type.Split(' ')[0] + "s", true, "Amount");
            UpdateGlobalStat(panel, 1);
            panel.StatManager.GetStat("Amount").SetActive(false);
        }
    }

    public void PopulateStatPanel(Stat stat, object[] objs)
    {
        Type t = stat.GetType();
        t.GetMethod("Populate").Invoke(stat, objs);
    }

    public bool SetStatPanelActiveState(bool active, string type)
    {
        StatPanel statPanel = StatPanels.FirstOrDefault(x => x.Type == type);
        if (statPanel != null)
        {
            statPanel.Tab.SetActive(active);
            statPanel.SetActive(active);
            return true;
        }
        return false;
    }

    public void SetAllStatPanelsActiveState(bool active)
    {
        foreach (StatPanel statPanel in StatPanels)
        {
            statPanel.StatManager.GetStat("Amount")?.SetActive(active);
            statPanel.Tab.SetActive(active);
            statPanel.SetActive(active);
        }
    }

    public void UpdateGlobalStat(StatPanel statPanel, int value)
    {
        if (statPanel != null)
        {
            statPanel.StatManager.StatToValue(statPanel.StatManager.GetStat("Amount")).Value += value;
        }
    }
}
