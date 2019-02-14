using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedTwippie : Twippie {

    public struct Skill
    {
        public SkillType SkillType;
        public float SkillValue;
        public Skill(SkillType skillType, float skillValue)
        {
            SkillType = skillType;
            SkillValue = skillValue;
        }
    }

    public enum SkillType
    {
        Socialize,
        Partnerize,
        Build,
        None
    }
    protected float[] advancedNeedSensibilities;
    protected List<Ressource> _ressources;
    private Twippie _partner;
    protected House _house;
    protected Coroutine _building, _collecting;
    protected SkillType _advancedNeed;
    protected Skill[] _skills;

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
        _advancedNeed = SkillType.None;
        SetSensibilities();
        _ressources = new List<Ressource> { new Ressource(Ressource.RessourceType.Drink, 0), new Ressource(Ressource.RessourceType.Food, 0) };
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
            case SkillType.Socialize:
                return GoalType.Socialize;
            case SkillType.Build:
                if (_og.GetObjects<IBuildable>().FirstOrDefault(x=>x.WoodCost <= _ressources[1].quantity) != null)
                {
                    return GoalType.Build;
                }
                else
                {
                    return GoalType.Collect;
                }
                
        }
        return GoalType.Wander;
    }

    protected override void OnStateChange()
    {
        base.OnStateChange();
        switch (_state)
        {
            case State.Building:
                if (_building == null)
                {
                    ManageableObjet obj = _og.GetObjects<IBuildable>().FirstOrDefault(x => x.WoodCost <= _ressources[1].quantity);
                    if (obj != null)
                    {
                        GameObject house = Instantiate(obj.gameObject, _arrival.FinishZone.Center, Quaternion.identity);
                        obj = house.GetComponent<ManageableObjet>();
                        obj.CurrentSize = Vector3.zero;
                        var buildable = (IBuildable)obj;
                        _building = StartCoroutine(buildable.Build());
                    }
                    else
                    {
                        SetDestination(DefineGoal());
                    }
                }
                break;
            case State.Collecting:
                if (_collecting == null)
                {
                    Ressource ressource = _arrival.FinishZone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Food);
                    if (ressource != null)
                    {
                        if (ressource.consumableObject != null)
                        {
                            if (ressource.consumableObject is ICollectable)
                            {
                                ICollectable collectable = (ICollectable)ressource.consumableObject;
                                _collecting = StartCoroutine(collectable.Collecting(this));
                            }
                        }
                    }
                    else
                    {
                        SetDestination(DefineGoal());
                    }
                }
                break;
        }
    }

    private IEnumerator CheckAdvancedNeeds()
    {
        while (true)
        {
            Skill skill = SelectSkill(_skills.ToList());
            _advancedNeed = skill.SkillType;
            yield return new WaitForSeconds(3);
        }

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
    
    private Skill SelectSkill(List<Skill> skillList)
    {
        if (skillList == null || skillList.Count == 0)
            return new Skill(SkillType.None, 0);
        
        for (int a = 0; a < skillList.Count; a++)
        {
            if (CoinFlip(skillList[a].SkillValue))
            {
                skillList.Remove(skillList[a]);
            }
        }
        if (skillList.Count != 1)
        {
            return SelectSkill(skillList);
        }
        else
        {
            Debug.Log("Skill choisi : " + skillList[0].SkillType);
            return skillList[0];
        }
    }

    private void SetSensibilities()
    {
        SkillType[] skillArray = (SkillType[])Enum.GetValues(typeof(SkillType));
        _skills = new Skill[skillArray.Length];
        for (int a = 0; a < skillArray.Length; a++)
        {
            _skills[a].SkillType = skillArray[a];
            _skills[a].SkillValue = UnityEngine.Random.Range(.1f, .3f);
        }
        if (_papa != null && _maman != null)
        {
            if (_papa is AdvancedTwippie && _maman is AdvancedTwippie)
            {
                AdvancedTwippie aPapa = (AdvancedTwippie)_papa;
                AdvancedTwippie aMaman = (AdvancedTwippie)_maman;
                for (int a = 0; a < skillArray.Length; a++)
                {
                    float ponderation = UnityEngine.Random.value;
                    _skills[a].SkillType = skillArray[a];
                    _skills[a].SkillValue = aPapa.Skills[a].SkillValue * ponderation + aMaman.Skills[a].SkillValue * (1 - ponderation);
                }
            }
            else
            {
                if (_papa is AdvancedTwippie)
                {
                    AdvancedTwippie aPapa = (AdvancedTwippie)_papa;
                    for (int a = 0; a < skillArray.Length; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a].SkillType = skillArray[a];
                        _skills[a].SkillValue = aPapa.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation);
                    }
                }
                else if (_maman is AdvancedTwippie)
                {
                    AdvancedTwippie aMaman = (AdvancedTwippie)_maman;
                    for (int a = 0; a < skillArray.Length; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a].SkillType = skillArray[a];
                        _skills[a].SkillValue = aMaman.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation);
                    }
                }
                else
                {
                    for (int a = 0; a < skillArray.Length; a++)
                    {
                        _skills[a].SkillType = skillArray[a];
                        _skills[a].SkillValue = UnityEngine.Random.Range(.1f, .3f);
                    }
                }
            }
        }
    }

    public Skill[] Skills
    {
        get
        {
            return _skills;
        }
        set
        {
            _skills = value;
        }
    }
}
