using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Twippie : DraggableObjet, ILightnable {

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

    protected enum BasicNeed
    {
        Eat,
        Drink,
        Sleep,
        Warmup,
        Cooldown,
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

    protected GameObject _goalObject;
    protected Arrival _arrival;
    protected List<Zone> _knownZones;
    protected List<Twippie> _knownTwippies;
    protected Twippie _papa, _maman;
    [SerializeField]
    protected PathFinder _pathFinder;
    protected Gender _gender;
    protected State _state, _previousState;
    protected BasicNeed _basicNeed;
    protected GoalType _goalType;
    private LineRenderer _lineRenderer;

    [SerializeField]
    private LayerMask _mask;
    private float _sleepiness;
    private float _hunger;
    private float _thirst;
    [SerializeField]
    private int _initialStepsBeforeReproduce;
    [SerializeField]
    private float _health;
    private float _sicknessDuration, _maxSicknessDuration;
    private int _nbGeneration;
    protected bool _healthy;
    private IConsumable _consumable;
    private float _ageSize;
    private float _endurance;
    private int _placeMemory;
    private int _peopleMemory;
    private int _stepsBeforeReproduce;
    [SerializeField]
    protected float _speed;

    protected float _initSpeed;

    protected Coroutine _contemplation, _drink, _eat, _reproduce;
    protected float _contemplateTime;

    private bool _reproducing;


    protected override void Awake()
    {
        base.Awake();
        _type = "Twippie primitif";
        _name = "Twippie sans défenses";
        _gender = CoinFlip() ? Gender.Male : Gender.Female;
    }

    protected override void Start()
    {
        base.Start();
        _displayIntervals = 5;
        _basicNeed = BasicNeed.None;
        _previousState = State.None;
        _goalType = GoalType.Wander;
        
        _stepsBeforeReproduce = _initialStepsBeforeReproduce;
        _consumable = null;
        _knownZones = new List<Zone>();
        _knownTwippies = new List<Twippie>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _initSpeed = _speed;
        _outline.color = 3;
        _endurance = 10;// +Random.value * 10;
        _health = 100;
        _goalObject = new GameObject();
        //_goalObject.GetComponent<SphereCollider>().isTrigger = true;
        _arrival = _goalObject.AddComponent<Arrival>();
        _arrival.ZoneManager = _zManager;
        SetDestination(GoalType.Wander);
        StartCoroutine(CheckSleepNeed());
        StartCoroutine(CheckBasicNeeds());
        
    }


    protected override void Update()
    {
        base.Update();
        UpdateHealth();
        _ageSize = _age / 40;
        if (!_mouseOver)
        {
            _currentSize = _initSize + Vector3.one * _ageSize;
        }
        else
        {
            _currentSize = _initSize * _sizeMultiplier + Vector3.one * _ageSize;
        }

        if (_pathFinder.Steps != null && _pathFinder.Steps.Count > 0)
        {
            for (int a = 0; a < _pathFinder.Steps.Count; a++)
            {
                _lineRenderer.SetPosition(a, _pathFinder.Steps[a].Zone.Center + ((_pathFinder.Steps[a].Zone.Center - _p.transform.position).normalized) / 2);
            }
            _lineRenderer.SetPosition(_pathFinder.Steps.Count, transform.position + transform.up / 2);
        }

        if (_state != State.Walking && _state != State.Sleeping)
        {
            if (Vector3.Distance(transform.position, _goalObject.transform.position) > 1)
            {
                ChangeState(State.Walking);
            }
        }

        switch (_state)
        {
            case State.Sleeping:
                _r.velocity = Vector3.zero;
                _r.angularVelocity = Vector3.zero;
                _sleepiness = UpdateValue(_sleepiness, -3);
                break;

            case State.Walking:
                if (_pathFinder.Steps != null)
                {
                    if (_pathFinder.Steps.Count > 0)
                    {
                        
                        Vector3 direction = _pathFinder.Steps[_pathFinder.Steps.Count-1].Zone.Center - transform.position;
                        Quaternion rotation = Quaternion.FromToRotation(transform.forward, direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation * transform.rotation, Mathf.Clamp(Time.deltaTime * _timeReference * 5, .3f, 1));
                        Vector3 newPos = _r.position + transform.TransformDirection(new Vector3(0, 0, _speed * _timeReference *Time.deltaTime));
                        _r.MovePosition(newPos); 
                        if (direction.magnitude < .3f)
                        {
                            if (_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go != null)
                                Destroy(_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go);
                            _pathFinder.Steps.RemoveAt(_pathFinder.Steps.Count - 1);
                            _lineRenderer.positionCount = _pathFinder.Steps.Count + 1;
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

                break;
        }

        if (_state != State.Sleeping)
        {
            _sleepiness = UpdateValue(_sleepiness, _age / 50); // Le besoin en sommeil augmente avec l'age. Pas besoin de dormir à 0 ans
        }
        _thirst = UpdateValue(_thirst, 2/(_age+1)); // Le besoin en eau diminue avec l'age
        _hunger = UpdateValue(_hunger, 2/(_age+1)); // Le besoin en nourriture diminue avec l'age
        _placeMemory = 100 - Mathf.FloorToInt(_age); // La mémoire diminue avec l'âge. Alzeimer à 100 ans
        _peopleMemory = 52 - Mathf.Abs(50 - Mathf.FloorToInt(_age)); // Mémoire des personnes maximale à la moitié de la vie

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (Time.frameCount % _displayIntervals == 0)
        {
            var oldZone = _zone;
            _zone = _zManager.GetZone(false, _zone, transform);
            if (_zone != oldZone) // Si on a changé de zone
            {
                if (_reproduce == null) { // S'ils ne sont pas déjà en train...
                    if (_healthy) // Faut être en forme
                    {
                        if (_age > 18) // Faut être majeur hein !
                        {
                            _stepsBeforeReproduce--; //On recharge les batteries
                            if (_stepsBeforeReproduce <= 0) // Si la marchandise est prête
                            {
                                if (CoinFlip(.5f)) // 50% de chances à chaque changement de zone
                                {
                                    Gender otherGender = (_gender == Gender.Male) ? Gender.Female : Gender.Male; // On fait ça avec le sexe opposé
                                    if (_zone.Twippies.Count > 1)
                                    {
                                        Twippie twippie = _zone.Twippies.FirstOrDefault(x => x.GenderName == otherGender && x.Age > 18); // Il faut aussi qu'il y ait un twippie du genre opposé majeur dans la même zone !
                                        if (twippie != null)
                                        {
                                            _goalType = GoalType.Reproduce;
                                            _goalObject.transform.position = twippie.transform.position;
                                            _goalObject.transform.parent = twippie.transform;
                                            _reproduce = StartCoroutine(Reproduce(twippie));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (_knownZones.Contains(_zone))
                {
                    if (_knownZones.IndexOf(_zone) < _knownZones.Count - 1)
                        _knownZones.Remove(_zone);
                    _knownZones.Add(_zone); //Rafraichissement de mémoire sur une zone déjà visitée
                }
                if (!_knownZones.Contains(_zone))
                {
                    _knownZones.Add(_zone); //Découverte d'une nouvelle zone
                }

                while (_knownZones.Count > _placeMemory)
                {
                    _knownZones.RemoveAt(0); //Oubli d'une zone lorsque la mémoire est saturée
                }
                if (_zone.Twippies?.Count > 0)
                {
                    foreach (Twippie twippie in _zone.Twippies)
                    {
                        if (twippie == this || twippie == null)
                            continue;
                        if (_knownTwippies.Contains(twippie))
                        {
                            if (_knownTwippies.IndexOf(twippie) < _knownTwippies.Count - 1)
                                _knownTwippies.Remove(twippie);
                            _knownTwippies.Add(twippie); //Rafraichissement de mémoire sur un twippie déjà rencontré
                        }
                        if (!_knownTwippies.Contains(twippie))
                        {
                            _knownTwippies.Add(twippie); //Rencontre d'un nouveau twippie
                        }

                        if (twippie.KnownTwippies.Contains(this))
                        {
                            if (twippie.KnownTwippies.IndexOf(this) < twippie.KnownTwippies.Count - 1)
                                twippie.KnownTwippies.Remove(this);
                            _knownTwippies.Add(this); //Rafraichissement de mémoire de l'autre twippie
                        }
                        if (!twippie.KnownTwippies.Contains(this))
                        {
                            twippie.KnownTwippies.Add(this); // Rencontre mutuelle
                        }
                    }
                }
                while (_knownTwippies.Count > _peopleMemory)
                {
                    _knownTwippies.RemoveAt(0); //Oubli d'un twippie lorsque la mémoire est saturée
                }
            }
        }
    }

    protected virtual void OnStateChange()
    {
        switch (_state)
        {
            case State.Sleeping:
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.black;
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
                    Ressource ressource = _arrival.FinishZone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Food);
                    if (ressource != null)
                    {
                        if (ressource.consumableObject != null)
                        {
                            _eat = StartCoroutine(Eat(ressource.consumableObject));
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
                    _contemplation = StartCoroutine(Contemplate(.5f));
                }
                break;
        }
    }

    protected override void OnMouseOver()
    {
        base.OnMouseOver();
        _currentSize = _initSize * _sizeMultiplier;
        _speed = 0;
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();
        _speed = _initSpeed;
    }

    private void UpdateHealth()
    {
        if (_hunger >= 100 || _thirst >= 100 || _sleepiness >= 100)
        {
            _health = UpdateValue(_health, -1);
            _sicknessDuration = UpdateValue(_sicknessDuration);
            _healthy = false;
        }
        else if (_hunger < 50 && _thirst < 50 && _sleepiness < 50)
        {
            _health = UpdateValue(_health);
            if (_sicknessDuration > 0)
            {
                if (_sicknessDuration > _maxSicknessDuration)
                {
                    _maxSicknessDuration = _sicknessDuration;
                    _sicknessDuration = 0;
                }
            }
            _healthy = true;
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

                if (_state != State.Sleeping)
                {
                    if (_basicNeed == BasicNeed.Sleep)
                    {
                        ChangeState(State.Sleeping);
                    }
                }
                yield return new WaitForSeconds(2);
            }
            else
            {
                    
                if (_state == State.Sleeping)
                {
                    ChangeState(_previousState);
                }

                yield return new WaitForSeconds(2);
            }
        }
        
    }

    private IEnumerator CheckBasicNeeds()
    {
        while (true)
        {
            if (_thirst > 90)
            {
                _renderer.material.color = Color.blue;
                _basicNeed = BasicNeed.Drink;
            }else if (_hunger > 90)
            {
                _renderer.material.color = Color.green;
                _basicNeed = BasicNeed.Eat;
            }
            else if (_sleepiness > 90)
            {
                _renderer.material.color = Color.red;
                _basicNeed = BasicNeed.Sleep;
            }
            else if (_thirst >= 50 && _thirst > _hunger - 10 && _thirst > _sleepiness - 20)
            {
                _renderer.material.color = Color.blue;
                _basicNeed = BasicNeed.Drink;
            }
            else if (_hunger >= 50 && _hunger > _sleepiness - 10)
            {
                _renderer.material.color = Color.green;
                _basicNeed = BasicNeed.Eat;
            }
            else if (_sleepiness >= 50)
            {
                _renderer.material.color = Color.red;
                _basicNeed = BasicNeed.Sleep;
            }
            else
            {
                if (_state != State.Sleeping)
                    _renderer.material.color = Color.white;
                _basicNeed = BasicNeed.None;
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
                _renderer.material.color = new Color(Random.value, Random.value, Random.value);
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
            foreach (Zone zone in _zone.Neighbours)
            {
             
                if (CoinFlip(.5f / (nbBabies + 1))) //% de chance de faire un bébé dans chaque zone voisine => diminue de moitié par twippie créé. Peu de chance d'avoir des triplets...
                {
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

    private void GenerateBaby(Zone zone, Twippie papa, Twippie mama)
    {
        GameObject twippieModel = null;
        float wealth = (100 - ((papa.MaxSicknessDuration + mama.MaxSicknessDuration) / 2))/100;
        int parentsMeanGeneration = Mathf.FloorToInt((papa.NbGeneration + mama.NbGeneration) / 2); // Papa et maman peuvent être à des générations différentes sur l'arbre généalogique
        if (papa is AdvancedTwippie && mama is AdvancedTwippie)
        {
            twippieModel = _og.GetGO<AdvancedTwippie>();
        }
        else if ((papa is AdvancedTwippie || mama is AdvancedTwippie)&& CoinFlip())
        {
            twippieModel = _og.GetGO<AdvancedTwippie>();
        }
        else if (CoinFlip(wealth/5 * parentsMeanGeneration)) // Chances de devenir un twippie avancé selon le niveau de santé des parents et la génération en cours
        {
            twippieModel = _og.GetGO<AdvancedTwippie>();
        }
        else
        {
            twippieModel = _og.GetGO<Twippie>();
        }
        GameObject baby = Instantiate(twippieModel, zone.Center, Quaternion.identity);
        Twippie twippie = baby.GetComponent<Twippie>();
        twippie.NbGeneration = parentsMeanGeneration+1;
        twippie.Papa = papa;
        twippie.Maman = mama;
        _om.allObjects.Add(twippie);
        //TODO : Affecter les stats de papa / maman à l'enfant
        //TODO : Calculer si évolution de twippie primitif à avancé
    }

    public override void GenerateStats()
    {
        base.GenerateStats();
        _stats.GenerateStat<LabelStat>(this).Populate(_gender.ToString());
        _stats.GenerateStat<ValueStat>(this).Populate(0, 0, 100, "Hunger", true);
        _stats.GenerateStat<ValueStat>(this).Populate(0, 0, 100, "Thirst", true);
        _stats.GenerateStat<ValueStat>(this).Populate(0, 0, 100, "Fatigue", true);
        _stats.GenerateStat<LabelStat>(this).Populate("Need : " + _basicNeed.ToString());
        _stats.GenerateStat<LabelStat>(this).Populate("Emotion : ");
        _stats.GenerateStat<LabelStat>(this).Populate("Action : " + _state.ToString());
    }

    protected void SetDestination(GoalType goal)
    {
        _goalObject.transform.parent = null;
        Zone zone = null;
        switch (goal)
        {
            case GoalType.Build:
                zone = _zManager.GetLargeZoneByDistance(transform, _pathFinder, checkAccessible: true, checkTaken: true, distanceMax: _endurance);
                break;
            case GoalType.Wander:
                zone = _zManager.GetRandomZoneByDistance(transform, _pathFinder, checkAccessible: true, checkTaken: false, distanceMax: _endurance);
                break;
            case GoalType.Drink:
                zone = _zManager.GetRessourceZoneByDistance(transform, _pathFinder, ressource: Ressource.RessourceType.Drink, checkAccessible: true, checkTaken: true, distanceMax: _endurance);
                if (zone != null)
                {
                    Debug.Log("Drink at close zone");
                }
                else
                {
                    zone = _zManager.GetZoneByRessourceInList(_knownZones, ressource: Ressource.RessourceType.Drink, p: _pathFinder, checkTaken: true, checkAccessible: true);
                    if (zone != null)
                    {
                        Debug.Log("Drink at known zone");
                    }
                    else
                    {
                        Debug.Log("Can't drink");
                        SetDestination(GoalType.Wander); // Or create conflict !!
                        return;
                    }
                }
                break;
            case GoalType.Collect:
            case GoalType.Eat:
                zone = _zManager.GetRessourceZoneByDistance(transform, _pathFinder, ressource: Ressource.RessourceType.Food, checkAccessible: true, checkTaken: true, distanceMax: _endurance);
                if (zone != null)
                {
                    Debug.Log("Try eating at close zone");
                }
                else
                {
                    zone = _zManager.GetZoneByRessourceInList(_knownZones, ressource: Ressource.RessourceType.Food, p:_pathFinder, checkTaken: true, checkAccessible: true);
                    if (zone != null)
                    {
                        Debug.Log("Try eating at known zone");
                    }
                    else
                    {
                        Debug.Log("Can't eat");
                        SetDestination(GoalType.Wander); // Or create conflict !!
                        return;
                    }
                }
                break;
        }

        if (zone != null)
        {
            _goalObject.transform.position = zone.Center;
            _goalType = goal;
            _lineRenderer.positionCount = _pathFinder.Steps.Count + 1;
            _state = State.Walking;
        }
        else
        {
            _goalObject.transform.position = transform.position;
            _state = State.Contemplating;
        }
        _goalObject.transform.parent = P.transform;
        _arrival.SetArrival();
        

    }

    protected virtual GoalType DefineGoal()
    {
        switch (_basicNeed)
        {
            case BasicNeed.Drink:
                return GoalType.Drink;
            case BasicNeed.Eat:
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
            Debug.Log("Previous : " + _previousState + " Current : " + _state);
            OnStateChange();
        }
    }

    private IEnumerator Contemplate(float temps)
    {
        yield return new WaitForSeconds(temps/_timeReference);
        SetDestination(DefineGoal());
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
            _r.velocity = Vector3.zero;
            _r.angularVelocity = Vector3.zero;
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
            _r.velocity = Vector3.zero;
            _r.angularVelocity = Vector3.zero;
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
        _r.AddForce((transform.position - _p.transform.position).normalized, ForceMode.Impulse);
        Debug.Log("Twippie mort :( thirst : " + Mathf.FloorToInt(_thirst) + " hunger : " + Mathf.FloorToInt(_hunger) + " _fatigue : " + Mathf.FloorToInt(_sleepiness));
        _om.allObjects.Remove(this);
        _stats.enabled = false;
        Destroy(_lineRenderer);
        _consumable?.Consume();
        Destroy(this);
    }

    public bool GetLight()
    {
        RaycastHit hit;
        if (_planetSun != null)
        {
            if (Physics.Linecast(_planetSun.transform.position, transform.position + transform.up / 2, out hit, _mask))
            {
                return false;
            }
        }
        return true;
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        _stats.StatToValue(_stats.StatsList[0]).Value = _age;
        _stats.StatToLabel(_stats.StatsList[3]).Value = _gender.ToString();
        _stats.StatToValue(_stats.StatsList[4]).Value = _hunger;
        _stats.StatToValue(_stats.StatsList[5]).Value = _thirst;
        _stats.StatToValue(_stats.StatsList[6]).Value = _sleepiness;
        _stats.StatToLabel(_stats.StatsList[7]).Value = "Need : " + _basicNeed.ToString();
        _stats.StatToLabel(_stats.StatsList[9]).Value = "Action : " + _state.ToString();
    }

    public LineRenderer LineRenderer
    {
        get
        {
            return _lineRenderer;
        }
    }

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
        }
    }

    public float MaxSicknessDuration
    {
        get
        {
            return _maxSicknessDuration;
        }
        protected set
        {
            _maxSicknessDuration = value;
        }
    }

    public List<Twippie> KnownTwippies
    {
        get
        {
            return _knownTwippies;
        }
        set
        {
            _knownTwippies = value;
        }
    }

    public Gender GenderName
    {
        get
        {
            return _gender;
        }
    }

    public int NbGeneration
    {
        get
        {
            return _nbGeneration;
        }
        protected set
        {
            _nbGeneration = value;
        }
    }

    public Twippie Papa
    {
        get
        {
            return _papa;
        }
        protected set
        {
            _papa = value;
        }
    }

    public Twippie Maman
    {
        get
        {
            return _maman;
        }
        protected set
        {
            _maman = value;
        }
    }

    public void FinishExternalAction()
    {
        SetDestination(DefineGoal());
    }
}
