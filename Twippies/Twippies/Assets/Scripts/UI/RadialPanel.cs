using System;
using UnityEngine;

public class RadialPanel : GraphicElement {

    public Animator Animator { get; set; }
    private static RadialPanel _instance;
    public static RadialPanel Instance { get { if (_instance == null) _instance = FindObjectOfType<RadialPanel>(); return _instance; } }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    public void GenerateStatsForRadialButton(RadialButton button, ManageableObjet obj)
    {
        Type t = obj.GetType();
        if (button != null)
        {
            t.GetMethod("GenerateActions").Invoke(obj, new object[] { button, obj.Type });
            //button.StatManager.GenerateStat<ValueStat>(obj.Type, mainStat: true, name: "Amount").Populate(0, 0, 100, "Nombre de " + obj.Type.Split(' ')[0] + "s", true, "Amount");
            //UpdateGlobalStat(button, 1);
            //button.StatManager.GetStat("Amount").SetActive(false);
        }
    }

    public void PopulateStatPanel(Stat stat, object[] objs)
    {
        Type t = stat.GetType();
        t.GetMethod("Populate").Invoke(stat, objs);
    }

    //public bool SetStatPanelActiveState(bool active, string type)
    //{
    //    StatPanel statPanel = StatPanels.FirstOrDefault(x => x.Type == type);
    //    if (statPanel != null)
    //    {
    //        statPanel.Tab.SetActive(active);
    //        statPanel.SetActive(active);
    //        return true;
    //    }
    //    return false;
    //}

    //public void SetAllStatPanelsActiveState(bool active)
    //{
    //    foreach (StatPanel statPanel in StatPanels)
    //    {
    //        statPanel.StatManager.GetStat("Amount")?.SetActive(active);
    //        statPanel.Tab.SetActive(active);
    //        statPanel.SetActive(active);
    //    }
    //}

    //public void UpdateGlobalStat(RadialButton button, int value)
    //{
    //    if (button != null)
    //    {
    //        button.StatManager.StatToValue(button.StatManager.GetStat("Amount")).Value += value;
    //    }
    //}

}
