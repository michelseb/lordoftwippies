using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RadialButton : RadialElement
{
    [SerializeField] private UIContainer _container;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private EdgeCollider2D _edgeCollider2D;
    private float _size;
    private float _startAngle;
    public UIContainer Container { get { return _container; } }
    public TextMeshProUGUI Text { get { return _text; } set { _text = value; } }

    protected override void Awake()
    {
        _animator = transform.GetComponentInParent<Animator>();
    }
    protected void Start()
    {
        if (transform.parent != null)
        {
            foreach (var element in transform.parent.transform.GetComponentsInChildren<GraphicElement>(true))
            {
                if (element == this)
                    continue;

                if (element.Image == null)
                {
                    element.Init();
                }
                if (element.Image != null && _image != null)
                {
                    element.Image.color = new Color(_image.color.r + .2f, _image.color.g + .2f, _image.color.b + .2f, 1);
                }
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


    public void UpdateSegment(float size, float startAngle)
    {
        var segmentAngle = size * 360;
        _size = size;
        _startAngle = startAngle;

        var arcPoints = new List<Vector2>();
        var angle = 0f;

        if (!Mathf.Approximately(Mathf.Abs(segmentAngle), 360)) arcPoints.Add(Vector2.zero);

        for (var i = 0; i <= segmentAngle; i++)
        {
            var x = Mathf.Sin(Mathf.Deg2Rad * angle) * _rectTransform.rect.width / 2;
            var y = Mathf.Cos(Mathf.Deg2Rad * angle) * _rectTransform.rect.width / 2;

            arcPoints.Add(new Vector2(x, y));

            angle += 1;
        }

        if (!Mathf.Approximately(Mathf.Abs(segmentAngle), 360)) arcPoints.Add(Vector2.zero);

        foreach (Image image in GetComponentsInChildren<Image>(true))
        {
            image.fillAmount = _size;
            image.transform.eulerAngles = new Vector3(0, 0, _startAngle);
        }
        
        _image.fillAmount = _size;
        _image.transform.eulerAngles = new Vector3(0, 0, _startAngle);
        _edgeCollider2D.points = arcPoints.ToArray();
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
        SubMenu.SetActive(true);
        foreach (Stat radial in SubMenu.Elements.OfType<Stat>())
        {
            if (radial.RadialButton == this)
                continue;
            StartCoroutine(radial.RadialButton.DeSelect(1f));
        }
        SubMenu.Open();
    }

    public override IEnumerator DeSelect(float delay = 0)
    {
        if (_image != null)
        {
            _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, .2f);
        }
        
        SubMenu.Close();
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
