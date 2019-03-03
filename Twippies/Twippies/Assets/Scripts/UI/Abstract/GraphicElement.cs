using UnityEngine;
using UnityEngine.UI;

public abstract class GraphicElement : MonoBehaviour {

    protected Image _image;
    protected bool _active;

    protected virtual void Start()
    {
        _image = GetComponent<Image>();
        SetActive(false);
    }

    public virtual void SetActive(bool active)
    {
        _active = active;
        gameObject.SetActive(active);
    }

    public void SetVisible(bool visible)
    {
        _image.enabled = visible;
    }
}
