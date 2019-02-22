using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : AerialObjet {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Light _light;

    private bool _on;


    public override void GenerateStats()
    {
        base.GenerateStats();
        _stats.GenerateStat<BoolStat>().Populate(true, "On/Off");
        _stats.GenerateStat<ValueStat>().Populate(1, 0, 10, "brightness", false);
        _stats.GenerateStat<ValueStat>().Populate(2, 0, 20, "rotation speed", false);
    }
    protected override void Awake()
    {
        base.Awake();
        _type = "Woaaaa (Soleil)";
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
        _on = _stats.StatToBool(_stats.StatsList[3]).Value;
        _speed = _stats.StatToValue(_stats.StatsList[5]).Value;
        _light.enabled = _on;
        _light.intensity = _stats.StatToValue(_stats.StatsList[4]).Value;
    }

    public float Speed
    {
        get
        {
            return _speed;
        }
    }
}
