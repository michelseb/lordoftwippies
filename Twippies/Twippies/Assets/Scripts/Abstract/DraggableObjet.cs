using UnityEngine;

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
    
    

    protected override void Awake()
    {
        base.Awake();
        
        _r = GetComponent<Rigidbody>();
        
    }


    protected override void Start()
    {
        base.Start();
        _r.constraints = RigidbodyConstraints.FreezeRotationZ;
        _dragLayer = LayerMask.GetMask("Positionning");
        _initSize = transform.lossyScale;
        SetPlanete();
        _zManager = P.gameObject.GetComponent<ZoneManager>();
        transform.parent = _p.gameObject.transform;
        _p.Face(transform);
        _initHeight = transform.position.x;
        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        foreach (Zone z in _zManager.Zones)
        {
            float dist = (transform.position - z.Center).sqrMagnitude;
            if (dist < distMin)
            {
                distMin = dist;
                tempZone = z;
            }
        }

        _zone = tempZone;


    }

    protected virtual void LateUpdate()
    {
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

        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        foreach (Zone z in _zManager.Zones)
        {
            float dist = (transform.position - z.Center).sqrMagnitude;
            if (dist < distMin)
            {
                distMin = dist;
                tempZone = z;
            }
        }
        
        _zone = tempZone;

        foreach (Transform child in transform)
        {
            if (child.gameObject.GetComponent<MeshRenderer>() != null && _zone != null)
            {
                //child.gameObject.GetComponent<MeshRenderer>().material.color = _zone.Col;
            }
        }
        
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
        }

        MakeAttraction();
    }

    protected override void OnMouseOver()
    {
        base.OnMouseOver();
        transform.localScale = _initSize * _sizeMultiplier;
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
        foreach(Planete p in _o.AllObjects<Planete>())
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
    }

}
