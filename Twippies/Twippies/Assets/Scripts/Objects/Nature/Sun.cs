using UnityEngine;

public class Sun : AerialObjet {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Light _light;

    [SerializeField]
    private bool _on;

    public override void GenerateActions()
    {
        Stats.GenerateRadialAction<ModificationAction>(this);
        base.GenerateActions();
    }

    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<BoolStat>().Populate("Active", true, "On/Off", false);
        Stats.GenerateWorldStat<ValueStat>().Populate("Strength", 1, 0, 10, "Brightness", false);
        Stats.GenerateWorldStat<ValueStat>().Populate("Speed", 2, 0, 20, "Rotation speed", false);
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Active", true, "On/Off", false });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Strength", 1, 0, 10, "Brightness", false });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Speed", 2, 0, 20, "Rotation speed", false });
    }
    public float Speed { get { return _speed; } }
    protected override void Awake()
    {
        base.Awake();
        _name = "Soleil";
    }

    protected override void Start()
    {
        base.Start();
        _outline.Color = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (_on)
            transform.Translate(_speed * Time.deltaTime, 0, 0);
    }

    //protected override Vector3 SetCurrentSize()
    //{
    //    return _initSize;
    //}

    protected override void OnMouseEnter()
    {
        _outline.enabled = true;
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        _on = _stats.StatToBool(_stats.GetStat("Active")).Value;
        if (_on)
        {
            _speed = _stats.StatToValue(_stats.GetStat("Speed")).Value;
        }
        else
        {
            _speed = 0;
        }
        _light.enabled = _on;
        _light.intensity = _stats.StatToValue(_stats.GetStat("Strength")).Value;
    }

}
