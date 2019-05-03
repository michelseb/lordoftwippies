using UnityEngine;
using UnityEngine.UI;

public abstract class GraphicElement : MonoBehaviour
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
    protected Canvas _canvas;
    public Canvas Canvas
    {
        get
        {
            _canvas = GetComponent<Canvas>();
            return _canvas;
        }
    }
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
        _focusedSize = _initSize;
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
        if (Canvas != null)
        {
            Canvas.sortingOrder = GetCurrentSortingOrder();
        }
    }

    protected Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        float x = transform.position.x + transform.anchoredPosition.x;
        float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

        return new Rect(x, y, size.x, size.y);
    }

    protected AnimationClip GetAnimationClip(string name)
    {
        if (_animator == null)
            return null;
        var anims = _animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < anims.Length; i++)
        {
            if (anims[i].name == name)
                return anims[i];
        }
        Debug.Log("Anim " + name + " not found for " + gameObject.name);
        return null;
    }

    protected virtual Vector3 GetCurrentSize()
    {
        return _initSize;
    }

    protected virtual float GetCurrentZPos()
    {
        return transform.position.z;
    }

    protected virtual int GetCurrentSortingOrder()
    {
        return 0;
    }
}
