using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : AerialObjet {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private Light _light;

    private bool _on;


    protected override void GenerateStats()
    {
        base.GenerateStats();
        _stats.StatsList[3] = new BoolStat(true, "On/Off");
        _stats.StatsList[4] = new ValueStat(1, 0, 10, "brightness", false);
        _stats.StatsList[5] = new ValueStat(2, 0, 20, "rotation speed", false);
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
        
        //_r.constraints = RigidbodyConstraints.FreezePositionX;
        _outline.color = 0;
    }

    protected override void Update()
    {
        base.Update();
        _on = _stats.StatToBool(_stats.StatsList[3]).Value;
        _speed = _stats.StatToValue(_stats.StatsList[5]).Value;
        _light.enabled = _on;
        _light.intensity = _stats.StatToValue(_stats.StatsList[4]).Value;
        if (_on)
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
