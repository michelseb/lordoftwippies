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
        Content
    }

    protected enum Need
    {
        Eat,
        Drink,
        Socialize,
        Sleep,
        Move,
        Warmup,
        Cooldown
    }

    protected GameObject _goalObject;
    protected Arrival _arrival;
    [SerializeField]
    protected PathFinder _pathFinder;
    protected State _state, _previousState;
    private Sun _sun;

    [SerializeField]
    private float _sleepiness;
    [SerializeField]
    private float _hunger;
    [SerializeField]
    private float _thirst;

    [SerializeField]
    protected float _speed;

    protected Coroutine _contemplation;
    protected float _contemplateTime;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
        _sun = _p.transform.GetComponentInChildren<Sun>();
        _outline.color = 3;
        _waterCost = 1;
        _goalObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);//new GameObject();
        _arrival = _goalObject.AddComponent<Arrival>();//GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _goalObject.GetComponent<SphereCollider>().isTrigger = true;
        _arrival.ZoneManager = _zManager;
        _pathFinder.Destination = _arrival;
        SetGoal();
        
    }


    protected override void Update()
    {
        base.Update();


        if (_state != State.Walking)
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

        if (_sun != null)
        {
            if (!CheckActiveState())
            {
                if (_state != State.Sleeping)
                {
                    _previousState = _state;
                    _state = State.Sleeping;
                    OnStateChange();
                }
            }
            else
            {
                if (_state == State.Sleeping)
                {
                    _state = _previousState;
                    OnStateChange();
                }
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
                        transform.rotation = rotation * transform.rotation;
                        Vector3 newPos = _r.position + transform.TransformDirection(new Vector3(0, 0, _speed * Time.deltaTime));
                        _r.MovePosition(newPos);
                        if (direction.magnitude < 1)
                        {
                            Destroy(_pathFinder.Steps[_pathFinder.Steps.Count - 1].Go);
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

        _stats.StatToValue(_stats.StatsList[3]).Value = _hunger;
        _stats.StatToValue(_stats.StatsList[4]).Value = _thirst;
        _stats.StatToValue(_stats.StatsList[5]).Value = _sleepiness;

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        
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

    private bool CheckActiveState()
    {
        RaycastHit hit;
        LayerMask mask = ~(1 << 12 | 1 << 9);
        if (Physics.Linecast(_sun.transform.position, transform.position + transform.up/2, out hit, mask))
        {
            
            if (hit.collider.tag == "Planet" || hit.collider.tag == "Water")
            {
                return false;
            } 
            else
            {
                //Debug.DrawLine(_s.transform.position, transform.position + transform.up/2, Color.red);
                return true;
            }
        }
        else
        {
            //Debug.DrawLine(_s.transform.position, transform.position + transform.up/2, Color.red);
            return true;
        }
        
        
    }

    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[9];
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
        int zoneId = Random.Range(0, _p.ZManager.Zones.Count - 1);
        if (!_p.ZManager.Zones[zoneId].Accessible)
        {
            for (int a = 0; a < 100; a++)
            {
                zoneId = Random.Range(0, _p.ZManager.Zones.Count - 1);// Choisit une zone aléatoire
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
