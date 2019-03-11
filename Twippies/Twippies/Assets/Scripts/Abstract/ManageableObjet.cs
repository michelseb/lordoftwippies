using System.Collections.Generic;
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
        //_om.StartCoroutine((_om.WaitFor(_stats, ()=> GenerateStats())));
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
        _stats = null;
    }


    protected virtual void Update()
    {
        if (!_outline.enabled)
        {
            if (_c.FocusedObjects.Contains(this))
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
            if (_c.FocusedObject != this && _outline.enabled && Input.mousePosition.x < Screen.width * 2/3)
            {
                _outline.enabled = false;
            }
        }

        if (Input.GetButton("Fire3"))
        {
            if (_c.FocusedObjects.Contains(this))
            {
                if (!IsWithinSelectionBounds())
                {
                    _c.FocusedObjects.Remove(this);
                    _outline.enabled = false;
                }
            }

            if (!_c.FocusedObjects.Contains(this))
            {
                if (IsWithinSelectionBounds())
                {
                    if (_renderer.isVisible)//if (!Physics.Linecast(_cam.transform.position, gameObject.transform.position + gameObject.transform.up))
                    {
                        _c.FocusedObjects.Add(this);
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


    protected virtual void OnMouseEnter()
    {
        _outline.enabled = true;
    }


    protected virtual void OnMouseExit()
    {
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
        transform.localScale = _initSize;
        _mouseOver = false;
    }


    protected virtual void OnMouseUp()
    {
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
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

    public virtual void GenerateStats(StatPanel statPanel, string type)
    {
        statPanel.StatManager.CreateSpecificPanel(statPanel.transform.Find("Mask").Find("Panel"));
        statPanel.StatManager.GenerateStat<ValueStat>(type, true).Populate(0, 0, 100, "Age", true, "Age");
        statPanel.StatManager.GenerateStat<DescriptionStat>(type, true).Populate(_icon, _name, 20, 14, "Description");
        statPanel.StatManager.GenerateStat<LabelStat>(type, true, "Titre").Populate(_type, "Titre");

    }

    public virtual void PopulateStats()
    {
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Age"), new object[] { _age, 0, 100, "Age", true, "Age" });
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Description"), new object[] { _icon, _name, 20, 14, "Description" });
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Titre"), new object[] { _type, "Titre" });
    }

    public void GetStatManager()
    {
        _stats = _og.MainPanel.StatPanels.FirstOrDefault(x => x.Type == _type)?.StatManager;
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
            GetViewportBounds(_c.OriginClic, Input.mousePosition);

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

    public Collider Coll { get { return _coll; } }
    public StatManager Stats { get { return _stats; } }
    public float Age { get { return _age; } set { _age = value; } }
    public string Type { get { return _type; } }
}
