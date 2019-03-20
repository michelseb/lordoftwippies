public enum SkillType
{
    Socialize,
    Partnerize,
    Build,
    None
}

public class Skill {

    public float SkillValue { get; set; }
    public SkillType Type { get; set; }

    public Skill(SkillType skillType, float value)
    {
        Type = skillType;
        SkillValue = value;
    }
}
