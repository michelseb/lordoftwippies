public class AdvancedTwippie : Twippie {

    protected enum AdvancedNeed
    {
        Socialize,
        Warmup,
        Cooldown,
        None
    }

    protected AdvancedNeed _advancedNeed;

    protected override void Awake()
    {
        base.Awake();
        _type = "Twippie";
        _name = "Twippie avancé";
    }

    protected override void Start()
    {
        base.Start();
        _advancedNeed = AdvancedNeed.None;
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

}
