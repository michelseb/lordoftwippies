using UnityEngine;

public class UserAction : GraphicElement {

    [SerializeField]
    private RadialButton _button;
    public RadialButton Button { get { return _button; } }
    public string Type { get; set; }
}
