using System;
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
    [SerializeField]
    private float _deformSize;
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private Vector3[] _originalVertices, _newVertices;
    private SortedDictionary<int, Vector3> _deformedVertices;
    private bool _deforming;
    private Zone.DisplayMode _displayMode;

    public bool Shaping { get; private set; }
    public WaterObjet Water { get { return _water; } }
    public Sun Sun { get { return _sun; } }
    public ZoneManager ZManager { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        _name = "Planète Twippie";
    }
    protected override void Start()
    {
        base.Start();
        ZManager = GetComponent<ZoneManager>();
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
        _newVertices = _originalVertices;
        _deformedVertices = new SortedDictionary<int, Vector3>();
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Array values = Enum.GetValues(typeof(Zone.DisplayMode));
            System.Random random = new System.Random();
            _displayMode = (Zone.DisplayMode)values.GetValue(random.Next(values.Length));
        }

        if (_c.ctrl != Controls.ControlMode.Dragging)
        {
            foreach(Zone z in ZManager.Zones)
            {
                z.Display = _displayMode;
            }
        }
        if (Shaping)
        {
            if (!_mouseOver)
            {
                AbsRotateObjet();
            }

            if (_water.Coll.enabled)
            {
                _water.Coll.enabled = false;
            }

            if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)) && _deforming)
            {
                ZManager.Vertices = _mesh.vertices;
                ZManager.SetTriangles(_deformedVertices);
                _deformedVertices.Clear();
                _deforming = false;
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
        _mouseOver = false;
        if (_c.FocusedObject != this && !_c.FocusedObjects.Contains(this))
        {
            _outline.enabled = false;
        }
    }

    protected override void OnMouseOver()
    {
        _mouseOver = true;
        if (Shaping)
        {

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                _deforming = true;
                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(inputRay, out hit, float.MaxValue, _layerMask) && Input.GetMouseButton(0))
                {
                    Deform(transform.InverseTransformPoint(hit.point), 1, _deformSize);
                }
                else if (Physics.Raycast(inputRay, out hit, float.MaxValue, _layerMask) && Input.GetMouseButton(1))
                {
                    Deform(transform.InverseTransformPoint(hit.point), -1, _deformSize);
                }

                foreach(KeyValuePair<int, Vector3> vertice in _deformedVertices)
                {
                    _newVertices[vertice.Key] = vertice.Value;
                }
                _mesh.vertices = _newVertices;
                _mesh.RecalculateNormals();
                _meshCollider.sharedMesh = _mesh;

            }
        }
    }

    protected override void OnMouseDrag()
    {
        if (!Shaping)
        {
            RotateObjet();
        }else
        {
            _c.FocusedObject = this;
            _c.ctrl = Controls.ControlMode.Checking;
        }
    }

    protected virtual void Deform(Vector3 point, float force, float size = 0)
    {
        for (int i = 0; i < _originalVertices.Length; i++)
        {
            Vector3 pointToVertex = _originalVertices[i] - point; //distance de chaque vertice au point touché
            Vector3 direction = transform.position - point; // distance du centre au point touché
            if ((direction.magnitude < _water.Radius * 4 / 7 && Mathf.Sign(force) == 1)  || (direction.magnitude > _water.Radius && Mathf.Sign(force) == -1))
                continue;
            float attenuatedForce = (force - (pointToVertex.sqrMagnitude - size) * Mathf.Sign(force)) * Time.deltaTime; //Force appliquée au vertice selon la distance au point touché (plus distance courte, plus force élevée)
            if (attenuatedForce * Mathf.Sign(force) < 0)
                continue;
            Vector3 velocity = direction.normalized * attenuatedForce;
            velocity *= 1f - 10 * Time.deltaTime;
            if (!_deformedVertices.ContainsKey(i))
            {
                _deformedVertices.Add(i, _originalVertices[i] + velocity * .2f); //Direction opposée au centre, force selon attenuatedForce stocké dans la liste des vertices déformés
            }
            else
            {
                _deformedVertices[i] = _originalVertices[i] + velocity * .2f;
            }
        }
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
        if (shapingMode && shapingMode != Shaping)
        {
            _mesh.MarkDynamic();
        }
        Shaping = shapingMode;
        _displayMode = (Zone.DisplayMode)_stats.StatToChoice(_stats.GetStat("Mode")).Value;
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
}
