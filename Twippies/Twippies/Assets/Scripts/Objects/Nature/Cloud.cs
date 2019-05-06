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

    public override void GenerateStatsForAction(UserAction action, StatManager statManager)
    {
        base.GenerateStatsForAction(action, statManager);
        var subMenu = action.SubMenu;
        statManager.GenerateWorldStat<BoolStat>(action).Populate(true, "Wet/Dry", "Active");
        statManager.GenerateWorldStat<BoolStat>(action).Populate(true, "Auto", "Auto");
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Active"), new object[] { true, "Wet/Dry", "Active" });
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Auto"), new object[] { true, "Auto", "Auto" });
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        BoolStat active = (BoolStat)_stats.GetStat("Active");
        BoolStat auto = (BoolStat)_stats.GetStat("Auto");
        _raining = active.Value;
        _auto = auto.Value;

    }

    protected override void Awake()
    {
        base.Awake();
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
