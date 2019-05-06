using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialPanel : RadialElement {

    public List<UserAction> UserActions { get; internal set; }

    private static RadialPanel _instance;
    public static RadialPanel Instance { get { if (_instance == null) _instance = FindObjectOfType<RadialPanel>(); return _instance; } }

    protected override void Awake()
    {
        base.Awake();
        UserActions = new List<UserAction>();
    }

    public void GenerateObjectActions(ManageableObjet obj)
    {
        Type t = obj.GetType();
        t.GetMethod("GenerateActions").Invoke(obj, new object[] { obj });
    }

    public void GenerateStatsForRadialButton(RadialButton button, ManageableObjet obj)
    {
        Type t = obj.GetType();
        if (button != null)
        {
            //button.StatManager.GenerateStat<ProgressButtonStat>(obj.Type, mainStat: true, name: "Amount").Populate(0, 0, 100, "Nombre de " + obj.Type.Split(' ')[0] + "s", true, "Amount");
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
        List<UserAction> actions = UserActions.FindAll(x => x.Type == type);
        if (actions != null && actions.Count > 0)
        {
            foreach (UserAction action in actions)
            {
                action.SetActive(active);
                action.Button.SetActive(active);
            }
            return true;
        }
        return false;
    }

    public void SetAllActionsActiveState(bool active, string exceptionType = null)
    {
        foreach (UserAction action in UserActions)
        {
            if (exceptionType != null && exceptionType == action.Type)
                continue;

            action.SetActive(active);
        }
    }

    public IEnumerator SetAllActionsActiveStateWithDelay(bool active, float delay = 0f)
    {
        foreach (UserAction action in UserActions)
        {
            if (active)
            {
                action.Button.Select();
            }
            else
            {
                action.Button.Selected = false;
                action.Button.DeSelect();
            }
        }
        yield return (delay != 0 ? new WaitForSeconds(delay) : null); 
        foreach (UserAction action in UserActions)
        {
            action.SetActive(active);
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
        StartCoroutine(DeSelect());
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine(DeSelect());
    }


    public override void Select()
    {
    }

    public override IEnumerator DeSelect(float delay = 0)
    {
        foreach (var child in transform.GetComponentsInChildren<RadialButton>(true))
        {
            if (!child.Selected)
            {
                StartCoroutine(child.DeSelect());
            }
        }
        yield return null;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnSelect(BaseEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }
}
