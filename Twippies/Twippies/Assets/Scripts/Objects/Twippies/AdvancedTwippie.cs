using System.Collections.Generic;
using UnityEngine;

public class AdvancedTwippie : Twippie {

    protected enum AdvancedNeed
    {
        Socialize,
        Partnerize,
        Warmup,
        Cooldown,
        None
    }

    protected List<Ressource> _ressources;
    protected AdvancedNeed _advancedNeed;
    private Twippie _partner;
    protected House _house;

    protected override void Awake()
    {
        base.Awake();
        _type = "Twippie avancé";
        _name = "Twippie qui sait faire des trucs cools !";
    }

    protected override void Start()
    {
        base.Start();
        _partner = null;
        _advancedNeed = AdvancedNeed.None;
        _ressources = new List<Ressource>();
        _house = null;
    }



    protected override GoalType DefineGoal()
    {
        switch (_basicNeed)
        {
            case BasicNeed.Drink:
                return GoalType.Drink;
            case BasicNeed.Eat:
                return GoalType.Eat;
        }
        switch (_advancedNeed)
        {
            case AdvancedNeed.Socialize:
                return GoalType.Socialize;
        }
        return GoalType.Wander;
    }

    protected virtual void Build(GameObject model)
    {

    }

}
