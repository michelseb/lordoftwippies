using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialButton : RadialElement
{
    [SerializeField]
    private UIContainer _container;
    [SerializeField]
    private TextMeshProUGUI _text;

    public RadialMenu RadialMenu { get; set; }
    public UIContainer Container { get { return _container; } }
    public TextMeshProUGUI Text { get { return _text; } set { _text = value; } }

    protected override void Awake()
    {
        _animator = transform.GetComponentInParent<Animator>();
    }
    protected void Start()
    {
        foreach(var element in transform.parent.transform.GetComponentsInChildren<GraphicElement>(true))
        {
            if (element == this)
                continue;

            if (element.Image == null)
            {
                element.Init();
            }
            if (element.Image != null)
            {
                element.Image.color = new Color(_image.color.r + .2f, _image.color.g + .2f, _image.color.b + .2f, 1);
            }
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
            if (!(child is RadialMenu && child.transform.parent != transform.parent.transform))
            {
                child.SetActive(true);
            }
        }
        foreach (RadialButton button in RadialMenu.Elements)
        {
            if (button == this)
                continue;
            StartCoroutine(button.DeSelect(1f));
        }
        _subMenu.Open();
    }

    public override IEnumerator DeSelect(float delay = 0)
    {
        if (_image != null)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, .2f);
        }
        
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
        if (Controls.FocusedUI == this)
            return _focusedSize;
        foreach (var radial in transform.parent.transform.GetComponentsInChildren<RadialElement>(true))
        {
            if (Controls.FocusedUI == (object)radial)
                return _focusedSize;
        }
        return _initSize;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }
}
