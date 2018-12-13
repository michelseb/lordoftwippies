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
        Sleep,
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

    private float _age;
    private float _ageSize;

    [SerializeField]
    protected float _speed;

    protected float _initSpeed;

    protected Coroutine _contemplation, _drink, _eat;
    protected float _contemplateTime;
    

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        _displayIntervals = 5;
        base.Start();
        _basicNeed = BasicNeed.None;
        _advancedNeed = AdvancedNeed.None;
        _previousState = State.None;
        _initSpeed = _speed;
        _outline.color = 3;
        _waterCost = 1;
        _goalObject = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Sphere);//new GameObject();
        _arrival = _goalObject.AddComponent<Arrival>();//GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //_goalObject.GetComponent<SphereCollider>().isTrigger = true;
        _arrival.ZoneManager = _zManager;
        _pathFinder.Destination = _arrival;
        SetGoal(GoalType.Wander);
        StartCoroutine(CheckSleepNeed());
        StartCoroutine(CheckBasicNeeds());
        
    }


    protected override void Update()
    {
        base.Update();

        _age = UpdateValue(_age, _timeReference * .01f);  
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
                _sleepiness = UpdateValue(_sleepiness, -1);
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
                    _drink = StartCoroutine(Drink(4f));
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
                        if (_sleepiness >= 50)
                        {
                            ChangeState(State.Sleeping);
                        }
                    }
                    Debug.Log(hit.collider.gameObject.name);
                    yield return new WaitForSeconds(2);
                }
                else
                {
                    Debug.Log("Previous : "+_previousState + " Current : " + _state);
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
        _stats.StatsList = new Stat[10];
        _stats.StatsList[0] = new LabelStat("Twippie");
        _stats.StatsList[1] = new TextStat("Petit Twippie", 20);
        _stats.StatsList[2] = new ValueStat(0, 0, 100, "age", false);
        _stats.StatsList[3] = new ValueStat(0, 0, 100, "hunger", true);
        _stats.StatsList[4] = new ValueStat(0, 0, 100, "thirst", true);
        _stats.StatsList[5] = new ValueStat(0, 0, 100, "fatigue", true);
        _stats.StatsList[6] = new LabelStat("Need :");
        _stats.StatsList[7] = new LabelStat("Emotion :");
        _stats.StatsList[8] = new LabelStat("Action :");
    }

    private void SetGoal(GoalType goal)
    {
        _goalObject.transform.parent = null;
        _goalType = goal;
        switch (goal)
        {
            case GoalType.Wander:
                int zoneId = Random.Range(0, _p.ZManager.Zones.Length - 1);

                if (!_p.ZManager.Zones[zoneId].Accessible)
                {
                    for (int a = 0; a < 100; a++)
                    {
                        zoneId = Random.Range(0, _p.ZManager.Zones.Length - 1);// Choisit une zone aléatoire
                        if (_p.ZManager.Zones[zoneId].Accessible)
                            break;
                    }
                }
                _goalObject.transform.position = _p.ZManager.Zones[zoneId].Center;// Place le goal en son centre

                break;
            case GoalType.Drink:
                WaterZone zone = (WaterZone)GetZone(false, _zManager.DrinkZones.ToArray());
                zone.Accessible = true;
                zone.taken = false;
                _goalObject.transform.position = zone.Center;
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
            case BasicNeed.Sleep:
                return GoalType.Sleep;
        }
        switch (_advancedNeed)
        {
            case AdvancedNeed.Socialize:
                return GoalType.Socialize;
        }
        return GoalType.Wander;
    }

    private float UpdateValue(float value, float factor = 1)
    {
        value += Time.deltaTime * factor;
        if (value < 0)
            value = 0;
        if (value > 100)
            value = 100;

        return value;
    }

    private void ChangeState(State state)
    {
        if (_state != state)
        {
            _previousState = _state;
        }
        _state = state;
        OnStateChange();
    }

    private IEnumerator Contemplate(float temps)
    {
        yield return new WaitForSeconds(temps);
        SetGoal(DefineGoal());
        ChangeState(State.Walking);
        _contemplation = null;
    }

    private IEnumerator Drink(float temps)
    {
        Debug.Log("Drinking");
        yield return new WaitForSeconds(temps);
        _thirst = 0;
        SetGoal(DefineGoal());
        ChangeState(State.Walking);
        _drink = null;
    }

}
