using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twippie : DraggableObjet {

    protected enum State
    {
        Walking,
        Sleeping,
        Building
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

    protected Vector3 _goal;
    protected List<Zone> _steps;
    protected State _state;
    private Sun _s;

    [SerializeField]
    private float _sleepiness;
    [SerializeField]
    private float _hunger;
    [SerializeField]
    private float _thirst;

    [SerializeField]
    protected float _speed;

    protected override void Awake()
    {
        base.Awake();

    }

    protected override void Start()
    {
        base.Start();
        _s = _p.transform.GetComponentInChildren<Sun>();
        _outline.color = 3;
        _waterCost = 1;
    }


    protected override void Update()
    {
        base.Update();

        if (_s != null)
        {
            if (!CheckActiveState())
            {
                if (_state != State.Sleeping)
                {
                    _state = State.Sleeping;
                    OnStateChange(_state);
                }
            }
            else
            {
                if (_state == State.Sleeping)
                {
                    _state = State.Walking;
                    OnStateChange(_state);
                }
            }
        }
        else
        {
            if (_state != State.Sleeping)
            {
                _state = State.Sleeping;
                OnStateChange(_state);
            }
        }

        switch (_state)
        {
            case State.Sleeping:
                _sleepiness -= Time.deltaTime;
                if (_sleepiness < 0) _sleepiness = 0;
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
        switch (_state) {

            case State.Walking:
                Vector3 newPos = _r.position + transform.TransformDirection(new Vector3(0, 0, _speed * Time.deltaTime));
                _r.MovePosition(newPos);
                break;


            case State.Sleeping:
                
                break;
        }
    }

    private void OnStateChange(State s)
    {
        switch (s)
        {
            case State.Sleeping:
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.black;
                break;
            case State.Walking:
                gameObject.GetComponentInChildren<MeshRenderer>().material.color = Color.white;
                break;
        }
    }

    private bool CheckActiveState()
    {
        RaycastHit hit;
        LayerMask mask = ~(1 << 12 | 1 << 9);
        if (Physics.Linecast(_s.transform.position, transform.position + transform.up/2, out hit, mask))
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

    }
}
