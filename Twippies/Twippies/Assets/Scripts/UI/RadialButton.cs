using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialButton : RadialElement
{
    protected override void Awake()
    {
        _animator = transform.GetComponentInParent<Animator>();
    }
    protected void Start()
    {
        _initSize = transform.parent.transform.localScale;
        _scaledSize = _initSize * 2;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        Select();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!Selected)
        {
            DeSelect();
        }
    }

    public override void Select()
    {
        Selected = true;
        transform.parent.transform.localScale = _scaledSize;
        foreach (UserAction action in RadialPanel.Instance.UserActions)
        {
            if (action.Button == this)
                continue;
            action.Button.DeSelect();
        }
        _subMenu.Open();
    }

    public override void DeSelect()
    {
        transform.parent.transform.localScale = _initSize;
        Close();
        Selected = false;
    }
}
