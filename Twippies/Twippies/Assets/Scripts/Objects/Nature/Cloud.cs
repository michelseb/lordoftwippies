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
        base.GenerateStats();
        _stats.StatsList[3] = new BoolStat(true, "Wet/Dry");
    }
    protected override void Awake()
    {
        base.Awake();
        _type = "Vvvvv (Nuage)";
        _name = "Nuage capricieux";
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
        BoolStat active = (BoolStat)_stats.StatsList[3];
        _raining = active.Value;
        
        if (_ps.isPlaying && !_raining)
        {
            _ps.Stop();
        }else if (_raining && !_ps.isPlaying)
        {
            _ps.Play();
        }

        if (_raining)
        {
            if (!_zone.Ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Drink))
            {
                _zone.Ressources.Add(new Ressource(Ressource.RessourceType.Drink, 10));
            }

        }
    }

}
