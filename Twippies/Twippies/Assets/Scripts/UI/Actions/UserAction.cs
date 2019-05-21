using UnityEngine;

public class UserAction : MonoBehaviour
{
    [SerializeField]
    private RadialButton _radialButton;
    public RadialButton RadialButton { get { return _radialButton; } }
    public AssociatedAction AssociatedAction { get; set; }
}
