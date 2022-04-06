using System.Linq;
using UnityEngine;

public abstract class ManageableObjet : Objet
{

    [SerializeField] protected ObjectStatManager _stats;
    [SerializeField] protected string _type;
    [SerializeField] protected Sprite _icon;
    [SerializeField] protected int _woodCost, _waterCost, _stoneCost;

    protected ObjetManager _objectManager;
    protected ZoneManager _zoneManager;
    protected ObjectGenerator _objectGenerator;

    protected Mesh _mesh;
    protected MeshCollider _meshCollider;
    protected MeshFilter _meshFilter;
    protected Vector3 _initSize, _currentSize, _focusedSize;

    protected Camera _camera;
    protected string _name;
    protected float _ageProgression;
    protected Controls _controls;
    protected Rigidbody _rigidBody;
    protected float _rotSpeedX, _rotSpeedY, _rotSpeedMultiplier = 10;
    protected cakeslice.Outline _outline;
    protected Collider _coll;
    protected Renderer _renderer;
    protected float _timeReference;
    protected bool _mouseOver;

    public Collider Coll => _coll;
    public ObjectStatManager Stats => _stats;
    public bool Grown { get; protected set; }
    public int Age
    {
        get
        {
            var newAge = (int)_ageProgression;
            if (newAge != Age)
            {
                Grown = true;
            }

            return newAge;
        }
        set
        {
            _ageProgression = value;
        }
    }

    public string Type { get { return _type; } }
    //protected float _mouseProximity { get { return Vector3.Distance(_camera.WorldToScreenPoint(transform.position), Input.mousePosition); } }
    public int WoodCost => _woodCost;
    public int WaterCost => _waterCost;
    public int StoneCost => _stoneCost;
    public Sprite Icon => _icon;
    public Vector3 CurrentSize { get { return _currentSize; } set { _currentSize = value; } }

    protected override void Awake()
    {
        base.Awake();

        _objectManager = ObjetManager.Instance;
        _objectGenerator = ObjectGenerator.Instance;
        _zoneManager = ZoneManager.Instance;
        _camera = Camera.main;
        _mesh = GetComponentInChildren<MeshFilter>().mesh;
        _meshCollider = GetComponentInChildren<MeshCollider>();
        _meshFilter = GetComponentInChildren<MeshFilter>();

        _objectManager.AddObject(this);
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
                foreach (Transform child in transform)
                {
                    if (child.gameObject.GetComponent<Renderer>() != null && child.gameObject.GetComponent<cakeslice.Outline>() == null)
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

    protected virtual void Start()
    {
        _initSize = transform.lossyScale;
        _currentSize = _initSize;

        _outline.enabled = false;
        //_originalVertices = _mesh.vertices.ToArray();
        //_deformedVertices = _originalVertices.ToArray();
    }


    protected virtual void Update()
    {
        //_currentSize = SetCurrentSize();
        //ScaleMe();
        if (!_outline.enabled)
        {
            if (_controls.FocusedObjects.Contains(this))
            {
                _outline.enabled = true;
            }
        }

        transform.RotateAround(transform.position, _camera.transform.up, -_rotSpeedX * _rotSpeedMultiplier);
        transform.RotateAround(transform.position, _camera.transform.right, _rotSpeedY * _rotSpeedMultiplier);
        _rotSpeedX = Mathf.Lerp(_rotSpeedX, 0, .05f);
        _rotSpeedY = Mathf.Lerp(_rotSpeedY, 0, .05f);

        if (Input.GetButtonDown("Fire1"))
        {
            if (_controls.FocusedObject != this && _outline.enabled && Input.mousePosition.x < Screen.width * 2 / 3)
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

        _ageProgression = UpdateValue(_ageProgression, _timeReference * .01f);
        //STATS DO NOT DELETE
        //UpdateStats();

        if (Grown)
        {
            UpdateAge();
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

    //protected virtual Vector3 SetCurrentSize()
    //{
    //    if (_controls.FocusedObject == this)
    //        return _focusedSize;

    //    //return _initSize + Vector3.one * 1 / (_mouseProximity * 100 + .01f) * 5000 * _initSize.magnitude;
    //}

    protected virtual void ScaleMe()
    {
        //transform.localScale = _currentSize;
    }

    protected virtual void ColorMe()
    {
        if (Utils.IsNull(_renderer) || !_renderer.material.HasProperty("_Color"))
            return;

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
        _rotSpeedX = rotationVector.x * .1f;
        _rotSpeedY = rotationVector.y * .1f;
    }

    protected void SetFocus()
    {
        _controls.FocusedObject = this;
        //var screenPosition = _camera.WorldToScreenPoint(transform.position);
        //_posX = Input.mousePosition.x - screenPosition.x;
        //_posY = Input.mousePosition.y - screenPosition.y;
    }

    protected float UpdateValue(float value, float factor, int minVal = 0, int maxVal = 100)
    {
        value += Time.deltaTime * factor * _timeReference;
        if (value < minVal)
            value = minVal;
        if (value > maxVal)
            value = maxVal;

        return value;
    }

    protected Vector3 UpdateVector(Vector3 value, float factor = 1, int minSize = 0, int maxSize = 100)
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
        Stats.GenerateWorldStat<ValueStat>().Populate("Age", 0, 0, 100, "Age", true);
        //statManager.GenerateWorldStat<DescriptionStat>(action).Populate(_icon, _name, 20, 14, "Description");
        //statManager.GenerateWorldStat<LabelStat>(action).Populate(_type, "Titre");
    }

    public virtual void PopulateStats()
    {
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Age"), new object[] { _age, 0, 100, "Age", true, "Age" });
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Description"), new object[] { _icon, _name, 20, 14, "Description" });
        //_og.MainPanel.PopulateStatPanel(_stats.GetStat("Titre"), new object[] { _type, "Titre" });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Age", _ageProgression, 0, 100, "Age", true });
    }

    public void GetStatManager()
    {
        _stats = _objectManager.StatManagers.FirstOrDefault(x => x.Type == _type);
    }

    private Bounds GetViewportBounds(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = _camera.nearClipPlane;
        max.z = _camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public bool IsWithinSelectionBounds()
    {
        var viewportBounds =
            GetViewportBounds(_controls.OriginClic, Input.mousePosition);

        return viewportBounds.Contains(
            _camera.WorldToViewportPoint(gameObject.transform.position));
    }

    public void SetSelectionActive(bool active)
    {
        _outline.enabled = active;
    }

    protected virtual void UpdateStats()
    {
        if (_stats == null)
            return;

        var ageStat = _stats.GetStat("Age");

        if (ageStat == null)
            return;

        _stats.StatToValue(ageStat).Value = _ageProgression;
    }

    protected virtual void UpdateAge()
    {
        Grown = false;
    }

}
