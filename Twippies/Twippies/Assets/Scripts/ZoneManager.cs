using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour {

    private Mesh _planeteMesh;
    private Vector3[] _vertices;
    [SerializeField]
    private Zone _zonePrefab;
    private int _nbVertex;
    private List<Zone> _zones;


    private void Start()
    {
        _zones = new List<Zone>();
        _planeteMesh = GetComponent<MeshFilter>().mesh;
        _vertices = _planeteMesh.vertices;
        _nbVertex = _planeteMesh.vertexCount;

        for (int i = 0; i < 1000; i++)
        {
            FindCenter(2f);
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
            foreach (Zone z in _zones)
            {
                float dist = (_vertices[i] - z.CenterZone).sqrMagnitude;
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }
            }
            tempZone.Vertices.Add(transform.TransformPoint(_vertices[i]));
        }

        /*for (int i = 0; i < _nbVertex; i++)
        { 
            Zone zone = _zones.FindClosest(transform.TransformPoint(_vertices[i]));
            zone.Vertices.Add(transform.TransformPoint(_vertices[i]));
        }*/
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
            zone.ZoneObject = MeshMaker.CreateSelection(zone.Vertices, zone.transform, zone.Center);
            zone.ZoneObject.transform.Translate((zone.gameObject.transform.position - transform.position).normalized * .2f);
            zone.ZoneObject.GetComponent<MeshRenderer>().material.color = new Color(1, (zone.MinHeight - 4) / 2, 0);//zone.Col;
        }
    }

}
