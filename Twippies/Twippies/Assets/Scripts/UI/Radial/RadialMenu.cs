using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialMenu : RadialElement
{

    private RadialLayout _radialLayout;
    private List<RadialElement> _elements;
    public List<RadialElement> Elements { get { if (_elements == null) _elements = new List<RadialElement>(); return _elements; } }
    public RadialLayout RadialLayout { get { if (_radialLayout == null) _radialLayout = GetComponent<RadialLayout>(); return _radialLayout; } }

    public void Arrange()
    {
        RadialLayout.MaxAngle = 20 * (Elements.Count - 1);
        RadialLayout.StartAngle = 90 - 10 * (Elements.Count - 1);
    }

    public void PopulateStatPanel(ObjectStatManager statManager, object[] objs)
    {
        var stat = statManager.GetStat((string)objs[0]); // objs[0] = statname
        stat.GetType().GetMethod("Populate").Invoke(stat, objs);
    }

    public void SetButtonsActiveState(bool active, string type, bool children = false)
    {
        foreach (RadialElement radial in Elements)
        {
            if (radial.Type != type)
                continue;

            radial.SetActive(active);
            if (!children)
                continue;

            foreach (var rad in radial.GetComponentsInChildren<RadialElement>(true))
            {
                if (!(rad is RadialMenu))
                {
                    rad.SetActive(active);
                }
            }
        }
    }

    public void SetAllButtonsActiveState(bool active, string exceptionType = null)
    {
        foreach (RadialElement radial in Elements)
        {
            if (exceptionType != null && exceptionType == radial.Type)
                continue;

            radial.SetActive(active);
        }
    }

    public IEnumerator SetAllButtonsActiveStateWithDelay(bool active, float delay = 0f)
    {
        foreach (RadialElement element in Elements)
        {
            if (active)
            {
                element.Select();
            }
            else
            {
                element.Selected = false;
                element.DeSelect();
            }
        }

        yield return (delay != 0 ? new WaitForSeconds(delay) : null);

        Elements.ForEach(x => x.SetActive(active));
    }

    public override void Close()
    {
        if (!gameObject.activeSelf)
            return;

        base.Close();
        StartCoroutine(DeSelect());
        transform.SetAsFirstSibling();
    }

    public override void Open()
    {
        base.Open();
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
