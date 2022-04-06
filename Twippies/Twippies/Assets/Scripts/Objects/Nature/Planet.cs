using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : ManageableObjet
{
    [SerializeField, Range(2, 256)] private int _resolution = 10;
    [SerializeField] private Material _material;
    [SerializeField] private RadialMenu _mainRadial;
    [SerializeField] private float _gravity;
    [SerializeField] private Collider _objectDeplacementField;
    [SerializeField] private WaterObjet _water;
    [SerializeField] private Sun _sun;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _deformSize;
    [SerializeField, Range(0, 1)] private float _deformSpeed;
    [SerializeField, Range(0, .1f)] private float _deformStep;
    [SerializeField, HideInInspector] private MeshFilter[] _meshFilters;
    [SerializeField] private bool _smoothDeform;
    [SerializeField, Range(0, 1)] private float _noiseAmount;

    private TerrainFace[] _terrainFaces;
    private DisplayMode _displayMode;
    public DisplayMode Display => _displayMode;

    public float DeformSize => _deformSize;
    public bool Deforming { get; set; }
    public bool Shaping { get; private set; }
    public WaterObjet Water { get { return _water; } }
    public Sun Sun { get { return _sun; } }
    public RadialMenu MainRadial { get { return _mainRadial; } }

    private PlanetManager _planetManager;
    private VertexManager _vertexManager;
    private TwippieManager _twippieManager;

    protected Guid[] _originalVertices;
    private Dictionary<Guid, Vector3> _newVertices;
    private Dictionary<Guid, Vector3> _deformedVertices;

    private float _deformMagnitude;

    private const int FACES_AMOUNT = 6;
    public const string MESH_NAME = "mesh";

    protected override void Awake()
    {
        base.Awake();

        _name = "Planète Twippie";
        _planetManager = PlanetManager.Instance;
        _planetManager.Add(this);
        _vertexManager = GetComponent<VertexManager>();
        _twippieManager = TwippieManager.Instance;
    }
    protected override void Start()
    {
        base.Start();

        _outline.Color = 2;

        Initialize();
        GenerateMesh();
        _mesh = Combine();
        AutoWeld(_mesh, .01f, .02f);
        AddNoise();
        _vertexManager.Initialize();
        _zoneManager.Initialize();

        _originalVertices = _vertexManager.Vertices;
        _newVertices = new Dictionary<Guid, Vector3>();

        for (int i = 0; i < _originalVertices.Length; i++)
        {
            _newVertices.Add(Utils.IntToGuid(i), _vertexManager.GetPosition(_originalVertices[i]));
        }
        _deformedVertices = new Dictionary<Guid, Vector3>();

        _objectGenerator.Generate();
    }


    private void Initialize()
    {
        if (_meshFilters == null || _meshFilters.Length == 0)
        {
            _meshFilters = new MeshFilter[FACES_AMOUNT];
        }

        _terrainFaces = new TerrainFace[FACES_AMOUNT];

        var directions = new Vector3[] { Vector3.up, Vector3.down, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

        for (var i = 0; i < FACES_AMOUNT; i++)
        {
            var meshObject = new GameObject(MESH_NAME);
            meshObject.transform.parent = transform;
            meshObject.layer = LayerMask.NameToLayer("Planet");
            meshObject.tag = "Planet";

            var rigidbody = meshObject.AddComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            meshObject.AddComponent<MeshRenderer>().sharedMaterial = _material;
            _meshFilters[i] = meshObject.AddComponent<MeshFilter>();

            var outline = meshObject.AddComponent<cakeslice.Outline>();
            outline.Color = 2;
            _meshFilters[i].sharedMesh = new Mesh();

            _terrainFaces[i] = new TerrainFace(meshObject, _meshFilters[i], _resolution, directions[i]);

            meshObject.AddComponent<TerrainFaceDisplay>().Face = _terrainFaces[i];
        }
    }

    private Mesh Combine()
    {
        // Combine meshes into 1
        var combine = new CombineInstance[FACES_AMOUNT];

        var meshFilters = GetComponentsInChildren<MeshFilter>().Where(x => x.gameObject.name == MESH_NAME).ToArray();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
        }

        var mesh = new Mesh();
        _meshFilter.mesh = mesh;
        _meshFilter.mesh.CombineMeshes(combine, true, false);
        _meshCollider.sharedMesh = mesh;
        transform.gameObject.SetActive(true);
        //_mesh.RecalculateNormals();

        return mesh;
    }

    private void AddNoise()
    {
        var vertices = _mesh.vertices.ToArray();
        var octave = UnityEngine.Random.Range(1, 5);
        var offset = UnityEngine.Random.Range(0, 1000);

        for (int i = 0; i < FACES_AMOUNT; i++)
        {
            for (int x = 0; x < _resolution; x++)
            {
                for (int y = 0; y < _resolution; y++)
                {
                    //var noise = Mathf.PerlinNoise((float)x / _resolution, (float)y / _resolution) * 2;
                    var indice = y + _resolution * x + _resolution * _resolution * i;

                    if (indice >= vertices.Length)
                        continue;

                    var noise = Perlin.Fbm(vertices[indice], octave, offset);//Perlin.Perlin3D(vertices[indice], offset);//Perlin.Fbm(vertices[indice], 100);

                    vertices[indice] += Vector3.one * noise * _noiseAmount;//* Perlin.Noise(vertices[indice]);
                }
            }
        }

        _mesh.vertices = vertices;
        //_mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh;
        _meshFilter.sharedMesh = _mesh;
    }

    private void GenerateMesh()
    {
        foreach (var face in _terrainFaces)
        {
            face.Build();
        }
    }


    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var values = Enum.GetValues(typeof(DisplayMode));
            System.Random random = new System.Random();
            _displayMode = (DisplayMode)values.GetValue(random.Next(values.Length));
        }

        if (_controls.CurrentContolMode != ControlMode.Dragging)
        {
            foreach (var zone in _zoneManager.Zones)
            {
                zone.SetDisplayMode(_displayMode);
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
        }
        else
        {
            if (!_water.Coll.enabled)
            {
                _water.Coll.enabled = true;
            }
        }
    }

    public void Face(Transform t)
    {
        Vector3 gravityUp = (t.position - transform.position).normalized;
        Vector3 localUp = t.up;
        t.rotation = Quaternion.FromToRotation(localUp, gravityUp) * t.rotation;
        //t.rotation = Quaternion.Slerp(t.rotation, targetRotation, 50f * Time.deltaTime);
    }

    protected override void OnMouseOver()
    {
        _mouseOver = true;

        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;



            if (Physics.Raycast(inputRay, out hit, float.MaxValue, 1 << LayerMask.NameToLayer("Planet")))
            {

                var force = Input.GetMouseButton(1) ? -1 : 1;

                var deformPosition = transform.InverseTransformPoint(hit.point);
                if (!Deforming)
                {
                    _deformMagnitude = Utils.GetNearestMultiple((deformPosition - transform.position).magnitude, _deformStep, force);
                    Deforming = true;
                }
                //DeformSmooth(transform.InverseTransformPoint(hit.point), force);
                Deform(deformPosition, _deformMagnitude);
            }

            ModifyShape();
        }
        else if (Deforming)
        {
            FinalizeShape();
            _deformedVertices.Clear();
        }
    }

    protected override void OnMouseDrag()
    {
        if (!Shaping)
        {
            RotateObjet();
        }
        else
        {
            _controls.FocusedObject = this;
            _controls.SetControlMode(ControlMode.Checking);
        }
    }

    public virtual void DeformSmooth(Vector3 deformLocation, int direction)
    {
        var deformDirection = deformLocation - transform.position;

        foreach (var vertexId in _originalVertices)
        {
            var vertexPosition = _vertexManager.GetPosition(vertexId);
            var vertexDirection = vertexPosition - transform.position;

            if (Vector3.Dot(deformDirection, vertexDirection) < 0)
                continue;

            var distanceFromDeformPoint = (vertexPosition - deformLocation).sqrMagnitude;

            if ((Mathf.Sign(direction) == 1 && deformDirection.magnitude > _water.Radius * 2) ||
                (Mathf.Sign(direction) == -1 && deformDirection.magnitude < _water.Radius * 2 / 3))
                continue;

            var force = direction * (DeformSize - distanceFromDeformPoint * distanceFromDeformPoint);

            if (force * direction < 0)
                continue;

            var velocity = deformDirection.normalized * force * _deformSpeed;
            //velocity *= 1f - 10 * Time.deltaTime;

            var newPos = vertexPosition + velocity;

            if (!_deformedVertices.ContainsKey(vertexId))
            {
                _deformedVertices.Add(vertexId, newPos);
            }
            else
            {
                _deformedVertices[vertexId] = newPos;
            }

            _vertexManager.UpdateVertex(vertexId, newPos);
        }
    }

    public virtual void Deform(Vector3 deformLocation, float magnitude)
    {
        var deformDirection = deformLocation - transform.position;

        foreach (var vertexId in _originalVertices)
        {
            var vertexPosition = _vertexManager.GetPosition(vertexId);
            var vertexDirection = vertexPosition - transform.position;

            if (Vector3.Dot(deformDirection, vertexDirection) < 0)
                continue;

            var distanceFromDeformPoint = (vertexPosition - deformLocation).sqrMagnitude;
            if (distanceFromDeformPoint > DeformSize)
                continue;

            //if ((Mathf.Sign(direction) == 1 && deformDirection.magnitude > _water.Radius * 2) ||
            //    (Mathf.Sign(direction) == -1 && deformDirection.magnitude < _water.Radius * .5f))
            //    continue;

            //var force = direction * DeformSize;

            //var velocity = deformDirection.normalized * force;

            var newPos = vertexPosition.normalized * magnitude;

            if (!_deformedVertices.ContainsKey(vertexId))
            {
                _deformedVertices.Add(vertexId, newPos);
            }
            else
            {
                _deformedVertices[vertexId] = newPos;
            }

            _vertexManager.UpdateVertex(vertexId, newPos);
        }
    }

    public virtual void Deform(Zone deformZone, int neighboursAmount)
    {
        var deformLocation = deformZone.transform.position;
        var deformDirection = deformLocation - transform.position;

        var zonesToDeform = _zoneManager
            .FindNeighboursRecursive(deformZone.Id, new List<Guid>(), neighboursAmount)
            .Select(x => _zoneManager.FindById(x))
            .ToList();

        var verticesToDeform = zonesToDeform.SelectMany(z => z.VerticeIds).Distinct().ToList();

        foreach (var verticeIndex in verticesToDeform)
        {
            var position = _vertexManager.GetPosition(verticeIndex);
            if (!_deformedVertices.ContainsKey(verticeIndex))
            {
                _deformedVertices.Add(verticeIndex, position + deformDirection.normalized * .2f);
            }
            else
            {
                _deformedVertices[verticeIndex] = position + deformDirection.normalized * .2f;
            }
        }
    }

    private void ModifyShape()
    {
        foreach (var vertice in _deformedVertices)
        {
            _newVertices[vertice.Key] = vertice.Value;
        }
        _mesh.vertices = _newVertices.Values.ToArray();
        _originalVertices = _newVertices.Keys.ToArray();
        _mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh;
        _meshFilter.sharedMesh = _mesh;
    }

    public void FinalizeShape()
    {
        Deforming = false;
        _deformedVertices.Clear();
        _zoneManager.SetZoneInfos();
        _twippieManager.RefreshAllPaths();
        //_zoneManager.SurfaceVertices = _mesh.vertices;
        //_zoneManager.SetTriangles(_deformedVerticesDictionnary);
        //_deformedVerticesDictionnary.Clear();
    }

    protected override void OnMouseDown()
    {
        SetFocus();
    }

    //protected override Vector3 SetCurrentSize()
    //{
    //    return _initSize;
    //}

    public override void GenerateActions()
    {
        Stats.GenerateRadialAction<AddAction>(this);
        Stats.GenerateRadialAction<ModificationAction>(this);
        base.GenerateActions();
    }

    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<BoolStat>().Populate("Destroy", true, "Don't press this", true);
        Stats.GenerateWorldStat<BoolStat>().Populate("Shape", false, "Shape mode", false);
        Stats.GenerateWorldStat<ChoiceStat>().Populate("Mode", "Display mode", new string[] { "None", "Population", "Height", "Needs", "Groups", "Access", "Water Access", "Food" }, 0, false);

    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Destroy", true, "Don't press this", false });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Shape", false, "Shape mode", false });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Mode", "Display mode", new string[] { "None", "Population", "Height", "Needs", "Groups", "Access", "Water Access", "Food" }, 0, false });
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
        _displayMode = (DisplayMode)_stats.StatToChoice(_stats.GetStat("Mode")).Value;
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


    private void AutoWeld(Mesh mesh, float threshold, float bucketStep)
    {
        var oldVertices = mesh.vertices;
        var newVertices = new Vector3[oldVertices.Length];
        int[] old2new = new int[oldVertices.Length];
        int newSize = 0;

        // Find AABB
        var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < oldVertices.Length; i++)
        {
            if (oldVertices[i].x < min.x) min.x = oldVertices[i].x;
            if (oldVertices[i].y < min.y) min.y = oldVertices[i].y;
            if (oldVertices[i].z < min.z) min.z = oldVertices[i].z;
            if (oldVertices[i].x > max.x) max.x = oldVertices[i].x;
            if (oldVertices[i].y > max.y) max.y = oldVertices[i].y;
            if (oldVertices[i].z > max.z) max.z = oldVertices[i].z;
        }

        // Make cubic buckets, each with dimensions "bucketStep"
        var bucketSizeX = Mathf.FloorToInt((max.x - min.x) / bucketStep) + 1;
        var bucketSizeY = Mathf.FloorToInt((max.y - min.y) / bucketStep) + 1;
        var bucketSizeZ = Mathf.FloorToInt((max.z - min.z) / bucketStep) + 1;
        var buckets = new List<int>[bucketSizeX, bucketSizeY, bucketSizeZ];

        // Make new vertices
        for (int i = 0; i < oldVertices.Length; i++)
        {
            // Determine which bucket it belongs to
            var x = Mathf.FloorToInt((oldVertices[i].x - min.x) / bucketStep);
            var y = Mathf.FloorToInt((oldVertices[i].y - min.y) / bucketStep);
            var z = Mathf.FloorToInt((oldVertices[i].z - min.z) / bucketStep);

            // Check to see if it's already been added
            if (buckets[x, y, z] == null)
                buckets[x, y, z] = new List<int>(); // Make buckets lazily

            for (int j = 0; j < buckets[x, y, z].Count; j++)
            {
                var to = newVertices[buckets[x, y, z][j]] - oldVertices[i];
                if (Vector3.SqrMagnitude(to) < threshold)
                {
                    old2new[i] = buckets[x, y, z][j];
                    goto skip; // Skip to next old vertex if this one is already there
                }
            }

            // Add new vertex
            newVertices[newSize] = oldVertices[i];
            buckets[x, y, z].Add(newSize);
            old2new[i] = newSize;
            newSize++;

            skip:;
        }

        // Make new triangles
        var oldTris = mesh.triangles;
        var newTris = new int[oldTris.Length];

        for (int i = 0; i < oldTris.Length; i++)
        {
            newTris[i] = old2new[oldTris[i]];
        }

        var finalVertices = new Vector3[newSize];

        for (int i = 0; i < newSize; i++)
        {
            finalVertices[i] = newVertices[i];
        }

        mesh.Clear();
        mesh.vertices = finalVertices;
        mesh.triangles = newTris;
        mesh.normals = finalVertices.Select(v => (v - transform.position).normalized).ToArray();
        mesh.Optimize();
    }
}
