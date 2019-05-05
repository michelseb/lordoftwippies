using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialButton : RadialElement
{
    protected override void Awake()
    {
        _animator = transform.GetComponentInParent<Animator>();
    }
    protected void Start()
    {
        _initSize = transform.parent.transform.localScale;
        _focusedSize = _initSize;
        foreach(var element in transform.GetComponentsInChildren<GraphicElement>())
        {
            element.Image.color = _image.color;
        }
    }

    protected override void Update()
    {
        if (_initSize != Vector3.zero)
        {
            transform.parent.transform.localScale = GetCurrentSize();
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
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
            Close();
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, .2f);
        }
    }

    

    public override void Select()
    {
        Selected = true;
        _controls.FocusedUI = this;
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 1f);
        foreach (var child in transform.parent.transform.GetComponentsInChildren<GraphicElement>(true))
        {
            child.SetActive(true);
        }
        foreach (UserAction action in RadialPanel.Instance.UserActions)
        {
            if (action.Button == this)
                continue;
            StartCoroutine(action.Button.DeSelect(1f));
        }
        _subMenu.Open();
    }

    public override IEnumerator DeSelect(float delay = 0)
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, .2f);
        Close();
        Selected = false;
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        foreach (var child in transform.parent.transform.GetComponentsInChildren<GraphicElement>(true))
        {
            if (child.transform == transform)
                continue;
            child.SetActive(false);
        }
    }

    protected override Vector3 GetCurrentSize()
    {
        if (Controls.FocusedUI != null)
        {
            if (Controls.FocusedUI == this)
                return _focusedSize;
            foreach (var radial in transform.parent.transform.GetComponentsInChildren<IRadial>(true))
            {
                if (Controls.FocusedUI == radial)
                    return _focusedSize;
            }
        }
        return _initSize;
        return Vector3.ClampMagnitude(_initSize + Vector3.one * 1 / (_mouseProximity * 100 + .01f) * 5000, _initSize.magnitude * 2);
    }
}
