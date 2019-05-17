using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class DraggableObjet : ManageableObjet {

    [SerializeField]
    protected Planete _p;

    private int _dragLayer;
    private Vector3 _lastPos;
    private bool _dragging;
    protected float _initHeight;
    protected ZoneManager _zManager;
    protected Zone _zone;
    protected Sun _planetSun;

    public Planete P { get { return _p; } set { _p = value; } }
    public Zone Zone { get { return _zone; } set { _zone = value; } }
    public ZoneManager ZoneManager { get { return _zManager; } }

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
        _focusedSize = _initSize;
        SetPlanete();
        _zManager = P.gameObject.GetComponent<ZoneManager>();
        transform.parent = _p.gameObject.transform;
        _p.Face(transform);
        _planetSun = _p.Sun;
        _initHeight = transform.position.x;
        if (_controls.ctrl != Controls.ControlMode.Dragging)
        {
            if ((this is Twippie) == false)
            {
                _zone = _zManager.GetZone(true, _zone, transform);
            }
            else
            {
                _zone = _zManager.GetZone(false, _zone, transform);
            }
        }
        else
        {
            _zone = _zManager.GetZone(false, _zone, transform);
        }

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (_p.Sun != null)
        {
            _timeReference = _p.Sun.Speed;
        }

        if (_controls.ctrl == Controls.ControlMode.Dragging && _controls.FocusedObject == this)
        {
            _dragging = true;
            _r.velocity = Vector3.zero;
            _r.angularVelocity = Vector3.zero;
            Ray pos = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(pos, out hit, 100, _dragLayer, QueryTriggerInteraction.Collide))
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
            if (_dragging || IsGrounded())
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

    protected virtual void SetRigidbodyState()
    {
        if (IsGrounded() && !_r.isKinematic)
        {
            _r.isKinematic = true;
        }
        else if (!IsGrounded() && _r.isKinematic)
        {
            _r.isKinematic = false;
        }
    }


    protected bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, .5f);
    }

}
