using UnityEngine;

public class UserAction : GraphicElement {

    [SerializeField]
    private RadialButton _button;
    [SerializeField]
    private RadialSubMenu _subMenu;
    public RadialButton Button { get { return _button; } }
    public RadialSubMenu SubMenu { get { return _subMenu; } }
    public string Type { get; set; }
    public AssociatedAction AssociatedAction { get; set; }
}
