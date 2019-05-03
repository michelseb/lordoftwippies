using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProgressButtonStat : Stat, IRadial {

    [SerializeField]
    private TMPro.TextMeshProUGUI _labelField;
    [SerializeField]
    private Image _fillImage;
    public Image FillImage { get { return _fillImage; } }
    public float Value { get; set; }
    public string Label { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }
    public bool ReadOnly { get; set; }

    private void Start()
    {
        _fillImage.fillAmount = Value;
        _initSize = transform.localScale;
        _focusedSize = _initSize;
    }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Label;
        if (ReadOnly)
        {
            _fillImage.fillAmount = Value;
        }
        else
        {
            Value = _fillImage.fillAmount;
        }
    }

    public void Populate(float value, int minValue, int maxValue, string label, bool readOnly, string statName)
    {
        _statType = StatType.Value;
        _name = "progressButton";
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        Label = label;
        ReadOnly = readOnly;
        _specificName = statName;
    }

    public void Open()
    {
    }

    public void Close()
    {
    }

    public void Select()
    {
        _focusedSize = GetCurrentSize();
        Selected = true;
        _controls.FocusedUI = this;
        transform.parent.transform.localScale = _focusedSize;
        //foreach (ProgressButtonStat stat in StatManager.Instance.UserActions)
        //{
        //    if (stat.Button == this)
        //        continue;
        //    action.Button.DeSelect();
        //}
    }

    public IEnumerator DeSelect(float delay = 0)
    {
        //transform.parent.transform.localScale = _initSize;
        yield break;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public void OnSelect(BaseEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!Selected)
        {
            DeSelect();
        }
    }

    protected override Vector3 GetCurrentSize()
    {
        if (Controls.FocusedUI != null)
        {
            if (Controls.FocusedUI == this)
                return _focusedSize;
        }
        return Vector3.ClampMagnitude(Vector3.one * 1 / (_mouseProximity + .01f) * 50, _initSize.magnitude * 10);
    }

    protected override int GetCurrentSortingOrder()
    {
        if (Controls.FocusedUI != null)
        {
            if (Controls.FocusedUI == this)
                return _focusedSortingOrder;
        }
        return Mathf.FloorToInt(1 / (_mouseProximity * 1000 + 1) * 5000);
    }

}
