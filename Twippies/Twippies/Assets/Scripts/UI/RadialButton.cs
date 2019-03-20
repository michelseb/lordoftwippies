using UnityEngine;
using UnityEngine.EventSystems;

public class RadialButton : GraphicElement, IPointerEnterHandler, IPointerExitHandler
{

    public Animator Animator { get; set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    //private void OnMouseEnter()
    //{
    //    SetOpenState(true);
    //}

    //private void OnMouseExit()
    //{
    //    SetOpenState(false);
    //}

    public void SetOpenState(bool open)
    {
        Animator.SetBool("Open", open);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOpenState(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetOpenState(false);
    }
}
