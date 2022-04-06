using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueStat : Stat
{
    [SerializeField] private TextMeshProUGUI _labelField;
    [SerializeField] private Image _fillImage;

    public Image FillImage { get { return _fillImage; } }
    public float Value { get; set; }
    public string Label { get; set; }
    public int MinValue { get; set; }
    public int MaxValue { get; set; }

    private void Start()
    {
        _fillImage.fillAmount = Value;
    }

    protected override void Update()
    {
        base.Update();
        _labelField.text = Label;
        if (ReadOnly)
        {
            _fillImage.fillAmount = Value / MaxValue;
        }
        else
        {
            Value = _fillImage.fillAmount * MaxValue;
        }
        if (Selected)
        {
            if (RadialButton.Container.Icon.IsActive())
            {
                RadialButton.Container.Icon.enabled = false;
            }
            RadialButton.Text.text = Value.ToString();
        }
        else
        {
            if (!RadialButton.Container.Icon.IsActive())
            {
                RadialButton.Container.Icon.enabled = true;
                RadialButton.Text.text = string.Empty;
            }
        }
    }

    //protected void LateUpdate()
    //{
    //    transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    //    foreach (Transform child in transform)
    //    {
    //        child.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
    //    }
    //}

    public void Populate(string statName, float value, int minValue, int maxValue, string label, bool readOnly)
    {
        _statType = StatType.Value;
        _name = "progressButton";
        Value = value;
        MinValue = minValue;
        MaxValue = maxValue;
        Label = label;
        ReadOnly = readOnly;
        _specificName = statName;
        if (ReadOnly)
        {
            AssociatedAction = AssociatedAction.Description;
        }
        else
        {
            AssociatedAction = AssociatedAction.Modification;
        }

    }
}
