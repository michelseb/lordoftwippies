using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialButton : RadialElement, IPointerEnterHandler, ISelectHandler, IPointerExitHandler
{
    public string Type { get; set; }
    public UserAction Action { get; internal set; }
    protected override void Awake()
    {
        Action = transform.GetComponentInParent<UserAction>();
        _animator = transform.GetComponentInParent<Animator>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public void OnSelect(BaseEventData eventData)
    {
        foreach(RadialButton button in RadialPanel.Instance.RadialButtons)
        {
            if (button == this)
                continue;
            button.Close();
            button.Selected = false;
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
