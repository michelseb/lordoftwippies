using UnityEngine;

public class Mask : MonoBehaviour {

    [SerializeField]
    private RectTransform _reference;
    private RectTransform _rectTransform;
    private Vector3 _initPos;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        _rectTransform.sizeDelta = _reference.localScale * 8;
    }

}
