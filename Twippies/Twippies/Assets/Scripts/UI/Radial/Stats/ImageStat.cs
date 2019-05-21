using UnityEngine;
using UnityEngine.UI;

public class ImageStat : Stat {

    [SerializeField]
    private Image _icon;

    public Image Icon { get { return _icon; } }

    protected override Vector3 GetCurrentSize()
    {
        return _initSize;
    }
}
