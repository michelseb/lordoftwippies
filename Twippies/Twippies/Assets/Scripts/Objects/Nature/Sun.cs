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


    public override void GenerateStats()
    {
        base.GenerateStats();
        _stats.GenerateStat<BoolStat>(this).Populate(true, "On/Off", "Active");
        _stats.GenerateStat<ValueStat>(this).Populate(1, 0, 10, "Brightness", false, "Strength");
        _stats.GenerateStat<ValueStat>(this).Populate(2, 0, 20, "Rotation speed", false, "Speed");
    }
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

    public float Speed
    {
        get
        {
            return _speed;
        }
    }
}
