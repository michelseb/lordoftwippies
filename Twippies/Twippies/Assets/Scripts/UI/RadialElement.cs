using UnityEngine;
using UnityEngine.EventSystems;

public abstract class RadialElement : GraphicElement, IRadial
{
    [SerializeField]
    protected RadialSubMenu _subMenu;

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Open()
    {
        if (gameObject.activeSelf)
        {
            _animator.ResetTrigger("Close");
            _animator.SetTrigger("Open");
        }
    }
    public virtual void Close()
    {
        if (gameObject.activeSelf)
        {
            _animator.ResetTrigger("Open");
            _animator.SetTrigger("Close");
            if (_subMenu != null)
            {
                _subMenu.Close();
            }
        }
    }
    public abstract void Select();
    public abstract void DeSelect();
    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnSelect(BaseEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);
}
