﻿using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public abstract class ManageableObjet : Objet {

    [SerializeField]
    protected StatManager _stats;

    [SerializeField]
    protected string _type;
    protected string _name;
    protected float _age;
    protected Controls _controls;
    protected Rigidbody _r;
    protected float _rotSpeedX, _rotSpeedY, _rotSpeedMultiplier = 10;
    protected cakeslice.Outline _outline;
    protected Collider _coll;
    protected Renderer _renderer;
    protected float _timeReference;
    protected bool _mouseOver;
    protected int _displayIntervals = 3;
    protected Vector3[] _originalVertices, _deformedVertices;

    public Collider Coll { get { return _coll; } }
    public StatManager Stats { get { return _stats; } }
    public float Age { get { return _age; } set { _age = value; } }
    public string Type { get { return _type; } }
    protected float _mouseProximity { get { return Vector3.Distance(_cam.WorldToScreenPoint(transform.position), Input.mousePosition); } }

    protected override void Awake()
    {
        base.Awake();
        _om.UpdateObjectList(this, true);
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
        _controls = Controls.Instance;
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
        _stats = null;
        _originalVertices = _mesh.vertices.ToArray();
        _deformedVertices = _originalVertices.ToArray();
    }


    protected virtual void Update()
    {
        _currentSize = SetCurrentSize();
        ScaleMe();
        if (!_outline.enabled)
        {
            if (_controls.FocusedObjects.Contains(this))
            {
                _outline.enabled = true;
            }
        }
        
        transform.RotateAround(transform.position, _cam.transform.up, -_rotSpeedX * _rotSpeedMultiplier);
        transform.RotateAround(transform.position, _cam.transform.right, _rotSpeedY * _rotSpeedMultiplier);
        _rotSpeedX = Mathf.Lerp(_rotSpeedX, 0, .05f);
        _rotSpeedY = Mathf.Lerp(_rotSpeedY, 0, .05f);

        if (Input.GetButtonDown("Fire1"))
        {
            if (_controls.FocusedObject != this && _outline.enabled && Input.mousePosition.x < Screen.width * 2/3)
            {
                _outline.enabled = false;
            }
        }

        if (Input.GetButton("Fire3"))
        {
            if (_controls.FocusedObjects.Contains(this))
            {
                if (!IsWithinSelectionBounds())
                {
                    _controls.FocusedObjects.Remove(this);
                    _outline.enabled = false;
                }
            }

            if (!_controls.FocusedObjects.Contains(this))
            {
                if (IsWithinSelectionBounds())
                {
                    if (_renderer.isVisible)//if (!Physics.Linecast(_cam.transform.position, gameObject.transform.position + gameObject.transform.up))
                    {
                        _controls.FocusedObjects.Add(this);
                    }
                }
            }
        }

        _age = UpdateValue(_age, _timeReference * .01f);
        if (_stats != null)
        {
            UpdateStats();
        }

    }

    protected virtual void LateUpdate()
    {
        ColorMe();
    }


    protected virtual void OnMouseEnter()
    {
        _outline.enabled = true;
        _focusedSize = _currentSize;
    }


    protected virtual void OnMouseExit()
    {
        if (_controls.FocusedObject != this && !_controls.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
        _mouseOver = false;
    }


    protected virtual void OnMouseUp()
    {
        if (_controls.FocusedObject != this && !_controls.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
    }

    protected virtual void OnMouseDown()
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

    protected virtual Vector3 SetCurrentSize()
    {
        if (_controls.FocusedObject == this)
            return _focusedSize;

        return _initSize + Vector3.one * 1 / (_mouseProximity * 100 + .01f) * 5000 * _initSize.magnitude;
    }

    protected virtual void ScaleMe()
    {
        transform.localScale = _currentSize;
    }

    protected virtual void ColorMe()
    {
        if (_renderer.material.HasProperty("_Color"))
        {
            var col = _renderer.material.color;
            if ((_controls.FocusedObject != null && _controls.FocusedObject != this) || (_controls.FocusedObjects.Count > 0 && !_controls.FocusedObjects.Contains(this)))
            {
                _renderer.material.color = new Color(col.r, col.g, col.b, .2f);
            }
            else
            {
                _renderer.material.color = new Color(col.r, col.g, col.b, 1);
            }
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

    protected void SetFocus()
    {
        
        _controls.FocusedObject = this;
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

    public virtual void GenerateActions()
    {
        Stats.GenerateRadialAction<DescriptionAction>(this);
        GenerateStatsForActions();
        Stats.LinkStatsToAction(Type);
    }

    public virtual void GenerateStatsForActions()
    {
        Stats.GenerateWorldStat<ValueStat>().Populate(0, 0, 100, "Age", true, "Age");
        //statManager.GenerateWorldStat<DescriptionStat>(action).Populate(_icon, _name, 20, 14, "Description");
        //statManager.GenerateWorldStat<LabelStat>(action).Populate(_type, "Titre");
    }

    public virtual void PopulateStats()
    {
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Age"), new object[] { _age, 0, 100, "Age", true, "Age" });
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Description"), new object[] { _icon, _name, 20, 14, "Description" });
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Titre"), new object[] { _type, "Titre" });
        _og.RadialPanel.PopulateStatPanel(_stats.GetStat("Age"), new object[] { _age, 0, 100, "Age", true, "Age" });
    }

    public void GetStatManager()
    {
        _stats = _om.StatManagers.FirstOrDefault(x => x.Type == _type);
    }

    private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = _cam.nearClipPlane;
        max.z = _cam.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public bool IsWithinSelectionBounds()
    {
        var viewportBounds =
            GetViewportBounds(_controls.OriginClic, Input.mousePosition);

        return viewportBounds.Contains(
            _cam.WorldToViewportPoint(gameObject.transform.position));
    }

    public void SetSelectionActive(bool active)
    {
        _outline.enabled = active;
    }

    protected virtual void UpdateStats()
    {
        _stats.StatToValue(_stats.GetStat("Age")).Value = _age;
    }

}
