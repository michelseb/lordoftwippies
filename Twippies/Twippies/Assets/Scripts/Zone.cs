using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    [SerializeField]
    private Material _zoneMaterial;
    private List<Vector3> _vertices;
    private Vector3 _centerZone;
    private GameObject _zoneObject;
    private float _minHeight, _maxHeight, _meanHeight, _deltaHeight;
    private Color _col;
    private bool _accessible;
    private int _centerId;

    private void Awake()
    {
        _col = new Color(Random.value, Random.value, Random.value);
        _vertices = new List<Vector3>();
    }

    public void SetMinHeight(Vector3 center)
    {
        float height = float.PositiveInfinity;
        foreach (Vector3 v in _vertices)
        {
            if (Vector3.Distance(v, center) < height)
            {
                height = Vector3.Distance(v, center);
            }
        }
        _minHeight = height;
    }


    public void SetMaxHeight(Vector3 center)
    {
        float height = 0;
        foreach (Vector3 v in _vertices)
        {
            if (Vector3.Distance(v, center) > height)
            {
                height = Vector3.Distance(v, center);
            }
        }
        _maxHeight = height;
    }

    public Color Col
    {
        get
        {
            return _col;
        }
    }

    public List<Vector3> Vertices
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

    public Vector3 CenterZone
    {
        get
        {
            return _centerZone;
        }
    }

    public bool Accessible
    {
        get
        {
            return _accessible;
        }
    }

    public Vector3 Center
    {
        get
        {
            return _centerZone;
        }
        set
        {
            _centerZone = value;
        }
    }

    public float MinHeight
    {
        get
        {
            return _minHeight;
        }
    }

    public float MaxHeight
    {
        get
        {
            return _maxHeight;
        }
    }

    public int CenterId
    {
        get
        {
            return _centerId;
        }
        set
        {
            _centerId = value;
        }
    }

    public GameObject ZoneObject
    {
        get
        {
            return _zoneObject;
        }
        set
        {
            _zoneObject = value;
        }
    }

    public Material ZoneMaterial
    {
        get
        {
            return _zoneMaterial;
        }
    }

}
