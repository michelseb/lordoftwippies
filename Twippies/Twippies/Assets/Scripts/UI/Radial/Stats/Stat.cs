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

public abstract class Stat : RadialElement {

    [SerializeField] protected string _name;

    protected ObjectGenerator _objectGenerator;
    protected StatType _statType;
    protected bool _main;
    protected string _specificName;

    public AssociatedAction AssociatedAction { get; set; }
    public string Name { get { return _name; } }
    public bool Main { get { return _main; } set { _main = value; } }
    public string SpecificName { get { return _specificName; } set { _specificName = value; } }
    public bool ReadOnly { get; set; } = false;
    public RadialButton RadialButton { get; set; }

    protected override void Awake()
    {
        base.Awake();

        _objectGenerator = ObjectGenerator.Instance;
    }

    protected override Vector3 GetCurrentSize()
    {
        if (Controls.FocusedUI == this)
            return _focusedSize;
        var size = Vector3.ClampMagnitude(Vector3.one / (_mouseProximity + .01f) * 20, _initSize.magnitude * 2);
        return size.magnitude > _initSize.magnitude ? size : _initSize;
    }

    public override void Select()
    {
        Selected = true;
        _controls.FocusedUI = this;
        transform.SetParent(null);
        transform.localScale = _focusedSize;
        transform.SetParent(Parent);
        foreach (var stat in RadialButton.transform.GetComponentsInChildren<Stat>())
        {
            if (stat == this)
                continue;

            StartCoroutine(stat.DeSelect());
        }
    }

    public override IEnumerator DeSelect(float delay = 0)
    {
        Selected = false;
        yield break;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Open();
    }
    public override void OnSelect(BaseEventData eventData)
    {
        Select();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!Selected)
        {
            DeSelect();
        }
    }

    public override void OnPointerClick(PointerEventData eventData) { }
}

