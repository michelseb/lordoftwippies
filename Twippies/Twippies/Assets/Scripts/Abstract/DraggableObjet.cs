using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class DraggableObjet : ManageableObjet {

    [SerializeField]
    protected Planete _p;

    protected Rigidbody _r;

    private int _dragLayer;
    private Vector3 _lastPos;
    private bool _dragging;
    protected float _initHeight;
    protected ZoneManager _zManager;
    protected Zone _zone;
    protected Sun _planetSun;


    protected override void Awake()
    {
        base.Awake();
        
        _r = GetComponent<Rigidbody>();
        
    }


    protected override void Start()
    {
        base.Start();
        transform.localScale = _initSize;
        _r.constraints = RigidbodyConstraints.FreezeRotationZ;
        _dragLayer = LayerMask.GetMask("Positionning");
        _currentSize = _initSize;
        SetPlanete();
        _zManager = P.gameObject.GetComponent<ZoneManager>();
        transform.parent = _p.gameObject.transform;
        _p.Face(transform);
        _planetSun = _p.Sun;
        _initHeight = transform.position.x;
        if (_c.ctrl != Controls.ControlMode.Dragging)
        {
            if ((this is Twippie) == false)
            {
                _zone = GetZone(true);
            }
            else
            {
                _zone = GetZone(false);
            }
        }
        else
        {
            _zone = GetZone(false);
        }

    }

    protected virtual void LateUpdate()
    {
        if (_p.Sun != null)
        {
            _timeReference = _p.Sun.Speed;
        }

        if (_c.ctrl == Controls.ControlMode.Dragging && _c.FocusedObject == this)
        {
            _dragging = true;
            _r.velocity = Vector3.zero;
            _r.angularVelocity = Vector3.zero;
            Ray pos = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(pos, out hit, 100, _dragLayer))
            {
                _lastPos = hit.point;
                transform.position = Vector3.Lerp(transform.position, hit.point, .2f);
            }
            else
            {
                if (_lastPos != Vector3.zero)
                {
                    transform.position = _lastPos;
                }
            }
        }
        else
        {
            _dragging = false;
        }
        
        /*
        if (_zone.MinHeight < P.Water.Radius/2)
        {
            
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    child.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
                }
            }
        }
        else
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    child.gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }*/

        MakeAttraction();
    }

    

    protected override void OnMouseUp()
    {
        _lastPos = Vector3.zero;
        base.OnMouseUp();
        _dragging = false;
    }

    private void SetPlanete()
    {
        //Debug.Log("Setting planete for " +GetType());
        float dist = float.PositiveInfinity;
        foreach(Planete p in _om.AllObjects<Planete>())
        {
            if (Vector2.Distance(transform.position, p.transform.position) < dist)
            {
                _p = p;
                dist = Vector2.Distance(transform.position, p.transform.position);
            }
        }
    }

    protected virtual void MakeAttraction()
    {
        if (_p)
        {
            if (_dragging)
            {
                if (Input.GetMouseButton(0))
                    _p.Face(transform);
            }
            else
            {
                _p.Attract(transform, _r);
            }
        }
    }

    public Zone GetZoneByRessourceInList(List<Zone> list, Ressources.RessourceType ressource = Ressources.RessourceType.None, bool checkTaken = false, bool checkAccessible = false)
    {
        List<Zone> zones = new List<Zone>();
        Zone zone = null;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Ressource.ressourceType == ressource)
            {
                zones.Add(list[i]);
            }
        }

        for (int i = 0; i < zones.Count; i++)
        {
            Zone temp = zones[i];
            int randomIndex = Random.Range(i, zones.Count);
            zones[i] = zones[randomIndex];
            zones[randomIndex] = temp;
        }

        float dist = float.MaxValue;
        foreach (Zone z in zones)
        {
            float distToZone = (transform.position - z.Center).sqrMagnitude;
            if ((checkAccessible && z.Accessible) || checkAccessible == false)
            {
                if (distToZone < dist)
                {
                    if ((checkTaken && z.Taken == false) || checkTaken == false)
                    {
                        dist = distToZone;
                        zone = z;
                    }
                }
            }
        }
        return zone;
    }

    public Zone GetRandomZoneByDistance(Ressources.RessourceType ressource, bool checkTaken = false, bool checkAccessible = false, float distanceMax = float.MaxValue)
    {
        List<Zone> zones = new List<Zone>();
        for (int i = 0; i < _zManager.Zones.Length; i++)
        {
            if (_zManager.Zones[i].Ressource.ressourceType == ressource)
            {
                zones.Add(_zManager.Zones[i]);
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
            float dist = (transform.position - z.Center).sqrMagnitude;
            if ((checkAccessible && z.Accessible) || checkAccessible == false)
            {
                if (dist < distanceMax)
                {
                    if ((checkTaken && z.Taken == false) || checkTaken == false)
                    {
                        return z;
                    }
                }
            }
        }
        return null;
    }



    public virtual Zone GetZone(bool take)
    {
        if (_zone != null && take)
            _zone.Accessible = true;
        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        Zone[] zones;

        zones = _zManager.Zones;

        foreach (Zone z in zones)
        {
            float dist = (transform.position - z.Center).sqrMagnitude;
            if (z.Accessible)
            {
                if (dist < distMin)
                {
                    distMin = dist;
                    tempZone = z;
                }
            }
        }

        if (take)
            tempZone.Accessible = false;

        return tempZone;
    }

    public Planete P
    {
        get
        {
            return _p;
        }

        set
        {
            _p = value;
        }
    }

    public Zone Zone
    {
        get
        {
            return _zone;
        }
        set
        {
            _zone = value;
        }
    }

}
