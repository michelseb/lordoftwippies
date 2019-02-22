﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : AerialObjet {

    [SerializeField]
    private bool _raining;
    private bool _auto;
    private Coroutine _isAuto;
    [SerializeField]
    private ParticleSystem _ps;


    public override void GenerateStats()
    {
        base.GenerateStats();
        _stats.GenerateStat<BoolStat>().Populate(true, "Wet/Dry");
        _stats.GenerateStat<BoolStat>().Populate(true, "Auto");
    }
    protected override void UpdateStats()
    {
        base.UpdateStats();
        BoolStat active = (BoolStat)_stats.StatsList[3];
        BoolStat auto = (BoolStat)_stats.StatsList[4];
        _raining = active.Value;
        _auto = auto.Value;

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
    }

    protected override void Update()
    {
        base.Update();
        

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

        if (_auto)
        {
            if (_isAuto == null)
            {
                _isAuto = StartCoroutine(AutoRain(10));
            }
        }
    }


    private IEnumerator AutoRain(float seconds)
    {
        while (true)
        {
            if (!_auto)
            {
                _isAuto = null;
                yield break;
            }
            
            _raining = CoinFlip();
            yield return new WaitForSeconds(seconds / _timeReference);
        }
    }
}
