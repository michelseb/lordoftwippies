using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : AerialObjet {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Light _light;

    [SerializeField]
    private bool _on;

    public override void GenerateStatsForAction(UserAction action, StatManager statManager)
    {
        base.GenerateStatsForAction(action, statManager);
        var subMenu = action.SubMenu;
        statManager.GenerateWorldStat<BoolStat>(action).Populate(true, "On/Off", "Active");
        statManager.GenerateWorldStat<ValueStat>(action).Populate(1, 0, 10, "Brightness", false, "Strength");
        statManager.GenerateWorldStat<ValueStat>(action).Populate(2, 0, 20, "Rotation speed", false, "Speed");
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Active"), new object[] { true, "On/Off", "Active" });
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Strength"), new object[] { 1, 0, 10, "Brightness", false, "Strength" });
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Speed"), new object[] { 2, 0, 20, "Rotation speed", false, "Speed" });
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
        _outline.color = 0;
    }

    protected override void Update()
    {
        base.Update();
        if (_on)
            transform.Translate(_speed * Time.deltaTime, 0, 0);
    }

    protected override Vector3 SetCurrentSize()
    {
        return _initSize;
    }

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
