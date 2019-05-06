using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum StatType
{
    Label,
    Text,
    Bool,
    Choice,
    Value
}

public abstract class Stat : GraphicElement, IRadial {
    protected StatManager _statManager;
    protected StatType _statType;
    [SerializeField]
    protected string _name;
    protected bool _main;
    protected string _specificName;

    public string Name { get { return _name; } }
    public bool Main { get { return _main; } set { _main = value; } }
    public string SpecificName { get { return _specificName; } set { _specificName = value; } }

    protected override Vector3 GetCurrentSize()
    {
        if (Controls.FocusedUI == this)
            return _focusedSize;
        return Vector3.ClampMagnitude(Vector3.one * 1 / (_mouseProximity + .01f) * 50, _initSize.magnitude * 10);
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
        _controls.FocusedUI = this;
        transform.localScale = _focusedSize;
    }

    public IEnumerator DeSelect(float delay = 0)
    {
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
}

