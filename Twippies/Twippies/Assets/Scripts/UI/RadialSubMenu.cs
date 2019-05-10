using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RadialSubMenu : RadialElement
{
    private List<IRadial> _elements;
    private RadialLayout _radialLayout;
    public List<IRadial> Elements { get { if (_elements == null) _elements = new List<IRadial>(); return _elements; } }
    public RadialLayout RadialLayout { get { if (_radialLayout == null) _radialLayout = GetComponent<RadialLayout>(); return _radialLayout; } }

    public void Arrange()
    {
        RadialLayout.MaxAngle = 20 * (Elements.Count - 1);
        RadialLayout.StartAngle = 90 - 10 * (Elements.Count - 1);
    }

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
