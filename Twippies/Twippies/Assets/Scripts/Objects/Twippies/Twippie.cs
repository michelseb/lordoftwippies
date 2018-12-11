using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Twippie : DraggableObjet {

    protected enum State
    {
        Walking,
        Sleeping,
        Building,
        Contemplating
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

    protected GameObject _goalObject;
    protected Arrival _arrival;
    [SerializeField]
    protected PathFinder _pathFinder;
    protected State _state, _previousState;
    protected BasicNeed _basicNeed;
    protected AdvancedNeed _advancedNeed;

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

    protected Coroutine _contemplation;
    protected float _contemplateTime;
    private int _frameInterval = 5;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
        _basicNeed = BasicNeed.None;
        _advancedNeed = AdvancedNeed.None;
        _initSpeed = _speed;
        _outline.color = 3;
        _waterCost = 1;
        _goalObject = new GameObject();//GameObject.CreatePrimitive(PrimitiveType.Sphere);//new GameObject();
        _arrival = _goalObject.AddComponent<Arrival>();//GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //_goalObject.GetComponent<SphereCollider>().isTrigger = true;
        _arrival.ZoneManager = _zManager;
        _pathFinder.Destination = _arrival;
        SetGoal();
        StartCoroutine(CheckSleepNeed());
        
    }


    protected override void Update()
    {
        base.Update();

        _age += Time.deltaTime * _timeReference * .01f;
        if (_age > 100)
            _age = 100;
        _ageSize = _age / 20;
        if (!_mouseOver)
        {
            _currentSize = _initSize + Vector3.one * _ageSize;
        }
        else
        {
            _currentSize = _initSize * _sizeMultiplier + Vector3.one * _ageSize;
        }

        if (_planetSun != null)
        {
            if (true)
            {
                
            }
            else
            {
                
            }
        }
        else
        {
            if (_state != State.Sleeping)
            {
                _previousState = _state;
                _state = State.Sleeping;
                OnStateChange();
            }
        }

        if (_state != State.Walking && _state != State.Sleeping)
        {
            if (Vector3.Distance(transform.position, _goalObject.transform.position) > 1)
            {
                _previousState = _state;
                _state = State.Walking;
                OnStateChange();
            }
        }

        if (_state != State.Contemplating)
        {
            if (Vector3.Distance(transform.position, _goalObject.transform.position) <= .3f)
            {
                
                _previousState = _state;
                _state = State.Contemplating;
                OnStateChange();
            }
        }

        

        switch (_state)
        {
            case State.Sleeping:
                _sleepiness -= Time.deltaTime;
                if (_sleepiness < 0) _sleepiness = 0;
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
                            //Destroy(_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go);
                            _pathFinder.Steps.RemoveAt(_pathFinder.Steps.Count - 1);
                        }
                    }
                    else
                    {
                        _previousState = _state;
                        _state = State.Contemplating;
                        OnStateChange();
                    }
                }
               
                break;

            case State.Contemplating:

                break;
        }

        if (_state != State.Sleeping)
        {
            _sleepiness += Time.deltaTime;
            if (_sleepiness > 100)
            {
                _sleepiness = 100;
            }
        }

        
        _stats.StatToValue(_stats.StatsList[2]).Value = _age;
        _stats.StatToValue(_stats.StatsList[3]).Value = _hunger;
        _stats.StatToValue(_stats.StatsList[4]).Value = _thirst;
        _stats.StatToValue(_stats.StatsList[5]).Value = _sleepiness;
        

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        if (Time.frameCount % _frameInterval == 0)
            GetZone(false);
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
        RaycastHit hit;
        LayerMask mask = ~(1 << 12 | 1 << 9 | 1 << 2);
        if (_planetSun != null)
        {
            if (Physics.Linecast(_planetSun.transform.position, transform.position + transform.up, out hit, mask))
            {
                if (hit.collider.tag != "Planet" && hit.collider.tag != "Water")
                {
                    //Debug.DrawLine(_planetSun.transform.position, transform.position + transform.up, Color.red);
                    if (_state == State.Sleeping)
                    {
                        _state = _previousState;
                        OnStateChange();
                    }
                    yield return new WaitForSeconds(5);
                }
            }
        }
        if (_state != State.Sleeping)
        {
            _previousState = _state;
            _state = State.Sleeping;
            OnStateChange();
        }

        yield return new WaitForSeconds(5);
        
        
    }

    private IEnumerator CheckBasicNeeds()
    {
        if (_thirst >= 50)
        {
            _basicNeed = BasicNeed.Drink;
            yield return new WaitForSeconds(6);
        }
        if (_hunger >= 50)
        {
            _basicNeed = BasicNeed.Eat;
            yield return new WaitForSeconds(6);
        }
        if (_sleepiness >= 50)
        {
            _basicNeed = BasicNeed.Sleep;
            yield return new WaitForSeconds(6);
        }

        yield return new WaitForSeconds(6);

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

    private void SetGoal()
    {
        _goalObject.transform.parent = null;
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
        _goalObject.transform.parent = P.transform;
        _arrival.SetArrival();
        _pathFinder.FindPath();

    }

    private IEnumerator Contemplate(float temps)
    {
        yield return new WaitForSeconds(temps);
        SetGoal();
        _state = State.Walking;
        OnStateChange();
        _contemplation = null;
    }
}
