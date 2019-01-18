using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Planete _planete;
    private Vector3[] _vertices;
    [SerializeField]
    private Zone _zonePrefab;
    private int _nbVertex;
    private Zone[] _zones;
    private bool _assigningZone;

    private void Awake()
    {
        
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _planete = gameObject.GetComponent<Planete>();
        _vertices = _planeteMesh.vertices;
        _nbVertex = _planeteMesh.vertexCount;

        List<Zone> tempZones = new List<Zone>();
        int id = 0;
        for (int i = 0; i < 1000; i++)
        {
            if (FindCenter(.7f, tempZones, id))
                id++;
        }
        Debug.Log("nombre de zones : " + id);
        _zones = tempZones.ToArray();
        SetTriangles();
        GenerateZoneObjects();
        FindNeighbours();
    }

    private void OnDrawGizmos()
    {
        if (_zones == null)
        {
            return;
        }

        if (Camera.current == Camera.main)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach (Zone z in _zones)
            {
                Gizmos.color = z.Col;
                foreach (Vector3 v in z.Vertices)
                {
                    Gizmos.DrawSphere(v, .2f);
                }
            }
        }
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
                if (_zones[a].Vertices.Any(x => _zones[b].Vertices.Any(y => x.Equals(y))))
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

    public bool AssigningZone
    {
        get
        {
            return _assigningZone;
        }
        set
        {
            _assigningZone = value;
        }
    }

    public Planete Planete
    {
        get
        {
            return _planete;
        }
    }

    public void SetTriangles()
    {
        for (int a = 0; a < _vertices.Length; a++)
        {
            _vertices[a] = transform.TransformPoint(_planeteMesh.vertices[a]);
        }

        int[] triangles = _planeteMesh.triangles;
        foreach (Zone z in _zones)
        {
            z.transform.parent = null;
            z.Vertices.Clear();
            z.Center = _vertices[z.CenterId];
            z.transform.position = z.Center;
            z.transform.parent = transform;
        }

        for (int i = 0; i < triangles.Length-3; i += 3)
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
           
            if (!tempZone.Vertices.Contains(a))
                tempZone.Vertices.Add(a);
            if (!tempZone.Vertices.Contains(b))
                tempZone.Vertices.Add(b);
            if (!tempZone.Vertices.Contains(c))
                tempZone.Vertices.Add(c);
            
        }

        GetZoneInfo();
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
            zone.ZoneObject = MeshMaker.CreateSelection(zone.Vertices, zone.transform, zone.Center, transform.position);
            zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .1f);

        }
    }

    public void GetZoneInfo()
    {
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
        for (int a = 0; a < _zones.Length; a++)
        {
            Zone zone = _zones[a];
            if (zone.Ressource.ressourceType == Ressources.RessourceType.Drink)
            {
                zone.Ressource.ressourceType = Ressources.RessourceType.None;
            }
            zone.SetMaxHeight(transform.position);
            zone.SetMinHeight(transform.position);
            zone.MeanHeight = (zone.MaxHeight + zone.MinHeight) / 2;
            zone.DeltaHeight = zone.MaxHeight - zone.MinHeight;

            if (zone.MinHeight < (radius / 2) + .7f && zone.MaxHeight > (radius / 2) + .7f)
            {
                zone.Ressource.ressourceType = Ressources.RessourceType.Drink;
            }
            if (zone.MeanHeight < (radius / 2) + .7f || zone.DeltaHeight > .5f)
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
