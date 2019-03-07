using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainPanel : GraphicElement {

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

    public void GenerateStatPanels(string type, ManageableObjet obj)
    {
        Type t = obj.GetType();
        StatPanel statPanel = StatPanels.FirstOrDefault(x => x.Type == type);
        if (statPanel != null)
        {
            t.GetMethod("GenerateStats").Invoke(obj, new object[] { statPanel, statPanel.StatManager, type });
            statPanel.StatManager.GenerateStat<ValueStat>(obj.Type, mainStat: true, name: "Amount").Populate(0, 0, 100, "Nombre de " + obj.Type.Split(' ')[0] + "s", true, "Amount");
            UpdateGlobalStat(statPanel, 1);
            Debug.Log("Global stat " + statPanel.Type + " mis à jour");
        }
    }

    public bool SetStatPanelActiveState(bool active, string type)
    {
        StatPanel statPanel = StatPanels.FirstOrDefault(x => x.Type == type);
        if (statPanel != null)
        {
            statPanel.SetActive(active);
            return true;
        }
        return false;
    }

    public void SetAllStatPanelsActiveState(bool active)
    {
        foreach (StatPanel statPanel in StatPanels)
        {
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
