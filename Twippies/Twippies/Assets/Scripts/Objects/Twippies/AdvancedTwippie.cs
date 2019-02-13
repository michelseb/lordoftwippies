using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedTwippie : Twippie {

    protected enum AdvancedNeed
    {
        Socialize,
        Partnerize,
        Build,
        None
    }
    protected float[] advancedNeedSensibilities;
    protected List<Ressource> _ressources;
    protected AdvancedNeed _advancedNeed;
    private Twippie _partner;
    protected House _house;
    protected Coroutine _building;

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
            case AdvancedNeed.Build:
                return GoalType.Build;
        }
        return GoalType.Wander;
    }

    private IEnumerator CheckAdvancedNeeds()
    {
        while (true)
        {
            if ()
            yield return new WaitForSeconds(3);
        }

    }


    protected virtual IEnumerator Build(GameObject model)
    {
        yield return null;
    }

    public List<Ressource> Ressources
    {
        get
        {
            return _ressources;
        }
        set
        {
            _ressources = value;
        }
    }
    
    private int SelectAdvancedNeed(float[] sensibilities)
    {
        List<float>
        for (int a = 0; a < sensibilities.Length; a++)
        {

        }
    }

    private void SetSensibilities()
    {

    }

}
