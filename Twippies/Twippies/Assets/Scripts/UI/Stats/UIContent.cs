using UnityEngine;

public class UIContent : MonoBehaviour {
    private bool _visible;

    private static UIContent _instance;
    public static UIContent Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIContent>();

            return _instance;
        }
    }

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
