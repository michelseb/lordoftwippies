using UnityEngine;

public class Mask : MonoBehaviour {

    [SerializeField]
    private RectTransform _reference;
    private RectTransform _rectTransform;
    private Vector3 _initPos;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initPos = _rectTransform.position;
    }

    private void Update()
    {
        _rectTransform.sizeDelta = _reference.localScale * 8;
        //_rectTransform.position = (_reference.position+_initPos)/2;
    }

}
