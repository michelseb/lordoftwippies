using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Twippie : DraggableObjet, ILightnable
{

    protected enum State
    {
        Walking,
        Sleeping,
        Building,
        Collecting,
        Drinking,
        Eating,
        Contemplating,
        Reproducing,
        BeingChecked,
        None
    }

    protected enum Emotion
    {
        Happy,
        Angry,
        Surprised,
        Scared,
        Euphoric,
        Content,
        None
    }

    protected enum GoalType
    {
        Wander,
        Drink,
        Eat,
        Reproduce,
        Build,
        Collect,
        Socialize
    }

    public enum Gender
    {
        Male,
        Female
    }

    [SerializeField] protected PathFinder _pathFinder;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private int _initialStepsBeforeReproduce;
    [SerializeField] private float _health;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] protected float _speed;
    [SerializeField] protected CharacterPosition _positionModel;


    protected GameObject _goalObject;
    protected Arrival _arrival;
    protected Twippie _dad, _mom;
    protected Gender _gender;
    protected State _state, _previousState;
    protected List<Need> _needs;
    protected GoalType _goalType;
    private float _sleepiness;
    private float _hunger;
    private float _thirst;
    private float _sicknessDuration;
    private IConsumable _consumable;
    private float _endurance;
    private int _placeMemory;
    private int _peopleMemory;
    private int _stepsBeforeReproduce;
    protected float _initSpeed;
    protected bool _isDeforming, _isDeformed;
    protected bool _hasDestination;
    protected Coroutine _contemplation, _drink, _eat, _reproduce, _setDestination;
    protected float _contemplateTime;

    protected TwippieManager _twippieManager;

    private readonly bool _reproducing;

    private GameObject _target;
    public bool Healthy => _sleepiness < 50 && _thirst < 50 && _hunger < 50;
    public Twippie Dad { get { return _dad; } protected set { _dad = value; } }
    public Twippie Mom { get { return _mom; } protected set { _mom = value; } }
    public float Health { get { return _health; } set { _health = value; } }
    public LineRenderer LineRenderer { get; private set; }
    public float MaxSicknessDuration { get; protected set; }
    public Gender GenderName { get { return _gender; } }
    public int GenerationIndex { get; protected set; }
    public HealthBar HealthBar { get { return _healthBar; } }
    public Dictionary<string, Memory> Memories { get; protected set; }

    protected const string TWIPPIE_MEMORY = "TwippieMemory";
    protected const string ZONE_MEMORY = "ZoneMemory";


    protected override void Awake()
    {
        base.Awake();

        _twippieManager = TwippieManager.Instance;

        _name = "Twippie sans défenses";
        _gender = Utils.CoinFlip() ? Gender.Male : Gender.Female;
        _needs = new List<Need> { new Need(NeedType.None) };
        _twippieManager.Add(this);
    }

    protected override void Start()
    {
        base.Start();
        _previousState = State.None;
        _goalType = GoalType.Wander;
        _stepsBeforeReproduce = _initialStepsBeforeReproduce;
        _consumable = null;

        Memories = new Dictionary<string, Memory>
        {
            { TWIPPIE_MEMORY, new Memory(MemoryType.People) },
            { ZONE_MEMORY, new Memory(MemoryType.Places) }
        };

        LineRenderer = GetComponent<LineRenderer>();
        LineRenderer.enabled = false;
        _initSpeed = _speed;
        _outline.Color = 3;
        _endurance = 10;
        _health = 100;
        _goalObject = new GameObject();
        _goalObject.transform.position = transform.position;
        _arrival = _goalObject.AddComponent<Arrival>();
        SetDestination(GoalType.Wander);
        StartCoroutine(CheckSleepNeed());
        StartCoroutine(CheckBasicNeeds());


        _target = new GameObject("Target");
        _target.transform.SetParent(_planet.transform);
    }


    protected override void Update()
    {
        base.Update();
        UpdateHealth();

        if (_isDeformed && _controls.FocusedObject != this)
        {
            _rigidBody.isKinematic = false;
            //StartCoroutine(Reform(1));
            _isDeformed = false;
        }

        if (_pathFinder?.Path?.Count > 0)
        {
            _target.transform.position = _pathFinder.Path[0].transform.position;
        }

        //if (_pathFinder.Steps != null && _pathFinder.Steps.Count > 0)
        //{
        //    for (int a = 0; a < _pathFinder.Steps.Count; a++)
        //    {
        //        LineRenderer.SetPosition(a, _pathFinder.Steps[a].Zone.Center + ((_pathFinder.Steps[a].Zone.Center - _planet.transform.position).normalized) / 2);
        //    }
        //    LineRenderer.SetPosition(_pathFinder.Steps.Count - 1, transform.position + transform.up / 2);
        //}

        if (_state != State.Walking && _state != State.Sleeping && _state != State.BeingChecked && _hasDestination)
        {
            ChangeState(State.Walking);
        }

        switch (_state)
        {
            case State.Sleeping:
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.angularVelocity = Vector3.zero;
                _sleepiness = UpdateValue(_sleepiness, -3);
                break;
            case State.BeingChecked:
                transform.LookAt(_camera.transform, (transform.position - _planet.transform.position).normalized);
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.angularVelocity = Vector3.zero;
                break;
            case State.Walking:
                if (_pathFinder.Path != null)
                {
                    if (_pathFinder.Path.Count > 0 && !_mouseOver)
                    {
                        var target = _target.transform.position;
                        var center = _planet.transform.position;

                        var up = (transform.position - center).normalized;
                        var forward = (target - transform.position).normalized;

                        var targetForward = forward - up * Vector3.Dot(forward, up);

                        transform.rotation = Quaternion.LookRotation(targetForward.normalized, up);

                        _rigidBody.velocity = transform.forward * _speed * _timeReference * Time.deltaTime;

                        if (Vector3.Distance(target, _rigidBody.position) < .1f)
                        {
                            if (_pathFinder.Path[0] != null)
                            {
                                Destroy(_pathFinder.Path[0]);
                            }

                            _pathFinder.Path.RemoveAt(0);
                            LineRenderer.positionCount = _pathFinder.Path.Count + 1;

                            if (_pathFinder.Path.Count == 0)
                            {
                                _hasDestination = false;
                            }
                        }
                    }
                    else
                    {
                        switch (_goalType)
                        {
                            case GoalType.Wander:
                                ChangeState(State.Contemplating);
                                break;

                            case GoalType.Drink:
                                ChangeState(State.Drinking);
                                break;

                            case GoalType.Eat:
                                ChangeState(State.Eating);
                                break;
                            case GoalType.Reproduce:
                                ChangeState(State.Reproducing);
                                break;
                            case GoalType.Collect:
                                ChangeState(State.Collecting);
                                break;
                            case GoalType.Build:
                                ChangeState(State.Building);
                                break;

                        }
                    }
                }

                break;

            case State.Contemplating:
                _rigidBody.velocity = Vector3.zero;
                _rigidBody.angularVelocity = Vector3.zero;
                break;
        }

        if (_state != State.Sleeping)
        {
            _sleepiness = UpdateValue(_sleepiness, _ageProgression / 50); // Le besoin en sommeil augmente avec l'age. Pas besoin de dormir à 0 ans
        }
        _thirst = UpdateValue(_thirst, 2 / (_ageProgression + 1)); // Le besoin en eau diminue avec l'age
        _hunger = UpdateValue(_hunger, 2 / (_ageProgression + 1)); // Le besoin en nourriture diminue avec l'age

        if (!_grounded)
        {
            _rigidBody.velocity -= transform.up;
        }
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (!Utils.DisplayHeavyFrame)
            return;

        UpdateZone();
    }

    protected override void UpdateAge()
    {
        Memories[ZONE_MEMORY].SetCapacity(100 - Age); // La mémoire diminue avec l'âge. Alzeimer à 100 ans
        Memories[TWIPPIE_MEMORY].SetCapacity(52 - Mathf.Abs(50 - Age)); // Mémoire des personnes maximale à la moitié de la vie

        base.UpdateAge();
    }

    protected void UpdateZone()
    {
        var oldZoneId = _zoneId;
        _zoneId = _zoneManager.GetZone(false, _zoneId, transform.position);

        if (_zoneId == oldZoneId) // Si on n'a pas changé de zone, return
            return;


        var zone = Zone;
        var oldZone = _zoneManager.FindById(oldZoneId);

        zone.AddTwippie(Id);
        oldZone.RemoveTwippie(Id);

        MakeOutIfNeeded(zone);

        Memories[ZONE_MEMORY].AddOrRefresh(_zoneId);

        var otherTwippiesIds = zone.TwippieIds.Where(id => id != Id).ToArray();

        if (otherTwippiesIds?.Any() == false)
            return;

        var peopleMemory = Memories[TWIPPIE_MEMORY];

        foreach (var twippieId in otherTwippiesIds)
        {
            var twippie = _twippieManager.FindById(twippieId);
            peopleMemory.AddOrRefresh(twippieId);
        }
    }

    protected void MakeOutIfNeeded(Zone zone)
    {
        if (_ageProgression < 18 || !Healthy || _reproduce != null)
            return;

        _stepsBeforeReproduce--; //On recharge les batteries
        if (_stepsBeforeReproduce > 0) // Si la marchandise est prête
            return;

        if (!Utils.CoinFlip(.5f)) // 50% de chances à chaque changement de zone
            return;

        if (zone.TwippieIds.Count <= 1)
            return;

        var twippie = zone.TwippieIds.Select(t => _twippieManager.FindById(t)).FirstOrDefault(x => x.GenderName != _gender && x.Age >= 18); // Il faut aussi qu'il y ait un twippie du genre opposé majeur dans la même zone !

        if (twippie == null)
            return;

        _goalType = GoalType.Reproduce;
        _goalObject.transform.position = twippie.transform.position;
        _goalObject.transform.parent = twippie.transform;
        _reproduce = StartCoroutine(Reproduce(twippie));
    }

    protected override void MakeAttraction()
    {
        if (_state == State.BeingChecked || _isDeforming)
            return;

        if (!_planet)
            return;

        if (_dragging)
        {
            _planet.Face(transform);
        }
    }

    protected virtual void OnStateChange()
    {
        switch (_state)
        {
            case State.Sleeping:
                _renderer.material.color = Color.black;
                break;
            case State.Walking:
                break;
            case State.Drinking:
                if (_drink == null)
                {
                    _drink = StartCoroutine(Drink());
                }
                break;
            case State.Eating:
                if (_eat == null)
                {
                    var ressource = _arrival.FinishZone.GetResourceByType(ResourceType.Food);
                    if (ressource != null)
                    {
                        if (ressource.Consumable != null)
                        {
                            _eat = StartCoroutine(Eat(ressource.Consumable));
                        }
                    }
                    else
                    {
                        SetDestination(DefineGoal());
                    }
                }
                break;
            case State.Contemplating:
                if (_contemplation == null)
                {
                    _contemplation = StartCoroutine(Contemplate(4f));
                }
                break;
        }
    }

    protected override void OnMouseDown()
    {
        base.OnMouseDown();
        ChangeState(State.BeingChecked);
        //if (!_isDeforming)
        //{
        //    StartCoroutine(Deform(1));
        //}
    }

    private void UpdateHealth()
    {
        if (_hunger >= 100 || _thirst >= 100 || _sleepiness >= 100)
        {
            _health = UpdateValue(_health, -1);
            _sicknessDuration = UpdateValue(_sicknessDuration, 1);
        }
        else if (_hunger < 50 && _thirst < 50 && _sleepiness < 50)
        {
            _health = UpdateValue(_health, 1);
            if (_sicknessDuration > 0)
            {
                if (_sicknessDuration > MaxSicknessDuration)
                {
                    MaxSicknessDuration = _sicknessDuration;
                    _sicknessDuration = 0;
                }
            }
        }

        if (_health <= 0)
        {
            Die();
        }
    }

    private IEnumerator CheckSleepNeed()
    {
        while (true)
        {
            if (!GetLight() && _sleepiness > 30)
            {
                if (_state != State.Sleeping && _needs.Any(x => x.Type == NeedType.Sleep))
                {
                    ChangeState(State.Sleeping);
                }
            }
            else
            {
                if (_state == State.Sleeping)
                {
                    ChangeState(_previousState);
                }
            }

            yield return new WaitForSeconds(2);
        }

    }

    private IEnumerator CheckBasicNeeds()
    {
        while (true)
        {
            if (_thirst > 90)
            {
                _renderer.material.color = Color.cyan;
                var thirst = _needs.FirstOrDefault(x => x.Type == NeedType.Drink);
                SetCurrentNeed(thirst != null ? thirst : new Need(NeedType.Drink));
            }
            else if (_hunger > 90)
            {
                _renderer.material.color = Color.green;
                var hunger = _needs.FirstOrDefault(x => x.Type == NeedType.Eat);
                SetCurrentNeed(hunger != null ? hunger : new Need(NeedType.Eat));
            }
            else if (_sleepiness > 90)
            {
                _renderer.material.color = Color.red;
                var sleepiness = _needs.FirstOrDefault(x => x.Type == NeedType.Sleep);
                SetCurrentNeed(sleepiness != null ? sleepiness : new Need(NeedType.Sleep));
            }
            else if (_thirst >= 50 && _thirst > _hunger - 10 && _thirst > _sleepiness - 20)
            {
                _renderer.material.color = Color.cyan;
                var thirst = _needs.FirstOrDefault(x => x.Type == NeedType.Drink);
                SetCurrentNeed(thirst != null ? thirst : new Need(NeedType.Drink));
            }
            else if (_hunger >= 50 && _hunger > _sleepiness - 10)
            {
                _renderer.material.color = Color.green;
                var hunger = _needs.FirstOrDefault(x => x.Type == NeedType.Eat);
                SetCurrentNeed(hunger != null ? hunger : new Need(NeedType.Eat));
            }
            else if (_sleepiness >= 50)
            {
                _renderer.material.color = Color.red;
                var sleepiness = _needs.FirstOrDefault(x => x.Type == NeedType.Sleep);
                SetCurrentNeed(sleepiness != null ? sleepiness : new Need(NeedType.Sleep));
            }
            else
            {
                if (_state != State.Sleeping)
                {
                    _renderer.material.color = Color.white;
                }
                SetCurrentNeed(_needs.FirstOrDefault(x => x.Type == NeedType.None));
            }
            yield return new WaitForSeconds(3);
        }

    }

    private IEnumerator Reproduce(Twippie other)
    {
        int count = 100;
        Debug.Log("Making babies !");
        while (count > 0)
        {
            if (_renderer != null)
            {
                _renderer.material.color = UnityEngine.Random.ColorHSV();
            }

            if (other != null)
            {
                transform.LookAt(other.transform);
                other.transform.LookAt(transform);
                count--;
                yield return null;
            }
            else
            {
                _reproduce = null;
                yield break;
            }
        }
        int nbBabies = 0;

        if (_gender == Gender.Female)// C'est la femme qui accouche
        {
            foreach (var zoneId in Zone.NeighbourIds)
            {

                if (Utils.CoinFlip(.5f / (nbBabies + 1))) //% de chance de faire un bébé dans chaque zone voisine => diminue de moitié par twippie créé. Peu de chance d'avoir des triplets...
                {
                    var zone = _zoneManager.FindById(zoneId);

                    nbBabies++;
                    GenerateBaby(zone, other, this); // Génère le bébé selon la maman et le papa
                }

            }
        }

        Debug.Log(nbBabies + " babies made !");
        _stepsBeforeReproduce = _initialStepsBeforeReproduce; // On réinitialise la durée avant prochaine aventure
        SetDestination(DefineGoal());
        _reproduce = null;
    }

    private void GenerateBaby(Zone zone, Twippie dad, Twippie mom)
    {
        Twippie twippieModel;
        float wealth = (100 - ((dad.MaxSicknessDuration + mom.MaxSicknessDuration) / 2)) / 100;
        int parentsMeanGeneration = Mathf.FloorToInt((dad.GenerationIndex + mom.GenerationIndex) / 2); // Papa et maman peuvent être à des générations différentes sur l'arbre généalogique
        if (dad is AdvancedTwippie && mom is AdvancedTwippie)
        {
            twippieModel = _objectGenerator.Get<AdvancedTwippie>();
        }
        else if ((dad is AdvancedTwippie || mom is AdvancedTwippie) && Utils.CoinFlip())
        {
            twippieModel = _objectGenerator.Get<AdvancedTwippie>();
        }
        else if (Utils.CoinFlip(wealth / 5 * parentsMeanGeneration)) // Chances de devenir un twippie avancé selon le niveau de santé des parents et la génération en cours
        {
            twippieModel = _objectGenerator.Get<AdvancedTwippie>();
        }
        else
        {
            twippieModel = _objectGenerator.Get<Twippie>();
        }
        Twippie baby = Instantiate(twippieModel, zone.WorldPos, Quaternion.identity);
        Twippie twippie = baby.GetComponent<Twippie>();
        twippie.GenerationIndex = parentsMeanGeneration + 1;
        twippie.Dad = dad;
        twippie.Mom = mom;
        //TODO : Affecter les stats de papa / maman à l'enfant
        //TODO : Calculer si évolution de twippie primitif à avancé
    }

    public override void GenerateActions()
    {
        Stats.GenerateRadialAction<ModificationAction>(this);
        base.GenerateActions();
    }


    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<LabelStat>().Populate(_gender.ToString(), "Gender");
        Stats.GenerateWorldStat<ValueStat>().Populate("Hunger", 0, 0, 100, "Hunger", true);
        Stats.GenerateWorldStat<ValueStat>().Populate("Thirst", 0, 0, 100, "Thirst", true);
        Stats.GenerateWorldStat<ValueStat>().Populate("Sleep", 0, 0, 100, "Fatigue", true);
        Stats.GenerateWorldStat<LabelStat>().Populate("Need", "Main need : ");
        Stats.GenerateWorldStat<LabelStat>().Populate("Emotion", "Emotion : ");
        Stats.GenerateWorldStat<LabelStat>().Populate("Action", "Action : " + _state.ToString());
    }


    public override void PopulateStats()
    {
        base.PopulateStats();
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Gender", _gender.ToString() });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Hunger", 0, 0, 100, "Hunger", true });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Thirst", 0, 0, 100, "Thirst", true });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Sleep", 0, 0, 100, "Fatigue", true });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Need", "Main need : " + _needs[0].Type.ToString() });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Emotion", "Emotion : " });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Action", "Action : " + _state.ToString() });
    }

    protected void SetDestination(GoalType goal, bool reinit = false)
    {
        if (reinit && _setDestination != null)
        {
            StopCoroutine(_setDestination);
            _setDestination = null;
        }

        if (_setDestination != null)
            return;

        _setDestination = StartCoroutine(DoSetDestination(goal));
    }

    private IEnumerator DoSetDestination(GoalType goal)
    {
        var knownZones = Memories[ZONE_MEMORY].Data;

        switch (goal)
        {
            case GoalType.Build:
                yield return StartCoroutine(_zoneManager.GetLargeZoneByDistance(
                    _zoneId,
                    transform,
                    _pathFinder,
                    checkTaken: true,
                    distanceMax: _endurance,
                    onComplete: zoneResult =>
                    {
                        TryFinalizePath(zoneResult, goal);
                    }));
                break;
            case GoalType.Wander:
                yield return StartCoroutine(_zoneManager.GetRandomZoneByDistance(
                    _zoneId,
                    transform,
                    _pathFinder,
                    checkTaken: false,
                    distanceMax: _endurance,
                    onComplete: zoneResult =>
                    {
                        TryFinalizePath(zoneResult, goal);
                    }));
                break;
            case GoalType.Drink:
                yield return StartCoroutine(_zoneManager.GetRessourceZoneByDistance(
                    _zoneId,
                    transform,
                    _pathFinder,
                    resource: ResourceType.Drink,
                    checkTaken: true,
                    distanceMax: _endurance,
                    onComplete: zoneResult =>
                    {
                        if (zoneResult != null)
                        {
                            TryFinalizePath(zoneResult, goal);
                            Debug.Log("Drink at close zone");
                        }
                        else
                        {
                            StartCoroutine(_zoneManager.GetZoneByRessourceInList(
                                _zoneId,
                                knownZones,
                                resource: ResourceType.Drink,
                                pathFinder: _pathFinder,
                                checkTaken: true,
                                onComplete: zoneResult =>
                                {

                                    if (zoneResult != null)
                                    {
                                        TryFinalizePath(zoneResult, goal);
                                        Debug.Log("Drink at known zone");
                                    }
                                    else
                                    {
                                        Debug.Log("Can't drink");
                                        SetDestination(GoalType.Wander, true); // Or create conflict !!
                                    }
                                }));
                        }
                    }));


                break;
            case GoalType.Collect:
            case GoalType.Eat:
                yield return StartCoroutine(_zoneManager.GetRessourceZoneByDistance(
                    _zoneId,
                    transform,
                    _pathFinder,
                    resource: ResourceType.Food,
                    checkTaken: true,
                    distanceMax: _endurance,
                    onComplete: zoneResult =>
                    {

                        if (zoneResult != null)
                        {
                            TryFinalizePath(zoneResult, goal);
                            Debug.Log("Eat at close zone");
                        }
                        else
                        {
                            StartCoroutine(_zoneManager.GetZoneByRessourceInList(
                                _zoneId,
                                knownZones,
                                resource: ResourceType.Food,
                                pathFinder: _pathFinder,
                                checkTaken: true,
                                onComplete: zoneResult =>
                                {
                                    if (zoneResult != null)
                                    {
                                        TryFinalizePath(zoneResult, goal);
                                        Debug.Log("Eat at known zone");
                                    }
                                    else
                                    {
                                        Debug.Log("Can't eat");
                                        SetDestination(GoalType.Wander, true); // Or create conflict !!
                                    }
                                }));
                        }
                    }));
                break;
        }


    }

    private void TryFinalizePath(Zone destination, GoalType goal)
    {
        if (destination == null)
        {
            Debug.Log("No paths found");
            return;
        }

        _goalObject.transform.SetParent(null);

        _goalObject.transform.position = destination.WorldPos;
        _goalType = goal;
        LineRenderer.positionCount = _pathFinder.Path.Count + 1;

        _goalObject.transform.SetParent(Planet.transform);
        _arrival.SetArrival();

        _hasDestination = true;
        _setDestination = null;
    }

    protected virtual GoalType DefineGoal()
    {
        switch (_needs[0].Type)
        {
            case NeedType.Drink:
                return GoalType.Drink;
            case NeedType.Eat:
                return GoalType.Eat;
        }
        return GoalType.Wander;
    }


    private void ChangeState(State state)
    {
        if (_state != state)
        {
            _previousState = _state;
            _state = state;
            //Debug.Log("Previous : " + _previousState + " Current : " + _state);
            OnStateChange();
        }
    }

    private IEnumerator Contemplate(float duration)
    {
        yield return new WaitForSeconds(duration / _timeReference);
        SetDestination(DefineGoal(), true);
        _contemplation = null;
    }

    private IEnumerator Drink()
    {
        if (_arrival.FinishZone.Taken)
        {
            SetDestination(DefineGoal());
            _drink = null;
            yield break;
        }
        _arrival.FinishZone.Taken = true;
        while (_thirst > 5)
        {
            _thirst = UpdateValue(_thirst, -10);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            yield return null;
        }
        _thirst = 0;
        _arrival.FinishZone.Accessible = false;
        _arrival.FinishZone.Taken = false;
        SetDestination(DefineGoal());
        _drink = null;
    }

    private IEnumerator Eat(IConsumable consumable)
    {
        if (_arrival.FinishZone.Taken)
        {
            SetDestination(DefineGoal());
            _eat = null;
            yield break;
        }
        _consumable = consumable;
        _arrival.FinishZone.Taken = true;
        consumable.Reserve();
        while (consumable.Consuming(_hunger))
        {
            _hunger = UpdateValue(_hunger, -20);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            yield return null;
        }
        _consumable = null;
        consumable.Consume();
        SetDestination(DefineGoal());
        _eat = null;

    }

    private void Die()
    {
        transform.Rotate(90, 0, 0);
        _renderer.material.color = Color.gray;
        _rigidBody.AddForce((transform.position - _planet.transform.position).normalized, ForceMode.Impulse);
        Debug.Log("Twippie mort :( thirst : " + Mathf.FloorToInt(_thirst) + " hunger : " + Mathf.FloorToInt(_hunger) + " _fatigue : " + Mathf.FloorToInt(_sleepiness));
        _objectManager.RemoveObject(this);
        if (_stats != null)
        {
            _stats.enabled = false;
        }
        Destroy(LineRenderer);
        _consumable?.Consume();
        Destroy(this);
    }

    public bool GetLight()
    {
        return _sun != null && Vector3.Dot(transform.position - _planet.transform.position, transform.position - _sun.transform.position) > 0f;
        //RaycastHit hit;
        //if (_sun != null)
        //{
        //    if (Physics.Linecast(_sun.transform.position, transform.position + transform.up / 2, out hit, _mask))
        //    {
        //        return false;
        //    }
        //}
        //return true;
    }

    //protected virtual IEnumerator Reform(float time)
    //{
    //    while (_isDeforming)
    //    {
    //        yield return null;
    //    }
    //    if (_mouseOver)
    //        yield break;
    //    _isDeforming = true;
    //    var currTime = 0f;

    //    while (currTime < time)
    //    {
    //        for (int i = 0; i < _deformedVertices.Length; i++)
    //        {
    //            Vector3 direction = transform.InverseTransformPoint(transform.position) - _originalVertices[i];
    //            _deformedVertices[i].x = Mathf.Lerp(_deformedVertices[i].x, _originalVertices[i].x, currTime / time);
    //            _deformedVertices[i].y = Mathf.Lerp(_deformedVertices[i].y, _originalVertices[i].y, currTime / time);
    //            _deformedVertices[i].z = Mathf.Lerp(_deformedVertices[i].z, _originalVertices[i].z, currTime / time);
    //        }
    //        currTime += .1f * _timeReference;
    //        _mesh.vertices = _deformedVertices;
    //        yield return null;
    //    }

    //    _deformedVertices = _originalVertices.ToArray();
    //    _mesh.vertices = _originalVertices;
    //    _mesh.RecalculateNormals();
    //    if (_meshCollider != null)
    //    {
    //        _meshCollider.sharedMesh = _mesh;
    //    }
    //    _isDeforming = false;
    //    ChangeState(_previousState);
    //}

    //protected virtual IEnumerator Deform(float time)
    //{
    //    _rigidBody.isKinematic = true;
    //    _isDeformed = true;
    //    _isDeforming = true;
    //    var currTime = 0f;

    //    while (currTime < time)
    //    {
    //        for (int i = 0; i < _deformedVertices.Length; i++)
    //        {
    //            Vector3 direction = transform.InverseTransformPoint(transform.position) - _originalVertices[i];
    //            _deformedVertices[i].x = Mathf.Lerp(_originalVertices[i].x, _originalVertices[i].x * Mathf.Clamp(direction.magnitude - 2, .1f, 10) * 15, currTime / time);
    //            _deformedVertices[i].y = Mathf.Lerp(_originalVertices[i].y, _originalVertices[i].y * Mathf.Clamp(direction.magnitude - 2, .1f, 10) * 4, currTime / time);
    //            _deformedVertices[i].z = Mathf.Lerp(_originalVertices[i].z, _originalVertices[i].z * Mathf.Clamp(direction.magnitude - 2, .1f, 10) * 4, currTime / time);
    //        }
    //        currTime += .1f * _timeReference;
    //        _mesh.vertices = _deformedVertices;
    //        yield return null;
    //    }

    //    _mesh.RecalculateNormals();
    //    if (_meshCollider != null)
    //    {
    //        _meshCollider.sharedMesh = _mesh;
    //    }
    //    _isDeforming = false;
    //}


    protected override void UpdateStats()
    {
        base.UpdateStats();
        _stats.StatToValue(_stats.GetStat("Age")).Value = _ageProgression;
        _stats.StatToLabel(_stats.GetStat("Gender")).Value = _gender.ToString();
        _stats.StatToValue(_stats.GetStat("Hunger")).Value = _hunger;
        _stats.StatToValue(_stats.GetStat("Thirst")).Value = _thirst;
        _stats.StatToValue(_stats.GetStat("Sleep")).Value = _sleepiness;
        _stats.StatToLabel(_stats.GetStat("Need")).Value = "Main need : " + _needs[0].Type.ToString();
        _stats.StatToLabel(_stats.GetStat("Action")).Value = "Action : " + _state.ToString();
    }

    protected void SetCurrentNeed(Need need)
    {
        if (_needs.Contains(need))
        {
            _needs.Remove(need);
        }
        _needs.Insert(0, need);

    }

    public void FinishExternalAction()
    {
        SetDestination(DefineGoal());
    }

    protected override void ColorMe()
    {
        base.ColorMe();
        HealthBar.Health.color = new Color(HealthBar.Health.color.r, HealthBar.Health.color.g, HealthBar.Health.color.b, _renderer.material.color.a);
    }

    public void RefreshPath()
    {
        StartCoroutine(_pathFinder.RefreshPath(ZoneId, zoneResult =>
        {
            if (zoneResult != null)
            {
                TryFinalizePath(zoneResult, _goalType);
                Debug.Log("Eat at close zone");
            }
        }));
    }
}
