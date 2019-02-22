using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using UnityEngine.UI;

public abstract class ManageableObjet : Objet {

    [SerializeField]
    protected StatManager _stats;


    protected string _type;
    protected string _name;
    protected float _age;
    protected Controls _c;
    protected float _rotSpeedX, _rotSpeedY, _rotSpeedMultiplier = 10;
    protected float _sizeMultiplier = 2.3f;
    protected cakeslice.Outline _outline;
    protected Collider _coll;
    protected Renderer _renderer;
    protected float _timeReference;
    protected bool _mouseOver;
    protected int _displayIntervals = 3;

    

    protected override void Awake()
    {
        base.Awake();
        _coll = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        if (_coll == null)
        {
            foreach (Transform child in transform)
            {
                _coll = child.GetComponent<Collider>();
                _renderer = child.GetComponent<Renderer>();
                if (_coll != null)
                {
                    continue;
                }
            }
        }
        _c = Controls.Instance;
        _outline = GetComponent<cakeslice.Outline>();
        if (_outline == null)
        {
            if (GetComponent<Collider>() != null)
            {
                _outline = gameObject.AddComponent<cakeslice.Outline>();
            }
            else
            {
                foreach(Transform child in transform)
                {
                    if (child.gameObject.GetComponent<Collider>() != null && child.gameObject.GetComponent<cakeslice.Outline>() == null)
                    {
                        _outline = child.gameObject.AddComponent<cakeslice.Outline>();
                    }
                    else if (child.gameObject.GetComponent<cakeslice.Outline>() != null)
                    {
                        _outline = child.gameObject.GetComponent<cakeslice.Outline>();
                    }
                }
            }
        }
        if (_outline == null)
        {
            gameObject.AddComponent<BoxCollider>();
            _outline = gameObject.AddComponent<cakeslice.Outline>();
        }
    }

    protected override void Start()
    {
        base.Start();
        _outline.enabled = false;
        _stats.enabled = false;
    }


    protected virtual void Update()
    {
        
        transform.RotateAround(transform.position, _cam.transform.up, -_rotSpeedX * _rotSpeedMultiplier);
        transform.RotateAround(transform.position, _cam.transform.right, _rotSpeedY * _rotSpeedMultiplier);
        _rotSpeedX = Mathf.Lerp(_rotSpeedX, 0, .05f);
        _rotSpeedY = Mathf.Lerp(_rotSpeedY, 0, .05f);

        if (Input.GetMouseButtonDown(0))
        {
            if (_c.FocusedObject != this && _outline.enabled && Input.mousePosition.y > Screen.height * 1/3)
            {
                _outline.enabled = false;
            }
        }

        _age = UpdateValue(_age, _timeReference * .01f);
        if (_stats.enabled)
        {
            UpdateStats();
        }

    }


    protected virtual void OnMouseEnter()
    {
        _outline.enabled = true;
    }


    protected virtual void OnMouseExit()
    {
        if (_c.FocusedObject != this)
        {
            _outline.enabled = false;
        }
        transform.localScale = _initSize;
        _mouseOver = false;
    }


    protected virtual void OnMouseUp()
    {
        if (_c.FocusedObject != this)
        {
            _outline.enabled = false;
        }
        transform.localScale = _initSize;
    }

    private void OnMouseDown()
    {
        SetFocus();
    }

    protected virtual void OnMouseOver()
    {
        _mouseOver = true;
        if (Input.GetMouseButtonDown(1))
        {
            SetFocus();
        }
    }

    protected virtual void OnMouseDrag()
    {
        RotateObjet();
    }

    public void RotateObjet()
    {
        _rotSpeedX = Input.GetAxis("Mouse X");
        _rotSpeedY = Input.GetAxis("Mouse Y");
    }

    public void AbsRotateObjet()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 rotationVector = (screenCenter - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).normalized;
        _rotSpeedX = rotationVector.x*.1f;
        _rotSpeedY = rotationVector.y*.1f;
    }

    public Collider Coll
    {
        get
        {
            return _coll;
        }
    }

    public StatManager Stats
    {
        get
        {
            return _stats;
        }

        set
        {
            _stats = value;
        }
    }

    public float Age
    {
        get
        {
            return _age;
        }
        set
        {
            _age = value;
        }
    }

    private void SetFocus()
    {
        
        _c.FocusedObject = this;
        _dist = _cam.WorldToScreenPoint(transform.position);
        _posX = Input.mousePosition.x - _dist.x;
        _posY = Input.mousePosition.y - _dist.y;
    }

    protected float UpdateValue(float value, float factor = 1, int minVal = 0, int maxVal = 100)
    {
        value += Time.deltaTime * factor * _timeReference;
        if (value < minVal)
            value = minVal;
        if (value > maxVal)
            value = maxVal;

        return value;
    }

    protected Vector3 UpdateVector(Vector3 value, float factor = 1,int minSize = 0, int maxSize = 100)
    {
        value += Vector3.one * Time.deltaTime * factor * _timeReference;
        if (value.x < minSize)
            value = Vector3.one * minSize;
        if (value.x > maxSize)
            value = Vector3.one * maxSize;

        return value;
    }

    public virtual void GenerateStats()
    {
        _stats.StatsList = new List<Stat>();
        _stats.GenerateStat<ValueStat>(true).Populate(0, 0, 100, "age", false);
        _stats.GenerateStat<TextStat>(true).Populate(_name, 20, 14);
        _stats.GenerateStat<LabelStat>(true).Populate(_type);
        
        
    }

    protected virtual void UpdateStats()
    {
        _stats.StatToValue(_stats.StatsList[2]).Value = _age;
    }
}
