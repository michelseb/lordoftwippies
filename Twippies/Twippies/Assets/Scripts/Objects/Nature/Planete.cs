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
    private ZoneManager _zManager;
    private Mesh _mesh;
    private MeshCollider _meshCollider;
    private Vector3[] _originalVertices, _deformedVertices, _updatedVertices;
    private Vector3[] _vertexVelocities;
    private bool _shaping, _deforming;
    private int _displayIntervals = 3;

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
        _deformedVertices = new Vector3[_originalVertices.Length];
        _updatedVertices = new Vector3[_originalVertices.Length];
        for (int i = 0; i < _originalVertices.Length; i++)
        {
            _deformedVertices[i] = _originalVertices[i];
            _updatedVertices[i] = _originalVertices[i];
        }
        _vertexVelocities = new Vector3[_originalVertices.Length];
    }

    protected override void Update()
    {
        base.Update();

        _shaping = _stats.StatToBool(_stats.StatsList[3]).Value;
        if (_c.ctrl != Controls.ControlMode.Dragging)
        {
            switch (_stats.StatToChoice(_stats.StatsList[4]).Value)
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
                    _mesh.MarkDynamic();
                    for (int i = 0; i < _deformedVertices.Length; i++)
                    {
                        UpdateVertex(i);
                    }
                    _deforming = true;
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

        if (_stats.StatToBool(_stats.StatsList[2]).Value == false)
        {
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Objet>() is WaterObjet == false)
                    child.parent = null;
            }
            Destroy(gameObject);
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
        if (_c.FocusedObject != this)
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

                if (Physics.Raycast(inputRay, out hit) && Input.GetMouseButton(0))
                {
                    Deform(hit.point, 10);
                }
                else if (Physics.Raycast(inputRay, out hit) && Input.GetMouseButton(1))
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
        _updatedVertices[i] = _deformedVertices[i]; //Update du vertice si bouton appuyé
        //Vector3 deplacement = _deformedVertices[i] - _updatedVertices[i]; //Distance de déplacement du vertice depuis l'original
        //velocity -= deplacement *5* Time.deltaTime; //plus le point est déplacé, moins il va vite
        velocity *= 1f - 5 * Time.deltaTime; //Réduction progressive de la vélocité pour éviter que ça rebondisse
        _vertexVelocities[i] = velocity;
        _deformedVertices[i] += velocity * Time.deltaTime;
    }


    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[5];
        _stats.StatsList[0] = new LabelStat("Woaaaa (Planète)");
        _stats.StatsList[1] = new TextStat("Planète Twippie", 20);
        _stats.StatsList[2] = new BoolStat(true, "Don't press this");
        _stats.StatsList[3] = new BoolStat(false, "Shape mode");
        _stats.StatsList[4] = new ChoiceStat(new string[] { "None", "Population", "Height", "Needs", "Groups", "Access" }, 0);
        //_stats.StatsList[4] = new ValueStat(0, -30, 30, "Shape strength", false);

    }

    /*private void OnDrawGizmos()
    {
        if (_updatedVertices == null)
        {
            return;
        }
        if (Camera.current == Camera.main)
        {
            
            Gizmos.matrix = transform.localToWorldMatrix;
            for (int i = 0; i < _updatedVertices.Length; i++)
            {
                float col = (float)i / (float)(_updatedVertices.Length - 1);
                Gizmos.color = new Color(1, col, col);
                Gizmos.DrawSphere(_updatedVertices[i], col/2); // + (_updatedVertices[i] - transform.position).normalized
            }
        }
    }*/

    private void Mapping()
    {
        Vector3 center = transform.position;
        for (int i = 0; i < _updatedVertices.Length; i++)
        {

        }

    }

    public bool Shaping
    {
        get
        {
            return _shaping;
        }
    }

    public Vector3[] Vertices
    {
        get
        {
            return _updatedVertices;
        }
    }

    public WaterObjet Water
    {
        get
        {
            return _water;
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
