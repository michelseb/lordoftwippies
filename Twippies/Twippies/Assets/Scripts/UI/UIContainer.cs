using UnityEngine;
using UnityEngine.UI;

public class UIContainer : GraphicElement {

    [SerializeField]
    private Image _icon;

    public Image Icon { get { return _icon; } }

    public override void SetActive(bool active)
    {
        return;
    }
}
