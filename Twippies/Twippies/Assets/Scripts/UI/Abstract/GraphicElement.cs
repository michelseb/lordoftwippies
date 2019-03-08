using UnityEngine;
using UnityEngine.UI;

public abstract class GraphicElement : MonoBehaviour {

    protected Image _image;
    protected bool _active;
    protected bool _visible;
    protected Canvas _canvas;
    protected UIManager _uiManager;

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
        _image = GetComponent<Image>();
        _canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        _uiManager = UIManager.Instance;
        SetActive(_active);
    }

    protected Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        float x = transform.position.x + transform.anchoredPosition.x;
        float y = Screen.height - transform.position.y - transform.anchoredPosition.y;

        return new Rect(x, y, size.x, size.y);
    }

    public Image Image { get { return _image; } }
    public bool Active { get { return _active; } }
}
