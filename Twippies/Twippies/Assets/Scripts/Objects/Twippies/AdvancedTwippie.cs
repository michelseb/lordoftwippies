using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedTwippie : Twippie {
    
    protected float[] advancedNeedSensibilities;
    protected List<Ressource> _ressources;
    private Twippie _partner;
    protected List<IBuildable> _builtStuff;
    protected Coroutine _building, _collecting;
    protected Skill _advancedNeed;
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
        _advancedNeed = new Skill(SkillType.None, 0);
        SetSensibilities();
        StartCoroutine(CheckAdvancedNeeds());
        _ressources = new List<Ressource> { new Ressource(Ressource.RessourceType.Drink, 0), new Ressource(Ressource.RessourceType.Food, 0) };
        _builtStuff = new List<IBuildable>();

    }

    protected override void Update()
    {
        base.Update();
        for(int a = 0; a < _skills.Length; a++)
        {
            _skills[a].SkillValue = UpdateValue(_skills[a].SkillValue, -.001f, 0, 1); //Perte de skill globale constante
        }

        _stats.StatToLabel(_stats.StatsList[10]).Value = "Bois possédé : " + _ressources[1].quantity;
    }

    protected override void GenerateStats()
    {
        base.GenerateStats();
        _stats.StatsList[10] = new LabelStat("Bois possédé : ");
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
        switch (_advancedNeed.Type)
        {
            case SkillType.Socialize:
                return GoalType.Socialize;
            case SkillType.Build:
                if (_og.GetObjects<IBuildable>().FirstOrDefault(x=>x.WoodCost <= _ressources[1].quantity) != null)
                {
                    Debug.Log("Need to build");
                    return GoalType.Build;
                }
                else
                {
                    Debug.Log("Need to collect");
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
                    ManageableObjet obj = _og.GetObjects<IBuildable>().FirstOrDefault(x => Mathf.Pow(x.WoodCost,(_builtStuff.Count+1)) <= _ressources[1].quantity); // Plus on possède de maisons plus elles sont chères
                    if (obj != null)
                    {
                        GameObject house = Instantiate(obj.gameObject, _arrival.FinishZone.Center, Quaternion.identity);
                        house.transform.localScale = Vector3.zero;
                        obj = house.GetComponent<ManageableObjet>();
                        obj.CurrentSize = Vector3.zero;
                        var buildable = (IBuildable)obj;
                        _builtStuff.Add(buildable);
                        Debug.Log("Building");
                        _arrival.FinishZone.Accessible = false;
                        foreach (Zone z in _arrival.FinishZone.Neighbours)
                        {
                            z.Accessible = false;
                        }
                        Skill updatingSkill = _skills.FirstOrDefault(x => x.Type == SkillType.Build);
                        updatingSkill.SkillValue = UpdateValue(updatingSkill.SkillValue + .2f, 0, 0, 1);
                        _building = StartCoroutine(buildable.Build(_builtStuff.Count+1));
                    }
                    else
                    {
                        SetDestination(DefineGoal());
                    }
                }
                break;
            case State.Collecting:
                Debug.Log("Should collect");
                if (_collecting == null)
                {
                    Ressource ressource = _arrival.FinishZone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Food);
                    if (ressource != null)
                    {
                        if (ressource.consumableObject != null)
                        {
                            if (ressource.consumableObject is ICollectable)
                            {
                                if (_arrival.FinishZone.Taken)
                                {
                                    SetDestination(DefineGoal());
                                }
                                else
                                {
                                    _arrival.FinishZone.Taken = true;
                                    ICollectable collectable = (ICollectable)ressource.consumableObject;
                                    Debug.Log("Collecting");
                                    _collecting = StartCoroutine(collectable.Collecting(this));
                                }
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
            _advancedNeed = skill;
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

        float skillsSum = skillList.Sum(x => x.SkillValue); // Somme des valeurs des skills
        for (int a = 0; a < skillList.Count; a++)
        {
            if (!CoinFlip(skillList[a].SkillValue/skillsSum)) //Coinflip avec chance de la valeur du skill divisée par la somme des valeurs de tous les skills
            {
                skillList.Remove(skillList[a]); //On enlève l'option si coinflip raté
            }
        }
        if (skillList.Count != 1)
        {
            return SelectSkill(skillList);
        }
        else
        {
            return skillList[0]; // Si une seule option restante, on la retourne. Sinon, indécis...
        }
    }

    private void SetSensibilities()
    {
        SkillType[] skillArray = (SkillType[])Enum.GetValues(typeof(SkillType));
        _skills = new Skill[skillArray.Length-1];
        for (int a = 0; a < skillArray.Length-1; a++)
        {
            _skills[a] = new Skill(skillArray[a], UnityEngine.Random.Range(.1f, .3f));
        }
        if (_papa != null && _maman != null)
        {
            if (_papa is AdvancedTwippie && _maman is AdvancedTwippie)
            {
                AdvancedTwippie aPapa = (AdvancedTwippie)_papa;
                AdvancedTwippie aMaman = (AdvancedTwippie)_maman;
                for (int a = 0; a < skillArray.Length-1; a++)
                {
                    float ponderation = UnityEngine.Random.value;
                    _skills[a] = new Skill(skillArray[a], aPapa.Skills[a].SkillValue * ponderation + aMaman.Skills[a].SkillValue * (1 - ponderation));
                }
            }
            else
            {
                if (_papa is AdvancedTwippie)
                {
                    AdvancedTwippie aPapa = (AdvancedTwippie)_papa;
                    for (int a = 0; a < skillArray.Length-1; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a] = new Skill(skillArray[a], aPapa.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation));
                    }
                }
                else if (_maman is AdvancedTwippie)
                {
                    AdvancedTwippie aMaman = (AdvancedTwippie)_maman;
                    for (int a = 0; a < skillArray.Length-1; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a] = new Skill(skillArray[a], aMaman.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation));
                    }
                }
                else
                {
                    for (int a = 0; a < skillArray.Length-1; a++)
                    {
                        _skills[a] = new Skill(skillArray[a], UnityEngine.Random.Range(.1f, .3f));
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

    public Coroutine Collecting
    {
        get
        {
            return _collecting;
        }
        set
        {
            _collecting = value;
        }
    }
}
