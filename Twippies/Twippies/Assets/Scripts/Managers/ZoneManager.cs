using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using System;

public class ZoneManager : MonoBehaviour, IManager<Zone>
{
    private static ZoneManager _instance;
    public static ZoneManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ZoneManager>(); return _instance; } }

    [SerializeField] private Zone _zonePrefab;
    [SerializeField] private float _zoneSize;
    [SerializeField] private float _zoneAmount;
    [SerializeField, Range(0, 1)] private float _acceptableSlope;

    private VertexManager _vertexManager;
    private Dictionary<Guid, Zone> _zoneDictionary = new Dictionary<Guid, Zone>();
    public Zone[] Zones { get; private set; }
    private int[] _triangles;
    private Mesh _planetMesh;
    private int _nbVertex;
    public Color[] Colors { get; set; }
    public Planet Planet { get; private set; }
    private bool _initialized;

    private void Awake()
    {
        _vertexManager = GetComponent<VertexManager>();
        Planet = GetComponent<Planet>();
        Colors = new Color[_nbVertex];
    }

    private void Start()
    {
        //for (int surfaceIndex = 0; surfaceIndex < AllTriangles.Count; surfaceIndex++)
        //{
        //    int triangleIndex = 0;
        //    for (int i = 0; i < AllTriangles[surfaceIndex].Length; i++)
        //    {
        //        var currentVertices = AllVertices[surfaceIndex];

        //        if (triangleIndex + 2 >= currentVertices.Length)
        //            continue;

        //        Zone zone = null;
        //        try
        //        {
        //            zone = Instantiate(_zonePrefab, (currentVertices[triangleIndex] + currentVertices[triangleIndex + 1] + currentVertices[triangleIndex + 2]) / 3, Quaternion.identity, transform);
        //        }
        //        catch(Exception e)
        //        {
        //            Debug.Log($"Impossible to create zone at surface index {surfaceIndex}, vertexIndex {i}: " + e);
        //        }

        //        zone.VerticeIds = new List<int> { triangleIndex, triangleIndex + 1, triangleIndex + 2 };
        //        triangleIndex += 3;

        //        tempZones.Add(zone);
        //    }
        //}


        //for (int i = 0; i < _zoneAmount; i++)
        //{
        //    TryFindCenter(_zoneSize, tempZones);
        //}

        // Display zones (for debugging)
        //foreach (var zone in Zones)
        //{
        //    var center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    center.GetComponent<Collider>().isTrigger = true;
        //    center.GetComponent<MeshRenderer>().material.color = zone.Resources.Any(x => x.ResourceType == ResourceType.Drink) ? Color.yellow : zone.Accessible ? Color.blue : Color.red;
        //    center.transform.localScale = Vector3.one * .1f;
        //    center.transform.position = zone.transform.position;
        //    center.transform.SetParent(Planet.transform);
        //}

    }

    public void Initialize()
    {
        _planetMesh = GetComponent<MeshFilter>().mesh;
        _triangles = _planetMesh.triangles;
        _nbVertex = _planetMesh.vertexCount;

        var tempZones = new List<Zone>();

        int triangleIndex = 0;
        for (int i = 0; i < _triangles.Length; i++)
        {
            if (triangleIndex + 2 >= _triangles.Length)
                continue;

            Zone zone = null;
            try
            {
                var v1 = Utils.IntToGuid(_triangles[triangleIndex]);
                var v2 = Utils.IntToGuid(_triangles[triangleIndex + 1]);
                var v3 = Utils.IntToGuid(_triangles[triangleIndex + 2]);

                var pos1 = _vertexManager.GetPosition(v1);
                var pos2 = _vertexManager.GetPosition(v2);
                var pos3 = _vertexManager.GetPosition(v3);

                zone = Instantiate(_zonePrefab, (pos1 + pos2 + pos3) / 3, Quaternion.identity, transform);

                zone.VerticeIds = new List<Guid> { v1, v2, v3 };

                triangleIndex += 3;
            }
            catch (Exception e)
            {
                Debug.Log($"Impossible to create zone at vertexIndex {i}: " + e);
            }

            tempZones.Add(zone);
        }

        _zoneDictionary = tempZones.ToDictionary(x => x.Id, x => x);
        Zones = tempZones.ToArray();
        SetZoneInfos();
        //InitTriangles();
        FindNeighbours();
        _initialized = true;
    }

    //private bool TryFindCenter(float minDist, List<Zone> zones)
    //{
    //    int id = UnityEngine.Random.Range(0, _nbVertex - 1);
    //    var pos = transform.TransformPoint(SurfaceVertices[id]);
    //    foreach (var z in zones)
    //    {
    //        float dist = (pos - z.WorldPos).magnitude;
    //        if (dist < minDist)
    //        {
    //            return false;
    //        }
    //    }

    //    var zone = Instantiate(_zonePrefab, pos, Quaternion.identity, transform);
    //    zones.Add(zone);
    //    //var center = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //    //center.GetComponent<Collider>().isTrigger = true;
    //    //center.GetComponent<MeshRenderer>().material.color = Color.red;
    //    //center.transform.localScale = Vector3.one * .2f;
    //    //center.transform.position = zone.transform.position;
    //    //center.transform.SetParent(Planet.transform);

    //    return true;
    //}

    public void FindNeighbours()
    {
        foreach (var zone in Zones)
        {
            zone.NeighbourIds.Clear();
            foreach (var otherZone in Zones)
            {
                if (otherZone.Id == zone.Id)
                    continue;

                if (zone.VerticeIds.Any(x => otherZone.VerticeIds.Contains(x)))
                {
                    zone.AddNeighbour(otherZone.Id);
                }
            }
        }
    }

    public List<Guid> FindNeighboursRecursive(Guid originId, List<Guid> results, int amount)
    {
        if (amount <= 0 || results == null)
            return results;

        var zone = FindById(originId);
        var neighbours = new List<Guid>();

        results.AddRange(zone.NeighbourIds.Where(n => !results.Contains(n)));
        amount--;

        neighbours.ForEach(n => FindNeighboursRecursive(n, results, amount));

        return results;
    }

    //public void SetTriangles(SortedDictionary<int, Guid> deformedVertices)
    //{
    //    var zonesToUpdate = new List<Zone>();

    //    foreach (int vertexId in deformedVertices.Keys.ToList())
    //    {
    //        //deformedVertices[vertexId] = transform.TransformPoint(_planetMeshes.vertices[vertexId]);

    //        var zone = Zones.FirstOrDefault(x => x.VerticeIds.Contains(vertexId));
    //        if (zone != null && !zonesToUpdate.Contains(zone))
    //        {
    //            foreach (int vertex in zone.VerticeIds)
    //            {
    //                if (!deformedVertices.Keys.Contains(vertex))
    //                {
    //                    //deformedVertices.Add(vertex, transform.TransformPoint(_planetMeshes.vertices[vertex]));
    //                }
    //            }

    //            zonesToUpdate.Add(zone);
    //            //SetColors();
    //        }

    //    }


    //    SetZoneInfos(zonesToUpdate.ToArray());
    //}


    //public void InitTriangles()
    //{
    //    foreach (var zone in Zones)
    //    {
    //        zone.transform.SetParent(transform);
    //    }


    //    for (int surfaceIndex = 0; surfaceIndex < AllTriangles.Count; surfaceIndex++)
    //    {
    //        var surfaceTriangles = _planetMeshes[surfaceIndex].triangles;

    //        for (int i = 0; i < surfaceTriangles.Length - 3; i += 3)
    //        {
    //            var currentVertices = AllVertices[surfaceIndex];
    //            //currentVertices[i] = transform.TransformPoint(_planetMeshes.vertices[a]);
    //            Vector3 a = currentVertices[i];
    //            Vector3 b = currentVertices[i + 1];
    //            Vector3 c = currentVertices[i + 2];

    //            Vector3 centre = (a + b + c) / 3;

    //            var closestZone = Zones.OrderBy(zone => (centre - zone.WorldPos).sqrMagnitude).FirstOrDefault();

    //            if (!closestZone.VerticeIds.Contains(surfaceTriangles[i]))
    //            {
    //                Colors[surfaceTriangles[i]] = closestZone.Color;
    //                closestZone.VerticeIds.Add(surfaceTriangles[i]);
    //            }
    //            if (!closestZone.VerticeIds.Contains(surfaceTriangles[i + 1]))
    //            {
    //                Colors[surfaceTriangles[i + 1]] = closestZone.Color;
    //                closestZone.VerticeIds.Add(surfaceTriangles[i + 1]);
    //            }
    //            if (!closestZone.VerticeIds.Contains(surfaceTriangles[i + 2]))
    //            {
    //                Colors[surfaceTriangles[i + 2]] = closestZone.Color;
    //                closestZone.VerticeIds.Add(surfaceTriangles[i + 2]);
    //            }

    //        }
    //    }

    //    SetZoneInfos();
    //    AllVertices = _planetMeshes.Select(m => m.vertices).ToList();

    //}

    public IEnumerator GetZoneByRessourceInList(Guid currentZone, List<Guid> zoneIds, ResourceType resource, Action<Zone> onComplete, PathFinder pathFinder = null, bool checkTaken = false)
    {
        yield return new WaitUntil(() => _initialized);

        var zone = zoneIds
            .Select(zoneId => FindById(zoneId))
            .Where(z => !checkTaken || !z.Taken)
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefault(x => x.HasResource(resource));

        if (zone == null)
        {
            onComplete(null);
            yield break;
        }

        if (pathFinder != null)
        {
            yield return StartCoroutine(pathFinder.FindPath(currentZone, zone.Id, z =>
            {
                onComplete(z);
            }));
        }
    }

    public IEnumerator GetRandomZoneByDistance(Guid currentZone, Transform transform, PathFinder pathFinder, bool checkTaken = false, float distanceMax = float.MaxValue, Action<Zone> onComplete = default)
    {
        yield return new WaitUntil(() => _initialized);

        var zone = Zones
            .Where(z => z.Accessible &&
                (!checkTaken || !z.Taken) &&
                (transform.position - z.WorldPos).sqrMagnitude < distanceMax)
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefault();

        if (zone == null)
        {
            onComplete(null);
            yield break;
        }

        yield return StartCoroutine(pathFinder.FindPath(currentZone, zone.Id, z =>
        {
            onComplete(z);
        }));
    }

    public IEnumerator GetLargeZoneByDistance(Guid currentZone, Transform transform, PathFinder pathFinder, bool checkTaken = false, float distanceMax = float.MaxValue, Action<Zone> onComplete = default)
    {
        yield return new WaitUntil(() => _initialized);

        var zone = Zones
            .Where(z => z.Accessible &&
                (!checkTaken || !z.Taken) &&
                (transform.position - z.WorldPos).sqrMagnitude < distanceMax &&
                z.NeighbourIds.Select(n => FindById(n)).All(x => !x.Taken && x.Accessible))
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefault();

        if (zone == null)
        {
            onComplete(null);
            yield break;
        }

        yield return StartCoroutine(pathFinder.FindPath(currentZone, zone.Id, z =>
        {
            zone.NeighbourIds.ForEach(n => FindById(n).Taken = true);
            onComplete(z);
        }));
    }

    public IEnumerator GetRessourceZoneByDistance(Guid currentZone, Transform transform, PathFinder pathFinder, ResourceType resource, bool checkTaken = false, float distanceMax = float.MaxValue, Action<Zone> onComplete = default)
    {
        yield return new WaitUntil(() => _initialized);

        var zone = Zones
            .Where(z => z.Accessible &&
                (!checkTaken || !z.Taken) &&
                (transform.position - z.WorldPos).sqrMagnitude < distanceMax)
            .OrderBy(x => Guid.NewGuid())
            .FirstOrDefault(x => x.HasResource(resource));

        if (zone == null)
        {
            onComplete(null);
            yield break;
        }

        yield return StartCoroutine(pathFinder.FindPath(currentZone, zone.Id, z =>
        {
            onComplete(z);
        }));
    }



    public Guid GetZone(bool take, Guid currentZoneId, Vector3 currentPosition)
    {
        if (!_initialized)
            return default;

        var currentZone = FindById(currentZoneId);

        if (take && currentZone != null)
        {
            currentZone.Accessible = true;
        }

        var possibleZones = currentZone != null ?
            currentZone.NeighbourIds.Concat(new Guid[] { currentZoneId }).Select(n => FindById(n)) :
            Zones;

        var result = possibleZones
            .OrderBy(zone => (currentPosition - zone.WorldPos).sqrMagnitude)
            .FirstOrDefault();

        if (take && result != null)
        {
            result.Accessible = false;
        }

        return result.Id;
    }


    public Zone GetAerialZone(Transform transform)
    {
        if (Physics.Linecast(transform.position, Planet.transform.position, out RaycastHit hit, 1 << 16, QueryTriggerInteraction.Collide))
        {
            return hit.collider.transform.parent.GetComponent<Zone>();
        }

        return null;
    }




    public void SetZoneInfos(Zone[] zonesToUpdate = null)
    {
        var zones = zonesToUpdate ?? Zones;

        float radius = 0;
        if (Planet.Water.Radius == 0)
        {
            var collider = Planet.Water.GetComponent<SphereCollider>();

            if (collider != null)
            {
                radius = collider.radius * Planet.Water.gameObject.transform.lossyScale.magnitude;
            }
        }
        else
        {
            radius = Planet.Water.Radius;
        }
        foreach (var zone in Zones)
        {
            zone.Resources.RemoveAll(x => x.ResourceType == ResourceType.Drink);
            zone.transform.position = zone.VerticeIds.Aggregate(new Vector3(0, 0, 0), (s, v) => s + transform.TransformPoint(_vertexManager.GetPosition(v))) / zone.VerticeIds.Count;

            zone.SetHeights(transform.position);

            if (zone.MinHeight >= (radius / 2) + .65f && zone.MaxHeight < (radius / 2) + .75f)
            {
                zone.Resources.Add(new Resource(ResourceType.Drink));
            }

            zone.Accessible = zone.Height >= radius / 2 + .13f && zone.DeltaHeight < _acceptableSlope;
            zone.GetComponent<MeshRenderer>().material.color = zone.Accessible ? Color.green : Color.red;
        }
    }

    public void Add(Zone zone)
    {
        if (_zoneDictionary.ContainsKey(zone.Id))
            return;

        _zoneDictionary.Add(zone.Id, zone);
    }

    public Zone FindById(Guid id)
    {
        if (!_zoneDictionary.ContainsKey(id))
            return null;

        return _zoneDictionary[id];
    }

    public bool TryGetById(Guid id, out Zone zone)
    {
        zone = null;

        if (!_zoneDictionary.ContainsKey(id))
            return false;

        zone = _zoneDictionary[id];
        return true;
    }

    public IList<Zone> FindAll()
    {
        return _zoneDictionary.Values.ToArray();
    }
}
