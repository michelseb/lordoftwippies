using UnityEngine;
using UnityEngine.EventSystems;

public class RadialButton : GraphicElement, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Animator _subMenuAnimator;
    public Animator Animator { get; set; }

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

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
        _subMenuAnimator.SetTrigger("Close");
    }
}
