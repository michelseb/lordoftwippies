public enum NeedType
{
    Eat,
    Drink,
    Sleep,
    Warmup,
    Cooldown,
    None
}

public class Need {

    private NeedType _needType;

    public Need(NeedType needType)
    {
        _needType = needType;
    }

    public NeedType Type
    {
        get
        {
            return _needType;
        }
    }
}
