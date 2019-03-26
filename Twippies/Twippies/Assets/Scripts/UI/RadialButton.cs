using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialButton : RadialElement, IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{
    protected override void Awake()
    {
        _animator = transform.GetComponentInParent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public void OnSelect(BaseEventData eventData)
    {
        foreach(UserAction action in RadialPanel.Instance.UserActions)
        {
            if (action.Button == this)
                continue;
            action.Button.Close();
            action.Button.Selected = false;
        }
        _subMenu.Open();
        Selected = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Selected)
        {
            Close();
        }
    }
}
