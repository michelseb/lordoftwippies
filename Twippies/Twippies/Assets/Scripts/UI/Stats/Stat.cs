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

public enum AssociatedAction
{
    Description,
    Modification,
    Add
}

public abstract class Stat : GraphicElement, IRadial {
    protected StatManager _statManager;
    protected StatType _statType;
    [SerializeField]
    protected string _name;
    protected bool _main;
    protected string _specificName;

    public AssociatedAction AssociatedAction { get; set; }
    public string Name { get { return _name; } }
    public bool Main { get { return _main; } set { _main = value; } }
    public string SpecificName { get { return _specificName; } set { _specificName = value; } }
    public bool ReadOnly { get; set; } = false;
    public RadialButton RadialButton { get; set; }

    protected override Vector3 GetCurrentSize()
    {
        if (Controls.FocusedUI == this)
            return _focusedSize;
        return Vector3.ClampMagnitude(Vector3.one * 1 / (_mouseProximity + .01f) * 50, _initSize.magnitude * 10);
    }

    public virtual void Open() { }
    public virtual void Close() { }

    public virtual void Select()
    {
        Selected = true;
        _controls.FocusedUI = this;
        transform.localScale = _focusedSize;
        foreach (var stat in RadialButton.transform.GetComponentsInChildren<Stat>())
        {
            if (stat == this)
                continue;
            StartCoroutine(stat.DeSelect());
        }
    }

    public IEnumerator DeSelect(float delay = 0)
    {
        Selected = false;
        yield break;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public virtual void OnSelect(BaseEventData eventData)
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

    public virtual void OnPointerClick(PointerEventData eventData){ }
    public void Populate() { }
}

