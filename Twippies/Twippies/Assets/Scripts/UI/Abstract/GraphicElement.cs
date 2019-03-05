using UnityEngine;
using UnityEngine.UI;

public abstract class GraphicElement : MonoBehaviour {

    protected Image _image;
    protected bool _active;
    protected bool _visible;

    protected virtual void Awake()
    {
        _image = GetComponent<Image>();
    }

    public virtual void SetActive(bool active)
    {
        _active = active;
        gameObject.SetActive(active);
    }

    public void SetVisible(bool visible)
    {
        _image.enabled = visible;
        _visible = visible;
    }

    public void Init()
    {
        _image = GetComponent<Image>();
        SetActive(false);
    }

    public Image Image { get { return _image; } }
}
