using System;
using System.Collections.Generic;
using System.Linq;

public class RadialPanel : RadialElement {

    public List<RadialButton> RadialButtons { get; internal set; }

    private static RadialPanel _instance;
    public static RadialPanel Instance { get { if (_instance == null) _instance = FindObjectOfType<RadialPanel>(); return _instance; } }

    private void Start()
    {
        RadialButtons = FindObjectsOfType<RadialButton>().ToList();
    }

    public void GenerateObjectActions(ManageableObjet obj, StatManager statManager)
    {
        Type t = obj.GetType();
        t.GetMethod("GenerateActions").Invoke(obj, new object[] { statManager });
    }

    public void GenerateStatsForRadialButton(RadialButton button, ManageableObjet obj)
    {
        Type t = obj.GetType();
        if (button != null)
        {
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

    public bool SetActionsActiveState(bool active, string type)
    {
        List<RadialButton> buttons = RadialButtons.FindAll(x => x.Type == type);
        if (buttons != null && buttons.Count > 0)
        {
            foreach (RadialButton button in buttons)
            {
                button.SetActive(active);
            }
            return true;
        }
        return false;
    }

    public void SetAllStatPanelsActiveState(bool active)
    {
        foreach (RadialButton button in RadialButtons)
        {
            button.SetActive(active);
        }
    }

    //public void UpdateGlobalStat(RadialButton button, int value)
    //{
    //    if (button != null)
    //    {
    //        button.StatManager.StatToValue(button.StatManager.GetStat("Amount")).Value += value;
    //    }
    //}

    public override void Close()
    {
        base.Close();
        RadialButtons.ForEach(x => x.Close());
    }

}
