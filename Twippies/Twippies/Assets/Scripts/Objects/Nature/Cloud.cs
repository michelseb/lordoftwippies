using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : AerialObjet {

    [SerializeField]
    private bool _raining;
    [SerializeField]
    private ParticleSystem _ps;

    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[3];
        _stats.StatsList[0] = new LabelStat("Vvvvv (Nuage)");
        _stats.StatsList[1] = new TextStat("Nuage capricieux", 20);
        _stats.StatsList[2] = new BoolStat(true, "Wet/Dry");
    }

    protected override void Start()
    {
        base.Start();
        _outline.color = 4;
        _waterCost = 3;
    }

    protected override void Update()
    {
        base.Update();
        BoolStat active = (BoolStat)_stats.StatsList[2];
        _raining = active.Value;
        
        if (_ps.isPlaying && !_raining)
        {
            _ps.Stop();
        }else if (_raining && !_ps.isPlaying)
        {
            _ps.Play();
        }
    }

}
