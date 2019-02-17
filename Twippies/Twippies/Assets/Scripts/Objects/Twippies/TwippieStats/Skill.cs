public enum SkillType
{
    Socialize,
    Partnerize,
    Build,
    None
}

public class Skill {

    private SkillType _skillType;
    private float _value;

    public Skill(SkillType skillType, float value)
    {
        _skillType = skillType;
        _value = value;
    }

    public float SkillValue
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
        }
    }
    public SkillType Type
    {
        get
        {
            return _skillType;
        }
        set
        {
            _skillType = value;
        }
    }

}
