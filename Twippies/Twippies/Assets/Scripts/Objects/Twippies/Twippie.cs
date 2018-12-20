using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Twippie : DraggableObjet {

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
    [SerializeField]
    protected PathFinder _pathFinder;
    protected State _state, _previousState;
    protected BasicNeed _basicNeed;
    protected AdvancedNeed _advancedNeed;
    protected GoalType _goalType;
    [SerializeField]
    private LayerMask _mask;

    [SerializeField]
    private float _sleepiness;
    [SerializeField]
    private float _hunger;
    [SerializeField]
    private float _thirst;

    private float _ageSize;
    private float _endurance;
    [SerializeField]
    protected float _speed;

    protected float _initSpeed;

    protected Coroutine _contemplation, _drink, _eat;
    protected float _contemplateTime;
    

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
        _initSpeed = _speed;
        _outline.color = 3;
        _waterCost = 1;
        _endurance = 30+Random.value;
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
                        Vector3 direction = _pathFinder.Steps[_pathFinder.Steps.Count-1].Zone.Center - transform.position;
                        Quaternion rotation = Quaternion.FromToRotation(transform.forward, direction);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation * transform.rotation, Time.deltaTime * 10);
                        Vector3 newPos = _r.position + transform.TransformDirection(new Vector3(0, 0, _speed * Time.deltaTime));
                        _r.MovePosition(newPos); 
                        if (direction.magnitude < 1)
                        {
                            if (_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go != null)
                                Destroy(_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go);
                            _pathFinder.Steps.RemoveAt(_pathFinder.Steps.Count - 1);
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
        _thirst = UpdateValue(_thirst, 1.5f);
        _hunger = UpdateValue(_hunger, .5f);
        
        _stats.StatToValue(_stats.StatsList[2]).Value = _age;
        _stats.StatToValue(_stats.StatsList[3]).Value = _hunger;
        _stats.StatToValue(_stats.StatsList[4]).Value = _thirst;
        _stats.StatToValue(_stats.StatsList[5]).Value = _sleepiness;
        

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (Time.frameCount % _displayIntervals == 0)
            _zone = GetZone(false);
    }

    private void OnStateChange()
    {
        switch (_state)
        {
            case State.Sleeping:
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.black;
                break;
            case State.Walking:
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                break;
            case State.Drinking:
                if (_drink == null)
                {
                    _drink = StartCoroutine(Drink());
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
        _speed = 0;
    }

    protected override void OnMouseExit()
    {
        base.OnMouseExit();
        _speed = _initSpeed;
    }

    private IEnumerator CheckSleepNeed()
    {
        while (true)
        {
            RaycastHit hit;
            if (_planetSun != null)
            {
                if (Physics.Linecast(_planetSun.transform.position, transform.position + transform.up/2, out hit, _mask))
                {
                    if (_state != State.Sleeping)
                    {
                        if (_basicNeed == BasicNeed.Sleep)
                        {
                            ChangeState(State.Sleeping);
                        }
                    }
                    Debug.Log(hit.collider.gameObject.name);
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
        
    }

    private IEnumerator CheckBasicNeeds()
    {
        while (true)
        {

            while (_thirst >= 50)
            {
                _basicNeed = BasicNeed.Drink;
                yield return new WaitForSeconds(3);
            }
            while (_hunger >= 50)
            {
                _basicNeed = BasicNeed.Eat;
                yield return new WaitForSeconds(3);
            }
            while (_sleepiness >= 50)
            {
                _basicNeed = BasicNeed.Sleep;
                yield return new WaitForSeconds(3);
            }

            _basicNeed = BasicNeed.None;
            yield return new WaitForSeconds(3);
        }

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
        _goalType = goal;
        Zone zone = null;
        switch (goal)
        {
            case GoalType.Wander:
                zone = GetRandomZoneByDistance(distanceMax: _endurance);
                if (zone != null)
                {
                    _goalObject.transform.position = zone.Center;
                }else
                {
                    _goalObject.transform.position = transform.position;
                }
                break;
            case GoalType.Drink:
                zone = GetRandomZoneByDistance(zoneList : _zManager.DrinkZones.ToArray(), distanceMax: _endurance);
                if (zone != null)
                {
                    zone.Accessible = true;
                    zone.Taken = false;
                    _goalObject.transform.position = zone.Center;
                }
                else
                {
                    SetDestination(GoalType.Wander); // Or create conflict !!
                }
                break;
        }

        
        _goalObject.transform.parent = P.transform;
        _arrival.SetArrival();
        _pathFinder.FindPath();

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
        }
        _state = state;
        Debug.Log("Previous : " + _previousState + " Current : " + _state);
        OnStateChange();
    }

    private IEnumerator Contemplate(float temps)
    {
        yield return new WaitForSeconds(temps);
        SetDestination(DefineGoal());
        _contemplation = null;
    }

    private IEnumerator Drink()
    {
        while (_thirst > 0)
        {
            _thirst -= 1;
            yield return null;
        }
        _thirst = 0;
        SetDestination(DefineGoal());
        _drink = null;
    }

    private IEnumerator Eat(TreeObjet tree)
    {
        while (tree.Size.x > 0)
        {
            tree.Size -= Vector3.one * Time.deltaTime;
        }
        yield return null;
    }

}
