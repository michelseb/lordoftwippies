using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Planete _planete;
    private Vector3[] _vertices;
    [SerializeField]
    private Zone _zonePrefab;
    [SerializeField]
    private float _zoneSize;
    private int _nbVertex;
    private Zone[] _zones;
    private Color[] _colors;

    private void Awake()
    {
        
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _planete = gameObject.GetComponent<Planete>();
        _vertices = _planeteMesh.vertices;
        _nbVertex = _planeteMesh.vertexCount;
        _colors = new Color[_nbVertex];

        List<Zone> tempZones = new List<Zone>();
        int id = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (FindCenter(_zoneSize, tempZones, id))
                id++;
        }
        Debug.Log("nombre de zones : " + id);
        _zones = tempZones.ToArray();
        InitTriangles();
        FindNeighbours();
    }

    private bool FindCenter(float minDist, List<Zone> zones, int zoneId)
    {
        int id = Random.Range(0, _nbVertex - 1);
        Vector3 center = transform.TransformPoint(_vertices[id]);
        foreach (Zone z in zones)
        {
            float dist = (center - z.Center).magnitude;
            if (dist < minDist)
            {
                return false;
            }
        }
        Zone zone = Instantiate(_zonePrefab,transform);
        zone.ZManager = this;
        zone.Center = center;
        zone.CenterId = id;
        zone.Id = zoneId;
        zones.Add(zone);
        return true;
    }

    public void FindNeighbours()
    {
        foreach (Zone z in _zones)
        {
            z.Neighbours.Clear();
        }
        for (int a = 0; a < _zones.Length; a++)
        {
            for (int b = a+1; b < _zones.Length; b++)
            {
                if (_zones[a].VerticeIds.Any(x => _zones[b].VerticeIds.Any(y => x.Equals(y))))
                {
                    _zones[a].Neighbours.Add(_zones[b]);
                    _zones[b].Neighbours.Add(_zones[a]);
                }
            }
        }
    }


    public Vector3[] Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value;
        }
    }

    public Zone[] Zones
    {
        get
        {
            return _zones;
        }
    }


    public Planete Planete
    {
        get
        {
            return _planete;
        }
    }

    public void SetTriangles(SortedDictionary<int, Vector3> deformedVertices)
    {
        List<Zone> zonesToUpdate = new List<Zone>();
        foreach (int vertexId in deformedVertices.Keys.ToList())
        {
            deformedVertices[vertexId] = transform.TransformPoint(_planeteMesh.vertices[vertexId]);

            Zone zone = _zones.FirstOrDefault(x => x.VerticeIds.Contains(vertexId));
            if (zone != null && !zonesToUpdate.Contains(zone))
            {
                foreach (int vertex in zone.VerticeIds)
                {
                    if (!deformedVertices.Keys.Contains(vertex))
                    {
                        deformedVertices.Add(vertex, transform.TransformPoint(_planeteMesh.vertices[vertex]));
                    }
                }
                zone.Center = deformedVertices[zone.CenterId];
                zonesToUpdate.Add(zone);

                if (zone.ZoneObject != null)
                {
                    Destroy(zone.ZoneObject);
                }
                zone.ZoneObject = MeshMaker.CreateSelection(this, zone, transform.position, deformedVertices);
                zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .1f);
                _planeteMesh.colors = _colors;
            }

        }
        

        GetZoneInfo(zonesToUpdate);
    }


    public void InitTriangles()
    {
        for (int a = 0; a < _vertices.Length; a++)
        {
            _vertices[a] = transform.TransformPoint(_planeteMesh.vertices[a]);
        }

        foreach (Zone z in _zones)
        {
            z.transform.parent = null;
            z.Center = _vertices[z.CenterId];
            z.transform.position = z.Center;
            z.transform.parent = transform;
        }

        int[] triangles = _planeteMesh.triangles;
        for (int i = 0; i < triangles.Length - 3; i += 3)
        {
            Vector3 a = _vertices[triangles[i]];
            Vector3 b = _vertices[triangles[i + 1]];
            Vector3 c = _vertices[triangles[i + 2]];

            Vector3 centre = (a + b + c) / 3;
            float distMin = Mathf.Infinity;
            Zone tempZone = null;
            foreach (Zone z in _zones)
            {
                float dist = (centre - z.Center).sqrMagnitude;
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;

                }
            }

            if (!tempZone.VerticeIds.Contains(triangles[i]))
            {
                _colors[triangles[i]] = tempZone.Color;
                tempZone.VerticeIds.Add(triangles[i]);
            }
            if (!tempZone.VerticeIds.Contains(triangles[i + 1]))
            {
                _colors[triangles[i + 1]] = tempZone.Color;
                tempZone.VerticeIds.Add(triangles[i + 1]);
            }
            if (!tempZone.VerticeIds.Contains(triangles[i + 2]))
            {
                _colors[triangles[i + 2]] = tempZone.Color;
                tempZone.VerticeIds.Add(triangles[i + 2]);
            }
            
        }

        GetZoneInfo();
        GenerateZoneObjects();
        _planeteMesh.colors = _colors;
        _vertices = _planeteMesh.vertices;

    }

    public void GenerateZoneObjects()
    {
        foreach (Zone zone in _zones)
        {
            if (zone.ZoneObject != null)
            {
                Destroy(zone.ZoneObject);
            }
            zone.ZoneObject = MeshMaker.CreateSelection(this, zone, transform.position);
            zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .1f);

        }
    }

    public Zone GetZoneByRessourceInList(List<Zone> list, Ressource.RessourceType ressource, PathFinder p = null, bool checkTaken = false, bool checkAccessible = false)
    {
        List<Zone> zones = new List<Zone>();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Ressources.Exists(x => x.ressourceType == ressource))
            {
                zones.Add(list[i]);
            }
        }

        if (zones.Count == 0)
            return null;

        for (int i = 0; i < zones.Count; i++)
        {
            Zone temp = zones[i];
            int randomIndex = Random.Range(i, zones.Count);
            zones[i] = zones[randomIndex];
            zones[randomIndex] = temp;
        }

        foreach (Zone z in zones)
        {
            
            if ((checkTaken && z.Taken == false) || checkTaken == false)
            {
                if (!checkAccessible)
                    return z;

                bool tempAccess = false;
                if (!z.Accessible)
                {
                    tempAccess = true;
                    z.Accessible = true;
                }
                Zone goZone = p.FindPath(z);
                if (goZone == null)
                {
                    if (tempAccess)
                        z.Accessible = false;
                    continue;
                }

                p.CreatePath(goZone);
                return z;
            }
            
        }

        return null;
    }

    public Zone GetRandomZoneByDistance(Transform t, PathFinder p, bool checkTaken = false, bool checkAccessible = false, float distanceMax = float.MaxValue)
    {
        List<Zone> zones = new List<Zone>();
        for (int i = 0; i < _zones.Length; i++)
        {
            float dist = (t.position - _zones[i].Center).sqrMagnitude;
            if (dist < distanceMax)
            {
                zones.Add(_zones[i]);
            }
        }

        for (int i = 0; i < zones.Count; i++)
        {
            Zone temp = zones[i];
            int randomIndex = Random.Range(i, zones.Count);
            zones[i] = zones[randomIndex];
            zones[randomIndex] = temp;
        }


        foreach (Zone z in zones)
        {
            if ((checkTaken && z.Taken == false) || checkTaken == false)
            {
                if (!checkAccessible)
                    return z;
                Zone goZone = p.FindPath(z);
                if (goZone == null)
                    continue;
                p.CreatePath(goZone);
                return z;
            }
            
        }
        return null;
    }

    public Zone GetLargeZoneByDistance(Transform t, PathFinder p, bool checkTaken = false, bool checkAccessible = false, float distanceMax = float.MaxValue)
    {
        List<Zone> zones = new List<Zone>();
        for (int i = 0; i < _zones.Length; i++)
        {
            float dist = (t.position - _zones[i].Center).sqrMagnitude;
            if (dist < distanceMax)
            {
                zones.Add(_zones[i]);
            }
        }

        for (int i = 0; i < zones.Count; i++)
        {
            Zone temp = zones[i];
            int randomIndex = Random.Range(i, zones.Count);
            zones[i] = zones[randomIndex];
            zones[randomIndex] = temp;
        }


        foreach (Zone z in zones)
        {
            if ((checkTaken && z.Taken == false) || checkTaken == false)
            {
                int count = 0;
                foreach (Zone neighbour in z.Neighbours)
                { 
                    if (neighbour.Taken || !neighbour.Accessible)
                        break;
                    count++;
                }
                if (count < z.Neighbours.Count - 1)
                    continue;
                if (!checkAccessible)
                    return z;
                Zone goZone = p.FindPath(z);
                if (goZone == null)
                    continue;
                foreach(Zone neighbour in z.Neighbours)
                {
                    neighbour.Taken = true;
                }
                p.CreatePath(goZone);
                return z;
            }

        }
        return null;
    }

    public Zone GetRessourceZoneByDistance(Transform t, PathFinder p, Ressource.RessourceType ressource, bool checkTaken = false, bool checkAccessible = false, float distanceMax = float.MaxValue)
    {
        List<Zone> zones = new List<Zone>();
        for (int i = 0; i < _zones.Length; i++)
        {
            float dist = (t.position - _zones[i].Center).sqrMagnitude;
            if (dist < distanceMax)
            {
                if (_zones[i].Ressources.Exists(x => x.ressourceType == ressource))
                {
                    zones.Add(_zones[i]);
                }
            }
        }

        if (zones.Count == 0)
            return null;

        for (int i = 0; i < zones.Count; i++)
        {
            Zone temp = zones[i];
            int randomIndex = Random.Range(i, zones.Count);
            zones[i] = zones[randomIndex];
            zones[randomIndex] = temp;
        }


        foreach (Zone z in zones)
        {
            if ((checkTaken && z.Taken == false) || checkTaken == false)
            {
                if (!checkAccessible)
                    return z;

                bool tempAccess = false;
                if (!z.Accessible)
                {
                    tempAccess = true;
                    z.Accessible = true;
                }
                Zone goZone = p.FindPath(z);
                if (goZone == null)
                {
                    if (tempAccess)
                        z.Accessible = false;
                    continue;
                }
                p.CreatePath(goZone);
                return z;
            }
        }
        
        return null;
    }



    public Zone GetZone(bool take, Zone current, Transform t)
    {
        if (current != null && take)
            current.Accessible = true;
        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        Zone[] zones;

        zones = _zones;

        foreach (Zone z in zones)
        {
            float dist = (t.position - z.Center).sqrMagnitude;
            if (z.Accessible)
            {
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }
            }
        }

        if (take && tempZone != null)
            tempZone.Accessible = false;
        return tempZone;
    }


    public Zone GetAerialZone(Transform t)
    {
        RaycastHit hit;
        if (Physics.Linecast(t.position, _planete.transform.position, out hit, 1 << 16, QueryTriggerInteraction.Collide))
        { 
            return hit.collider.transform.parent.GetComponent<Zone>();
        }
        return null;
    }




    public void GetZoneInfo(List<Zone> zonesToUpdate = null)
    {
        Zone[] zones;
        if (zonesToUpdate != null)
        {
            zones = zonesToUpdate.ToArray();
        }
        else
        {
            zones = _zones;
        }
        float radius = 0;
        if (_planete.Water.Radius == 0)
        {
            SphereCollider s = _planete.Water.GetComponent<SphereCollider>();

            if (s != null)
            {
                radius = s.radius * _planete.Water.gameObject.transform.lossyScale.magnitude;
            }
        }
        else
        {
            radius = _planete.Water.Radius;
        }
        for (int a = 0; a < zones.Length; a++)
        {
            Zone zone = zones[a];
            Ressource water = zone.Ressources.Find(x => x.ressourceType == Ressource.RessourceType.Drink);
            if (water != null)
            {
                zone.Ressources.Remove(water);
            }
            zone.SetMaxHeight(transform.position);
            zone.SetMinHeight(transform.position);
            zone.MeanHeight = (zone.MaxHeight + zone.MinHeight) / 2;
            zone.DeltaHeight = zone.MaxHeight - zone.MinHeight;

            if (zone.MinHeight < (radius / 2) + .7f && zone.MaxHeight > (radius / 2) + .7f)
            {
                zone.Ressources.Add(new Ressource(Ressource.RessourceType.Drink));
            }
            if (zone.MeanHeight < (radius / 2) + .7f || zone.DeltaHeight > .3f)
            {
                zone.Accessible = false;
            }
            else
            {
                zone.Accessible = true;
            }

        }
    }
}
