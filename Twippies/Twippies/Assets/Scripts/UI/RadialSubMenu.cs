using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RadialSubMenu : RadialElement
{
    public List<RadialElement> Elements { get; set; }

    public override IEnumerator DeSelect(float delay = 0)
    {
        yield break;
    }

    public override void Open()
    {
        base.Open();
        transform.SetAsLastSibling();
    }

    public override void Close()
    {
        base.Close();
        transform.SetAsFirstSibling();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }

    public override void OnSelect(BaseEventData eventData)
    {
    }

    public override void Select()
    {
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }
}
