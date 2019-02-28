using UnityEngine;

public class UIContent : MonoBehaviour {
    private bool _visible;

    public bool Visible
    {
        get
        {
            return _visible;
        }
        set
        {
            _visible = value;
        }
    }
}
