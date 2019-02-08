﻿using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    [SerializeField]
    private int _id;
    private Material _zoneMaterial;
    private float _minHeight, _maxHeight, _meanHeight, _deltaHeight;
    private Color _color;
    private int _centerId;
    private List<Twippie> _twippies;
    private MeshCollider _collider;
    private MeshRenderer _renderer;
    private ObjetManager _om;
    private ZoneManager _zManager;
    private Vector3 _centerZone;
    private List<Vector3> _vertices;
    private GameObject _zoneObject;
    private List<Zone> _neighbours;
    private List<PathCost> _pathCosts;
    private bool _accessible;
    private bool _taken;
    private List<Ressource> _ressources;


    public enum DisplayMode
    {
        Population,
        Height,
        Needs,
        Groups,
        Accessible,
        Water,
        Food,
        None
    }

    private DisplayMode _displayMode; 

    private void Awake()
    {
        _vertices = new List<Vector3>();
        _neighbours = new List<Zone>();
        _om = ObjetManager.Instance;
        _pathCosts = new List<PathCost>();
        _ressources = new List<Ressource>();
        _twippies = new List<Twippie>();
    }

    private void Update()
    {

        foreach (Zone z in Neighbours)
        {
            Debug.DrawLine(_centerZone, z._centerZone);
        }

        _centerZone = _zManager.transform.TransformPoint(_zManager.Vertices[_centerId]);
        if (_collider == null)
        {
            if (_zoneObject != null) {
                _collider = _zoneObject.GetComponent<MeshCollider>();
                _renderer = _zoneObject.GetComponent<MeshRenderer>();
            }
        }

        if (_displayMode == DisplayMode.None)
        {
            if (_renderer.enabled)
            {
                _renderer.enabled = false;
            }
        }
        else
        {
            if (!_renderer.enabled)
            {
                _renderer.enabled = true;
            }
            switch (_displayMode)
            {
                case DisplayMode.Population:

                    _renderer.material.color = new Color(Mathf.Lerp(_renderer.material.color.r, (_twippies.Count * 50) / _om.allObjects.Count, Time.deltaTime * 2), 
                                                        .3f, 
                                                        Mathf.Lerp(_renderer.material.color.b, .3f + (_twippies.Count * 100) / _om.allObjects.Count, Time.deltaTime * 2), 
                                                        .4f);

                    break;


                case DisplayMode.Height:

                    _renderer.material.color = new Color(1, ((_minHeight+_maxHeight)/2 - 4) / 2, 0, .4f);
                    break;


                case DisplayMode.Needs:
                    break;


                case DisplayMode.Groups:
                    break;

                case DisplayMode.Accessible:

                    if (_accessible)
                    {
                        _renderer.material.color = new Color(0, 1, 1, .4f);
                    }else
                    {
                        _renderer.material.color = new Color(1, 0, 0, .4f);
                    }

                    break;

                case DisplayMode.Water:
                    if (_ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Drink))
                    {
                        _renderer.material.color = new Color(0, .8f, 1, .6f);
                    }
                    else
                    {
                        _renderer.material.color = new Color(0, 0, 0, .3f);
                    }
                    break;

                case DisplayMode.Food:
                    if (_ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Food))
                    {
                        _renderer.material.color = new Color(0, 1, 0, .6f);
                    }
                    else
                    {
                        _renderer.material.color = new Color(0, 0, 0, .3f);
                    }
                    break;
               
            }
        }
    }

    public bool CheckRessourceInNeighbours(Ressource.RessourceType ressourceType)
    {
        if (_ressources.Exists(x => x.ressourceType == ressourceType))
            return true;

        foreach (Zone neighbour in Neighbours)
        {
            if (neighbour._ressources.Exists(x => x.ressourceType == ressourceType))
                return true;
        }

        return false;
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
        if (other.tag == "Twippie")
        {
            Twippie twippie = other.gameObject.GetComponent<Twippie>();
            if (!_twippies.Contains(twippie))
            {
                _twippies.Add(twippie);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Twippie")
        {
            Twippie twippie = other.gameObject.GetComponent<Twippie>();
            if (_twippies.Contains(twippie))
            {
                _twippies.Remove(twippie);
            }
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

    public bool Accessible
    {
        get
        {
            return _accessible;
        }
        set
        {
            _accessible = value; 
        }
    }

    public bool Taken
    {
        get
        {
            return _taken;
        }
        set
        {
            _taken = value;
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

    public float MeanHeight
    {
        get
        {
            return _meanHeight;
        }
        set
        {
            _meanHeight = value;
        }
    }

    public float DeltaHeight
    {
        get
        {
            return _deltaHeight;
        }
        set
        {
            _deltaHeight = value;
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

    public DisplayMode Display
    {
        get
        {
            return _displayMode;
        }
        set
        {
            _displayMode = value;
        }
    }

    public ZoneManager ZManager
    {
        get
        {
            return _zManager;
        }
        set
        {
            _zManager = value;
        }
    }

    public List<Zone> Neighbours
    {
        get
        {
            return _neighbours;
        }
        set
        {
            _neighbours = value;
        }
    }

    public List<PathCost> PathCosts
    {
        get
        {
            return _pathCosts;
        }
        set
        {
            _pathCosts = value;
        }
    }

    public int Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }
    public List<Ressource> Ressources
    {
        get
        {
            return _ressources;
        }
        set
        {
            _ressources = value;
        }
    }

    public List<Twippie> Twippies
    {
        get
        {
            return _twippies;
        }
    }


}
