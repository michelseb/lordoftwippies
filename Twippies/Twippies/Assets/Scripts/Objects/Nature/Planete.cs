using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planete : ManageableObjet {

    [SerializeField]
    private float _gravity;
    [SerializeField]
    private Collider _objectDeplacementField;
    [SerializeField]
    private WaterObjet _water;
    [SerializeField]
    private Sun _sun;
    [SerializeField]
    private LayerMask _layerMask;
    private ZoneManager _zManager;
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private Vector3[] _originalVertices;
    private Dictionary<int, Vector3> _deformedVertices;
    private Vector3[] _vertexVelocities;
    private bool _shaping, _deforming;
    private int _displayMode;

    protected override void Awake()
    {
        base.Awake();
        _name = "Planète Twippie";
    }
    protected override void Start()
    {
        base.Start();
        _zManager = GetComponent<ZoneManager>();
        _outline.color = 2;
        _meshCollider = GetComponent<MeshCollider>();
        if (GetComponent<MeshFilter>() != null)
        {
            _mesh = GetComponent<MeshFilter>().mesh;
        }
        else
        {
            foreach (Transform child in transform)
            {
                _mesh = child.gameObject.GetComponent<MeshFilter>().mesh;
                if (_mesh != null)
                    continue;
            }
        }
        _originalVertices = _mesh.vertices;
        _deformedVertices = new Dictionary<int, Vector3>();
        _vertexVelocities = new Vector3[_originalVertices.Length];
    }

    protected override void Update()
    {
        base.Update();

        
        if (_c.ctrl != Controls.ControlMode.Dragging)
        {
            switch (_displayMode)
            {
                case 0:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.None;
                    }
                    break;
                case 1:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.Population;
                    }
                    break;
                case 2:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.Height;
                    }
                    break;
                case 5:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.Accessible;
                    }
                    break;
                case 6:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.Water;
                    }
                    break;
                case 7:
                    foreach (Zone z in _zManager.Zones)
                    {
                        z.Display = Zone.DisplayMode.Food;
                    }
                    break;
            }
        }
        if (_shaping)
        {

            if (_water.Coll.enabled)
            {
                _water.Coll.enabled = false;
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (Time.frameCount % _displayIntervals == 0)
                {
                    for (int i = 0; i < _deformedVertices.Count; i++)
                    {
                        UpdateVertex(i);
                    }
                    _deforming = true;
                    foreach (Vector3 point in _deformedVertices)
                    {

                    }
                    _mesh.vertices = _deformedVertices;
                    _mesh.RecalculateNormals();
                    _meshCollider.sharedMesh = _mesh;
                }
            }
            else
            {
                if (_deforming)
                {
                    _zManager.Vertices = _mesh.vertices;
                    _zManager.SetTriangles();
                    _zManager.GenerateZoneObjects();
                    _deforming = false;
                }
            }

        }
        else
        {
            if (!_water.Coll.enabled)
            {
                _water.Coll.enabled = true;
            }
        }

        
    }

    public void Attract(Transform t, Rigidbody r)
    {
        Vector3 gravityUp = (t.position - transform.position).normalized;
        Vector3 localUp = t.up;

        r.AddForce(-gravityUp * _gravity * r.mass);
        Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * t.rotation;
        t.rotation = targetRotation;//Quaternion.Slerp(t.rotation, targetRotation, 50f * Time.deltaTime);
    }

    public void Face(Transform t)
    {
        Vector3 gravityUp = (t.position - transform.position).normalized;
        Vector3 localUp = t.up;
        Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * t.rotation;
        t.rotation = targetRotation;//Quaternion.Slerp(t.rotation, targetRotation, 50f * Time.deltaTime);
    }

    protected override void OnMouseExit()
    {
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
    }

    protected override void OnMouseOver()
    {
        if (_shaping)
        {

            if
           (Input.mousePosition.x < Screen.width * 9 / 20 ||
           Input.mousePosition.x > Screen.width * 11 / 20 ||
           Input.mousePosition.y < Screen.height * 8 / 18 ||
           Input.mousePosition.y > Screen.height * 10 / 18)
            {
                AbsRotateObjet();
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(inputRay, out hit, float.MaxValue, _layerMask) && Input.GetMouseButton(0))
                {
                    Deform(hit.point, 10);
                }
                else if (Physics.Raycast(inputRay, out hit, float.MaxValue, _layerMask) && Input.GetMouseButton(1))
                {
                    Deform(hit.point, -10);
                }
            }
        }
    }

    protected override void OnMouseDrag()
    {
        if (!_shaping)
        {
            RotateObjet();
        }else
        {
            _c.FocusedObject = this;
            _c.ctrl = Controls.ControlMode.Checking;
        }
    }

    protected virtual void Deform(Vector3 point, float force)
    {
        point = transform.InverseTransformPoint(point);
        for (int i = 0; i < _deformedVertices.Length; i++)
        {
            AddForceToVertex(i, point, force);
        }
    }

    protected void AddForceToVertex(int i, Vector3 point, float force)
    {
        Vector3 pointToVertex = _deformedVertices[i] - point; //distance de chaque vertice au point touché
        Vector3 direction = transform.position - point; // distance du centre au point touché
        float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude); //Force appliquée au vertice selon la distance au point touché (plus distance courte, plus force élevée)
        float velocity = attenuatedForce * Time.deltaTime;
        if (Vector3.Distance(_deformedVertices[i], _originalVertices[i]) < 5) //Limite de magnitude de la déformation
            _vertexVelocities[i] += direction.normalized * velocity; //Direction opposée au centre, force selon attenuatedForce
    }

    protected void UpdateVertex(int i)
    {
        Vector3 velocity = _vertexVelocities[i]; // Récupération de la force appliquée au vertice
        velocity *= 1f - 5 * Time.deltaTime; //Réduction progressive de la vélocité pour éviter que ça rebondisse
        _vertexVelocities[i] = velocity;
        _deformedVertices[i] += velocity * Time.deltaTime;
    }


    public override void GenerateStats(StatPanel statPanel, string type)
    {
        base.GenerateStats(statPanel, type);
        statPanel.StatManager.GenerateStat<BoolStat>(type).Populate(true, "Don't press this", "Destroy");
        statPanel.StatManager.GenerateStat<BoolStat>(type).Populate(false, "Shape mode", "Shape");
        statPanel.StatManager.GenerateStat<ChoiceStat>(type).Populate("Display mode", new string[] { "None", "Population", "Height", "Needs", "Groups", "Access", "Water Access", "Food" }, 0, "Mode");

    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Destroy"), new object[] { true, "Don't press this", "Destroy" });
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Shape"), new object[] { false, "Shape mode", "Shape" });
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Mode"), new object[] { "Display mode", new string[] { "None", "Population", "Height", "Needs", "Groups", "Access", "Water Access", "Food" }, 0, "Mode" });
    }


    protected override void UpdateStats()
    {
        base.UpdateStats();
        bool shapingMode = _stats.StatToBool(_stats.GetStat("Shape")).Value;
        if (shapingMode && shapingMode != _shaping)
        {
            _mesh.MarkDynamic();
        }
        _shaping = shapingMode;
        _displayMode = _stats.StatToChoice(_stats.GetStat("Mode")).Value;
        if (_stats.StatToBool(_stats.GetStat("Destroy")).Value == false)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Objet>() is WaterObjet == false)
                    child.parent = null;
            }
            Destroy(gameObject);
        }

    }

    public bool Shaping
    {
        get
        {
            return _shaping;
        }
    }

    public WaterObjet Water
    {
        get
        {
            return _water;
        }
    }

    public Sun Sun
    {
        get
        {
            return _sun;
        }
    }

    public ZoneManager ZManager
    {
        get
        {
            return _zManager;
        }
    }
}
