using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : AerialObjet {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Light _light;

    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[5];
        _stats.StatsList[0] = new LabelStat("Woaaaa (Soleil)");
        _stats.StatsList[1] = new TextStat("Soleil", 20);
        _stats.StatsList[2] = new BoolStat(true, "On/Off");
        _stats.StatsList[3] = new ValueStat(1, 0, 10, "brightness", false);
        _stats.StatsList[4] = new ValueStat(5, 0, 50, "rotation speed", false);
    }

    protected override void Start()
    {
        base.Start();
        //_r.constraints = RigidbodyConstraints.FreezePositionX;
        _outline.color = 0;
    }

    protected override void Update()
    {
        base.Update();
        _speed = _stats.StatToValue(_stats.StatsList[4]).Value;
        _light.enabled = _stats.StatToBool(_stats.StatsList[2]).Value;
        _light.intensity = _stats.StatToValue(_stats.StatsList[3]).Value;
        transform.Translate(_speed * Time.deltaTime, 0, 0);
    }

    public float Speed
    {
        get
        {
            return _speed;
        }
    }
}
