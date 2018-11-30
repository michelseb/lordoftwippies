using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Vector3[] _vertices;
    private int[] _triangles;
    [SerializeField]
    private Zone _zonePrefab;
    private int _nbVertex;
    private List<Zone> _zones;


    private void Start()
    {
        _zones = new List<Zone>();
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _vertices = _planeteMesh.vertices;
        _triangles = _planeteMesh.triangles;
        _nbVertex = _planeteMesh.vertexCount;

        for (int i = 0; i < 1000; i++)
        {
            FindCenter(2f);
        }

        //SetVertices();
        SetTriangles();
        SetHeights();

        /*for (int i = 0; i < _nbVertex; i++)
        {
            float distMin = Mathf.Infinity;
            Zone tempZone = null;
            foreach (Zone z in _zones)
            {
                float dist = (_vertices[i] - z.CenterZone).sqrMagnitude;
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }
            }
            tempZone.Vertices.Add(_vertices[i]);
        }*/

        

        /*for (int a = 0; a < _nbZones; a++)
        {
            Vector3[] vertices = new Vector3[_nbVertexPerZone];
            for (int i = 0; i < _nbVertexPerZone; i++)
            {
                vertices[i] = _planeteMesh.vertices[a * _nbVertexPerZone + i];
            }
            Zone zone = new Zone(vertices);
            zone.SetMinHeight(transform.position);
            zone.SetMaxHeight(transform.position);
            _zones.Add(zone);
        }*/
    }

    private void OnDrawGizmos()
    {
        if (_zones == null)
        {
            Debug.Log("no zones");
            return;
        }

        if (Camera.current == Camera.main)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            foreach (Zone z in _zones)
            {
                //Gizmos.color = new Color(1, (z.MinHeight - 4) / 2, 0);
                Gizmos.color = z.Col;
                foreach (Vector3 v in z.Vertices)
                {
                    Gizmos.DrawSphere(v, .2f);
                }
            }
        }
    }

    private void FindCenter(float minDist)
    {
        int id = Random.Range(0, _nbVertex - 1);
        Vector3 center = transform.TransformPoint(_vertices[id]);
        foreach (Zone z in _zones)
        {
            float dist = (center - z.CenterZone).magnitude;
            if (dist < minDist)
            {
                return;
            }
        }
        Zone zone = Instantiate(_zonePrefab,transform);
        zone.Center = center;
        zone.CenterId = id;
        _zones.Add(zone);
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

    public List<Zone> Zones
    {
        get
        {
            return _zones;
        }
    }

    public void SetVertices()
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
            Zone secondZone = null;
            float dist = 0, dist2 = 0;
            foreach (Zone z in _zones)
            {
                dist = (_vertices[i] - z.CenterZone).sqrMagnitude;
                if (dist < distMin)
                {
                    if (tempZone != null)
                    {
                        if (dist2 < distMin)
                        {
                            dist2 = distMin;
                            secondZone = tempZone;
                        }
                    }
                    distMin = dist;
                    tempZone = z;
                }
                else if (dist2 > dist)
                {
                    dist2 = dist;
                    secondZone = z;
                }

            }
            tempZone.Vertices.Add(transform.TransformPoint(_vertices[i]));
            if (secondZone != null) {
                if (Mathf.Abs(distMin-dist2) < 1f)
                    secondZone.Vertices.Add(transform.TransformPoint(_vertices[i]));
            }
        }

        /*for (int i = 0; i < _nbVertex; i++)
        { 
            Zone zone = _zones.FindClosest(transform.TransformPoint(_vertices[i]));
            zone.Vertices.Add(transform.TransformPoint(_vertices[i]));
        }*/
    }

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
                float dist = (centre - z.CenterZone).sqrMagnitude;
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
        foreach (Zone zone in _zones)
        {
            if (zone.ZoneObject!= null)
            {
                Destroy(zone.ZoneObject);
            }
            zone.SetMaxHeight(transform.position);
            zone.SetMinHeight(transform.position);
            zone.ZoneObject = MeshMaker.CreateSelection(zone.Vertices, zone.transform, zone.Center, transform.position);
            zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .1f);
            zone.ZoneObject.GetComponent<MeshRenderer>().material.color = zone.Col;//new Color(1, (zone.MinHeight - 4) / 2, 0);//zone.Col;
        }
    }

}
