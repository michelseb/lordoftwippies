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
        _initSize = transform.parent.transform.localScale;
        _scaledSize = _initSize * 2;
    }

    private void Update()
    {
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
        Selected = true;
        transform.parent.transform.localScale = _scaledSize;
        //foreach (ProgressButtonStat stat in StatManager.Instance.UserActions)
        //{
        //    if (stat.Button == this)
        //        continue;
        //    action.Button.DeSelect();
        //}
    }

    public void DeSelect()
    {
        transform.parent.transform.localScale = _initSize;
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
}
