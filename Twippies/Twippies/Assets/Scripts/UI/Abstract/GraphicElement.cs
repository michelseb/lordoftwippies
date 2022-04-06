using UnityEngine;
using UnityEngine.UI;

public abstract class GraphicElement : Objet
{

    protected Controls _controls;
    protected Camera _cam;
    protected Image _image;
    protected bool _active;
    protected bool _visible;
    protected Canvas _screenCanvas, _worldCanvas;
    protected UIManager _uiManager;
    protected Animator _animator;
    protected Vector3 _initSize, _focusedSize;
    protected float _initZ, _focusedZ;
    protected int _focusedSortingOrder;
    public Transform Parent { get; set; }
    public Camera Cam { get { if (_cam == null) _cam = UIManager.Instance.UICam; return _cam; } }
    public Controls Controls { get { if (_controls == null) _controls = Controls.Instance; return _controls; } }
    public bool Selected { get; internal set; }
    public Image Image { get { return _image; } }
    public bool Active { get { return _active; } }
    public Animator Animator { get { return _animator; } }
    protected float _mouseProximity { get { return Vector3.Distance(Cam.WorldToScreenPoint(transform.position), Input.mousePosition); } }
    public virtual void SetActive(bool active)
    {
        _active = active;
        gameObject.SetActive(active);
    }

    public virtual void SetVisible(bool visible)
    {
        if (_image != null)
        {
            _image.enabled = visible;
            _visible = visible;
        }
    }

    public virtual void Init()
    {
        _initSize = transform.localScale;
        _focusedSize = _initSize * 2;
        _controls = Controls.Instance;
        _cam = UIManager.Instance.UICam;
        _image = GetComponent<Image>();
        _screenCanvas = GameObject.Find("ScreenCanvas").GetComponent<Canvas>();
        _worldCanvas = GameObject.Find("WorldCanvas").GetComponent<Canvas>();
        _uiManager = UIManager.Instance;
        SetActive(_active);
    }

    protected virtual void Update()
    {
        if (_initSize != Vector3.zero)
        {
            transform.localScale = GetCurrentSize();
        }
    }

    protected virtual Vector3 GetCurrentSize()
    {
        return _initSize;
    }
}
