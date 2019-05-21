using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialMenu : RadialElement {

    private RadialLayout _radialLayout;
    private List<RadialElement> _elements;
    public List<RadialElement> Elements { get { if (_elements == null) _elements = new List<RadialElement>(); return _elements; } }
    public RadialLayout RadialLayout { get { if (_radialLayout == null) _radialLayout = GetComponent<RadialLayout>(); return _radialLayout; } }

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

    public void Arrange()
    {
        RadialLayout.MaxAngle = 20 * (Elements.Count - 1);
        RadialLayout.StartAngle = 90 - 10 * (Elements.Count - 1);
    }

    public void PopulateStatPanel(Stat stat, object[] objs)
    {
        Type t = stat.GetType();
        t.GetMethod("Populate").Invoke(stat, objs);
    }

    public void SetButtonsActiveState(bool active, string type)
    {
        foreach (RadialButton button in Elements)
        {
            if (button.Type == type)
            {
                button.SetActive(active);
            }    
        }
    }

    public void SetAllButtonsActiveState(bool active, string exceptionType = null)
    {
        foreach (RadialButton button in Elements)
        {
            if (exceptionType != null && exceptionType == button.Type)
                continue;

            button.SetActive(active);
        }
    }

    public IEnumerator SetAllButtonsActiveStateWithDelay(bool active, float delay = 0f)
    {
        foreach (RadialButton button in Elements)
        {
            if (active)
            {
                button.Select();
            }
            else
            {
                button.Selected = false;
                button.DeSelect();
            }
        }
        yield return (delay != 0 ? new WaitForSeconds(delay) : null); 
        foreach (RadialButton button in Elements)
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
        StartCoroutine(DeSelect());
        transform.SetAsFirstSibling();
    }

    public override void Open()
    {
        base.Open();
        StartCoroutine(DeSelect());
        transform.SetAsLastSibling();
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

    public override void OnPointerClick(PointerEventData eventData)
    {
    }
}
