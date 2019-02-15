using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : AerialObjet {

    [SerializeField]
    private bool _raining;
    private bool _auto;
    private Coroutine _isAuto;
    [SerializeField]
    private ParticleSystem _ps;


    protected override void GenerateStats()
    {
        base.GenerateStats();
        _stats.StatsList[3] = new BoolStat(true, "Wet/Dry");
        _stats.StatsList[4] = new BoolStat(true, "Auto");
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
        BoolStat active = (BoolStat)_stats.StatsList[3];
        BoolStat auto = (BoolStat)_stats.StatsList[4];
        _raining = active.Value;
        _auto = auto.Value;

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
            
            BoolStat active = (BoolStat)_stats.StatsList[3];
            active.Value = CoinFlip();
            _stats.StatsList[3] = active;
            yield return new WaitForSeconds(seconds / _timeReference);
        }
    }
}
