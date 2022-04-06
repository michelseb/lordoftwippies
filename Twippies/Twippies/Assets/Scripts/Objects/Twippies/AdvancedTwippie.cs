using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdvancedTwippie : Twippie
{

    protected float[] advancedNeedSensibilities;
    protected List<Resource> _ressources;
    private Twippie _partner;
    protected List<IBuildable> _builtStuff;
    protected Coroutine _building, _collecting;
    protected Skill _advancedNeed;
    protected Skill[] _skills;

    public Skill[] Skills { get { return _skills; } set { _skills = value; } }
    public Coroutine Collecting { get { return _collecting; } set { _collecting = value; } }

    protected override void Awake()
    {
        base.Awake();
        _name = "Twippie qui sait faire des trucs cools !";
    }

    protected override void Start()
    {
        base.Start();
        _partner = null;
        _advancedNeed = new Skill(SkillType.None, 0);
        SetSensibilities();
        StartCoroutine(CheckAdvancedNeeds());
        _ressources = new List<Resource> { new Resource(ResourceType.Drink, 0), new Resource(ResourceType.Food, 0) };
        _builtStuff = new List<IBuildable>();

    }

    protected override void Update()
    {
        base.Update();
        for (int a = 0; a < _skills.Length; a++)
        {
            _skills[a].SkillValue = UpdateValue(_skills[a].SkillValue, -.001f, 0, 1); //Perte de skill globale constante
        }
    }

    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<LabelStat>().Populate("WoodPossession", "Bois possédé : ");
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "WoodPossession", "Bois possédé : " });
    }

    protected override GoalType DefineGoal()
    {
        switch (_needs[0].Type)
        {
            case NeedType.Drink:
                return GoalType.Drink;
            case NeedType.Eat:
                return GoalType.Eat;
        }
        switch (_advancedNeed.Type)
        {
            case SkillType.Socialize:
                return GoalType.Socialize;
            case SkillType.Build:
                if (_objectGenerator.GetObjects<IBuildable>().FirstOrDefault(x => x.WoodCost <= _ressources[1].Quantity) != null)
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
                    var obj = _objectGenerator.GetObjects<IBuildable>().FirstOrDefault(x => Mathf.Pow(x.WoodCost, _builtStuff.Count + 1) <= _ressources[1].Quantity); // Plus on possède de maisons plus elles sont chères
                    if (obj != null)
                    {
                        GameObject house = Instantiate(obj.gameObject, _arrival.FinishZone.WorldPos, Quaternion.identity);
                        house.transform.localScale = Vector3.zero;
                        obj = house.GetComponent<ManageableObjet>();
                        obj.CurrentSize = Vector3.zero;
                        var buildable = (IBuildable)obj;
                        _builtStuff.Add(buildable);
                        Debug.Log("Building");

                        _arrival.FinishZone.Accessible = false;
                        _arrival.FinishZone.NeighbourIds.ForEach(n => _zoneManager.FindById(n).Accessible = false);

                        Skill updatingSkill = _skills.FirstOrDefault(x => x.Type == SkillType.Build);
                        updatingSkill.SkillValue = UpdateValue(updatingSkill.SkillValue + .2f, 0, 0, 1);
                        _building = StartCoroutine(buildable.Build(_builtStuff.Count + 1));
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
                    var ressource = _arrival.FinishZone.GetResourceByType(ResourceType.Food);
                    if (ressource != null)
                    {
                        if (ressource.Consumable != null)
                        {
                            if (ressource.Consumable is ICollectable collectable)
                            {
                                if (_arrival.FinishZone.Taken)
                                {
                                    SetDestination(DefineGoal());
                                }
                                else
                                {
                                    _arrival.FinishZone.Taken = true;
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

    public List<Resource> Ressources
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
            if (!Utils.CoinFlip(skillList[a].SkillValue / skillsSum)) //Coinflip avec chance de la valeur du skill divisée par la somme des valeurs de tous les skills
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
        _skills = new Skill[skillArray.Length - 1];
        for (int a = 0; a < skillArray.Length - 1; a++)
        {
            _skills[a] = new Skill(skillArray[a], UnityEngine.Random.Range(.1f, .3f));
        }
        if (_dad != null && _mom != null)
        {
            if (_dad is AdvancedTwippie && _mom is AdvancedTwippie)
            {
                AdvancedTwippie aPapa = (AdvancedTwippie)_dad;
                AdvancedTwippie aMaman = (AdvancedTwippie)_mom;
                for (int a = 0; a < skillArray.Length - 1; a++)
                {
                    float ponderation = UnityEngine.Random.value;
                    _skills[a] = new Skill(skillArray[a], aPapa.Skills[a].SkillValue * ponderation + aMaman.Skills[a].SkillValue * (1 - ponderation));
                }
            }
            else
            {
                if (_dad is AdvancedTwippie)
                {
                    AdvancedTwippie aPapa = (AdvancedTwippie)_dad;
                    for (int a = 0; a < skillArray.Length - 1; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a] = new Skill(skillArray[a], aPapa.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation));
                    }
                }
                else if (_mom is AdvancedTwippie)
                {
                    AdvancedTwippie aMaman = (AdvancedTwippie)_mom;
                    for (int a = 0; a < skillArray.Length - 1; a++)
                    {
                        float ponderation = UnityEngine.Random.value;
                        _skills[a] = new Skill(skillArray[a], aMaman.Skills[a].SkillValue * ponderation + UnityEngine.Random.Range(.1f, .3f) * (1 - ponderation));
                    }
                }
                else
                {
                    for (int a = 0; a < skillArray.Length - 1; a++)
                    {
                        _skills[a] = new Skill(skillArray[a], UnityEngine.Random.Range(.1f, .3f));
                    }
                }
            }
        }
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        _stats.StatToLabel(_stats.GetStat("WoodPossession")).Value = "Bois possédé : " + _ressources[1].Quantity;
    }

}
