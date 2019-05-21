using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class RadialElement : GraphicElement, IPointerEnterHandler, ISelectHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    protected RadialMenu _subMenu;

    public string Type { get; set; }

    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    public virtual void Open()
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
    public abstract IEnumerator DeSelect(float delay = 0);
    public abstract void OnPointerEnter(PointerEventData eventData);
    public abstract void OnSelect(BaseEventData eventData);
    public abstract void OnPointerExit(PointerEventData eventData);
    public abstract void OnPointerClick(PointerEventData eventData);
}
