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
    protected Rigidbody _r;
    protected float _rotSpeedX, _rotSpeedY, _rotSpeedMultiplier = 10;
    protected cakeslice.Outline _outline;
    protected Collider _coll;
    protected Renderer _renderer;
    protected float _timeReference;
    protected bool _mouseOver;
    protected int _displayIntervals = 3;
    protected bool _isDeforming, _isDeformed;
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
        _originalVertices = _mesh.vertices.ToArray();
        _deformedVertices = _originalVertices.ToArray();
    }


    protected virtual void Update()
    {
        if (_isDeformed && _c.FocusedObject != this)
        {
            _r.isKinematic = false;
            StartCoroutine(Reform(1));
            _isDeformed = false;
        }
        _currentSize = SetCurrentSize();
        ScaleMe();
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
        _focusedSize = _currentSize;
    }


    protected virtual void OnMouseExit()
    {
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
        _mouseOver = false;
    }


    protected virtual void OnMouseUp()
    {
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
    }

    protected virtual void OnMouseDown()
    {
        SetFocus();
        if (!_isDeforming)
        {
            StartCoroutine(Deform(1));
        }
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
        if (_c.FocusedObject == this)
            return _focusedSize;

        return _initSize + Vector3.one * 1 / (_mouseProximity * 100 + .01f) * 5000 * _initSize.magnitude;
    }

    protected virtual void ScaleMe()
    {
        transform.localScale = _currentSize;
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

    public virtual void GenerateActions(ManageableObjet obj)
    {
        obj.Stats.GenerateAction<DescriptionAction>(obj);
    }

    public virtual void GenerateStats(StatPanel statPanel)
    {
        statPanel.StatManager.CreateSpecificPanel(statPanel.transform.Find("Mask").Find("Panel"));
        statPanel.StatManager.GenerateStat<ValueStat>(true).Populate(0, 0, 100, "Age", true, "Age");
        statPanel.StatManager.GenerateStat<DescriptionStat>(true).Populate(_icon, _name, 20, 14, "Description");
        statPanel.StatManager.GenerateStat<LabelStat>(true, "Titre").Populate(_type, "Titre");

    }

    public virtual void GenerateStatsForAction(UserAction action, StatManager statManager)
    {
        var subMenu = action.SubMenu;
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
        statManager.GenerateWorldStat<ProgressButtonStat>(subMenu).Populate(0, 0, 100, "Age", true, "Age");
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

    protected virtual IEnumerator Reform(float time)
    {
        while (_isDeforming)
        {
            yield return null;
        }
        if (_mouseOver)
            yield break;
        _isDeforming = true;
        var currTime = 0f;

        while (currTime < time)
        {
            for (int i = 0; i < _deformedVertices.Length; i++)
            {
                Vector3 direction = transform.InverseTransformPoint(transform.position) - _originalVertices[i];
                _deformedVertices[i].x = Mathf.Lerp(_deformedVertices[i].x, _originalVertices[i].x, currTime / time);
                _deformedVertices[i].y = Mathf.Lerp(_deformedVertices[i].y, _originalVertices[i].y, currTime / time);
                _deformedVertices[i].z = Mathf.Lerp(_deformedVertices[i].z, _originalVertices[i].z, currTime / time);
            }
            currTime += .1f * _timeReference;
            _mesh.vertices = _deformedVertices;
            yield return null;
        }

        _deformedVertices = _originalVertices.ToArray();
        _mesh.vertices = _originalVertices;
        _mesh.RecalculateNormals();
        if (_meshCollider != null)
        {
            _meshCollider.sharedMesh = _mesh;
        }
        _isDeforming = false;
    }

    protected virtual IEnumerator Deform(float time)
    {
        _r.isKinematic = true;
        _isDeformed = true;
        _isDeforming = true;
        var currTime = 0f;

        while (currTime < time)
        {
            for (int i = 0; i < _deformedVertices.Length; i++)
            {
                Vector3 direction = transform.InverseTransformPoint(transform.position) - _originalVertices[i];
                _deformedVertices[i].x = Mathf.Lerp(_originalVertices[i].x, _originalVertices[i].x * Mathf.Clamp(direction.magnitude, .1f, 10) * 2, currTime / time);
                _deformedVertices[i].y = Mathf.Lerp(_originalVertices[i].y, _originalVertices[i].y * Mathf.Clamp(direction.magnitude, .1f, 10), currTime / time);
                _deformedVertices[i].z = Mathf.Lerp(_originalVertices[i].z, _originalVertices[i].z * Mathf.Clamp(direction.magnitude, .1f, 10), currTime / time);
            }
            currTime += .1f * _timeReference;
            _mesh.vertices = _deformedVertices;
            yield return null;
        }
        
        _mesh.RecalculateNormals();
        if (_meshCollider != null)
        {
            _meshCollider.sharedMesh = _mesh;
        }
        _isDeforming = false;
    }

}
