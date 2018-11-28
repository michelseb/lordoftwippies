using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Vector3[] _vertices;
    [SerializeField]
    private Zone _zonePrefab;
    private int _nbVertex;
    private KdTree<Zone> _zones;


    private void Start()
    {
        _zones = new KdTree<Zone>();
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _vertices = _planeteMesh.vertices;
        _nbVertex = _planeteMesh.vertexCount;

        for (int i = 0; i < 1000; i++)
        {
            FindCenter(.5f);
        }

        SetVertices();
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
            float dist = (center - transform.TransformPoint(z.CenterZone)).magnitude;
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

    public KdTree<Zone> Zones
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

        /*for (int i = 0; i < _nbVertex; i++)
        {
            float distMin = Mathf.Infinity;
            Zone tempZone = null;
            foreach (Zone z in _zones)
            {
                float dist = (_planeteMesh.vertices[i] - z.CenterZone).sqrMagnitude;
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }
            }
            tempZone.Vertices.Add(_planeteMesh.vertices[i]);
        }*/

        foreach (Vector3 v in _vertices)
        {
            Zone zone = _zones.FindClosest(transform.TransformPoint(v));
            zone.Vertices.Add(transform.TransformPoint(v));
        }
    }
    public void SetHeights()
    {
        foreach (Zone zone in _zones)
        {
            Debug.Log(zone.Vertices.Count);
            zone.SetMaxHeight(transform.position);
            zone.SetMinHeight(transform.position);
        }
    }

}
