using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Planete _planete;
    private Vector3[] _vertices;
    private int[] _triangles;
    [SerializeField]
    private Zone _zonePrefab;
    private int _nbVertex;
    private Zone[] _zones;


    private void Awake()
    {
        
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _planete = gameObject.GetComponent<Planete>();
        _vertices = _planeteMesh.vertices;
        _triangles = _planeteMesh.triangles;
        _nbVertex = _planeteMesh.vertexCount;

        List<Zone> tempZones = new List<Zone>();
        for (int i = 0; i < 1000; i++)
        {
            FindCenter(.7f, tempZones);
        }
        _zones = tempZones.ToArray();
        SetTriangles();
        SetHeights();
        FindNeighbours();
    }

    /*private void OnDrawGizmos()
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
    }*/

    private void FindCenter(float minDist, List<Zone> zones)
    {
        int id = Random.Range(0, _nbVertex - 1);
        Vector3 center = transform.TransformPoint(_vertices[id]);
        foreach (Zone z in zones)
        {
            float dist = (center - z.Center).magnitude;
            if (dist < minDist)
            {
                return;
            }
        }
        Zone zone = Instantiate(_zonePrefab,transform);
        zone.ZManager = this;
        zone.Center = center;
        zone.CenterId = id;
        zones.Add(zone);
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
                    if (!_zones[a].Neighbours.Contains(_zones[b]))
                    {
                        _zones[a].Neighbours.Add(_zones[b]);
                    }
                    if (!_zones[b].Neighbours.Contains(_zones[a]))
                    {
                        _zones[b].Neighbours.Add(_zones[a]);
                    }
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

    /*public void SetVertices()
    {
       foreach (Zone z in _zones)
        {
            z.transform.parent = null;
            z.Vertices.Clear();
            z.Center = transform.TransformPoint(_vertices[z.CenterId]);
            z.transform.position = z.Center;
            z.transform.parent = transform;
        }

        for (int i = 0; i < _nbVertex; i++)
        {
            float distMin = Mathf.Infinity;
            Zone tempZone = null;
            float dist = 0;
            foreach (Zone z in _zones)
            {
                dist = (_vertices[i] - z.Center).sqrMagnitude;
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }

            }
            tempZone.Vertices.Add(transform.TransformPoint(_vertices[i]));
        }
    }*/

    public void SetTriangles()
    {

        foreach (Zone z in _zones)
        {
            z.transform.parent = null;
            z.Vertices.Clear();
            z.Center = transform.TransformPoint(_vertices[z.CenterId]);
            z.transform.position = z.Center;
            z.transform.parent = transform;
        }

        for (int i = 0; i < _triangles.Length-3; i += 3)
        {
            Vector3 a = _vertices[_triangles[i]];
            Vector3 b = _vertices[_triangles[i + 1]];
            Vector3 c = _vertices[_triangles[i + 2]];

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
                tempZone.Vertices.Add(transform.TransformPoint(a));
            if (!tempZone.Vertices.Contains(b))
                tempZone.Vertices.Add(transform.TransformPoint(b));
            if (!tempZone.Vertices.Contains(c))
                tempZone.Vertices.Add(transform.TransformPoint(c));

        }


    }

    public void SetHeights()
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
        foreach (Zone zone in _zones)
        {
            if (zone.ZoneObject!= null)
            {
                Destroy(zone.ZoneObject);
            }
            zone.SetMaxHeight(transform.position);
            zone.SetMinHeight(transform.position);
            zone.MeanHeight = (zone.MaxHeight + zone.MinHeight) / 2;
            zone.DeltaHeight = zone.MaxHeight - zone.MinHeight;
            zone.ZoneObject = MeshMaker.CreateSelection(zone.Vertices, zone.transform, zone.Center, transform.position);
            zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .1f);
            if (zone.MeanHeight < (radius/2)+.7f || zone.DeltaHeight > 1) 
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
