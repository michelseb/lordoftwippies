using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    [SerializeField]
    private Material _zoneMaterial;
    private List<Vector3> _vertices;
    private Vector3 _centerZone;
    private GameObject _zoneObject;
    private float _minHeight, _maxHeight, _meanHeight, _deltaHeight;
    private Color _color;
    private bool _accessible;
    private int _centerId;
    private int _nbEntries;
    private MeshCollider _collider;
    private MeshRenderer _renderer;
    private ObjetManager _om;

    private void Awake()
    {
        _color = new Color(Random.value, Random.value, Random.value);
        _vertices = new List<Vector3>();
        _om = ObjetManager.Instance;
    }

    private void Update()
    {
        if (_collider == null)
        {
            if (_zoneObject != null) {
                _collider = _zoneObject.GetComponent<MeshCollider>();
                _renderer = _zoneObject.GetComponent<MeshRenderer>();
            }
        }
        _renderer.material.color = new Color(Mathf.Lerp(_renderer.material.color.r, (_nbEntries*50)/_om.allObjects.Count, Time.deltaTime * 2), .3f, Mathf.Lerp(_renderer.material.color.b, .3f + (_nbEntries * 100) / _om.allObjects.Count, Time.deltaTime * 2), .6f);
        if (!_renderer.enabled)
        {
            if (_nbEntries > 0)
            {
                _renderer.enabled = true;
            } 
        }
        else
        {
            if (_nbEntries <= 0)
            {
               //_renderer.enabled = false;
            }
        }
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entrée");
        if (other.tag == "Twippie")
        {
            
            _nbEntries++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("sortie");
        if (other.tag == "Twippie")
        {
            
            if (_nbEntries > 0)
                _nbEntries--;
        }
    }

    public Color Col
    {
        get
        {
            return _color;
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
