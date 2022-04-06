using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserAction : RadialElement
{
    [SerializeField]
    private RadialButton _radialButton;
    public RadialButton RadialButton { get { return _radialButton; } }
    public AssociatedAction AssociatedAction { get; set; }

    public override IEnumerator DeSelect(float delay = 0)
    {
        yield return null;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
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

    protected override void Awake()
    {
        AssociatedAction = AssociatedAction.Description;
    }
}
