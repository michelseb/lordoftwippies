using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Twippie : DraggableObjet, ILightnable {

    protected enum State
    {
        Walking,
        Sleeping,
        Building,
        Drinking,
        Eating,
        Contemplating,
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
        None
    }

    protected enum AdvancedNeed
    {
        Socialize,
        Warmup,
        Cooldown,
        None
    }

    protected enum GoalType
    {
        Wander,
        Drink,
        Eat,
        Socialize
    }

    protected GameObject _goalObject;
    protected Arrival _arrival;
    protected List<Zone> _knownZones;
    [SerializeField]
    protected PathFinder _pathFinder;
    protected State _state, _previousState;
    protected BasicNeed _basicNeed;
    protected AdvancedNeed _advancedNeed;
    protected GoalType _goalType;
    private LineRenderer _lineRenderer;

    [SerializeField]
    private LayerMask _mask;

    [SerializeField]
    private float _sleepiness;
    [SerializeField]
    private float _hunger;
    [SerializeField]
    private float _thirst;

    [SerializeField]
    private float _health;
    private float _ageSize;
    private float _endurance;
    [SerializeField]
    protected float _speed;

    protected float _initSpeed;

    protected Coroutine _contemplation, _drink, _eat;
    protected float _contemplateTime;

    private bool _reproducing;
    

    protected override void Awake()
    {
        base.Awake();
        _type = "Twippie";
        _name = "Petit twippie";
    }

    protected override void Start()
    {
        base.Start();
        _displayIntervals = 5;
        _basicNeed = BasicNeed.None;
        _advancedNeed = AdvancedNeed.None;
        _previousState = State.None;
        _goalType = GoalType.Wander;
        _knownZones = new List<Zone>();
        _lineRenderer = GetComponent<LineRenderer>();
        //_lineRenderer.enabled = false;
        _initSpeed = _speed;
        _outline.color = 3;
        _waterCost = 1;
        _endurance = 25+Random.value;
        _health = 100;
        _goalObject = new GameObject();
        _arrival = _goalObject.AddComponent<Arrival>();
        _arrival.ZoneManager = _zManager;
        _pathFinder.Destination = _arrival;
        SetDestination(GoalType.Wander);
        StartCoroutine(CheckSleepNeed());
        StartCoroutine(CheckBasicNeeds());
        
    }


    protected override void Update()
    {
        base.Update();

        _ageSize = _age / 40;
        if (!_mouseOver)
        {
            _currentSize = _initSize + Vector3.one * _ageSize;
        }
        else
        {
            _currentSize = _initSize * _sizeMultiplier + Vector3.one * _ageSize;
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
                _sleepiness = UpdateValue(_sleepiness, -3);
                break;


            case State.Walking:
                if (_pathFinder.Steps != null)
                {
                    if (_pathFinder.Steps.Count > 0)
                    {
                        for (int a = 0; a < _pathFinder.Steps.Count; a++)
                        {
                            _lineRenderer.SetPosition(a, _pathFinder.Steps[a].Zone.Center + ((_pathFinder.Steps[a].Zone.Center - _p.transform.position).normalized)/2);
                        }
                        _lineRenderer.SetPosition(_pathFinder.Steps.Count, transform.position+transform.up/2);
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
                            _lineRenderer.positionCount--;
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

                        }
                    }
                }
               
                break;
                
            case State.Contemplating:

                break;
        }

        if (_state != State.Sleeping)
        {
            _sleepiness = UpdateValue(_sleepiness);
        }
        _thirst = UpdateValue(_thirst, ((100 -_thirst+1)/100));
        _hunger = UpdateValue(_hunger, ((100 - _hunger+1)/50));
        
        _stats.StatToValue(_stats.StatsList[2]).Value = _age;
        _stats.StatToValue(_stats.StatsList[3]).Value = _hunger;
        _stats.StatToValue(_stats.StatsList[4]).Value = _thirst;
        _stats.StatToValue(_stats.StatsList[5]).Value = _sleepiness;
        

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (Time.frameCount % _displayIntervals == 0)
        {
            if (!_knownZones.Contains(_zone))
            {
                _knownZones.Add(_zone);
            }
            _zone = _zManager.GetZone(false, _zone, transform);
        }
    }

    private void OnStateChange()
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
                Debug.Log("On va pouvoir manger !");
                if (_eat == null)
                {
                    _eat = StartCoroutine(Eat(_arrival.FinishZone.Ressources.Find(x=>x.ressourceType == Ressource.RessourceType.Food).consumableObject)); 
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
        }
        else if (_hunger < 50 && _thirst < 50 && _sleepiness < 50)
        {
            _health = UpdateValue(_health);
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
            if (GetLight())
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

            if (_thirst >= 50 && _thirst > _hunger - 10 && _thirst > _sleepiness - 20)
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
        yield return null;
    }

    protected override void GenerateStats()
    {
        base.GenerateStats();
        
        _stats.StatsList[3] = new ValueStat(0, 0, 100, "hunger", true);
        _stats.StatsList[4] = new ValueStat(0, 0, 100, "thirst", true);
        _stats.StatsList[5] = new ValueStat(0, 0, 100, "fatigue", true);
        _stats.StatsList[6] = new LabelStat("Need :");
        _stats.StatsList[7] = new LabelStat("Emotion :");
        _stats.StatsList[8] = new LabelStat("Action :");
    }

    private void SetDestination(GoalType goal)
    {
        _goalObject.transform.parent = null;
        Zone zone = null;
        switch (goal)
        {
            case GoalType.Wander:
                zone = _zManager.GetRandomZoneByDistance(transform, checkAccessible: true, distanceMax: _endurance);
                if (zone != null)
                {
                    _goalObject.transform.position = zone.Center;
                    _goalType = goal;
                }
                else
                {
                    _goalObject.transform.position = transform.position;
                }
                break;
            case GoalType.Drink:
                zone = _zManager.GetRessourceZoneByDistance(transform, ressource: Ressource.RessourceType.Drink, checkTaken: true, distanceMax: _endurance);
                if (zone != null)
                {
                    zone.Accessible = true;
                    zone.Taken = true;
                    _goalObject.transform.position = zone.Center;
                    _goalType = goal;
                }
                else
                {
                    zone = _zManager.GetZoneByRessourceInList(transform, _knownZones, ressource: Ressource.RessourceType.Drink, checkTaken: true);
                    if (zone != null)
                    {
                        zone.Accessible = true;
                        zone.Taken = true;
                        _goalObject.transform.position = zone.Center;
                        _goalType = goal;
                    }
                    else
                    {
                        SetDestination(GoalType.Wander); // Or create conflict !!
                    }
                }
            break;
            case GoalType.Eat:
                zone = _zManager.GetRessourceZoneByDistance(transform, ressource: Ressource.RessourceType.Food, checkTaken: true, distanceMax: _endurance);
                if (zone != null)
                {
                    zone.Accessible = true;
                    zone.Taken = true;
                    _goalObject.transform.position = zone.Center;
                    _goalType = goal;

                }
                else
                {
                    zone = _zManager.GetZoneByRessourceInList(transform, _knownZones, ressource: Ressource.RessourceType.Food, checkTaken: true);
                    if (zone != null)
                    {
                        zone.Accessible = true;
                        zone.Taken = true;
                        _goalObject.transform.position = zone.Center;
                        _goalType = goal;
                    }
                    else
                    {
                        SetDestination(GoalType.Wander); // Or create conflict !!
                    }
                }
                break;
        }

        _goalObject.transform.parent = P.transform;
        _arrival.SetArrival();
        _pathFinder.FindPath();
        _lineRenderer.positionCount = _pathFinder.Steps.Count+1;
        _state = State.Walking;

    }

    private GoalType DefineGoal()
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
        while (_thirst > 0)
        {
            _thirst -= Time.deltaTime*_timeReference;
            yield return null;
        }
        _thirst = 0;
        SetDestination(DefineGoal());
        _drink = null;
    }

    private IEnumerator Eat(IConsumable consumable)
    {
        while (consumable.Consuming(_hunger))
        {
            _hunger -= Time.deltaTime*_timeReference;
            yield return null;
        }
        consumable.Consume();
        SetDestination(DefineGoal());
        _eat = null;
        
    }

    private void Die()
    {
        transform.Rotate(90, 0, 0);
        _renderer.material.color = Color.gray;
        _r.AddForce((transform.position - _p.transform.position).normalized, ForceMode.Impulse);
        _om.allObjects.Remove(this);
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

    public LineRenderer LineRenderer
    {
        get
        {
            return _lineRenderer;
        }
    }
}
