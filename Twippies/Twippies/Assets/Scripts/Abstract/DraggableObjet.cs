using System;
using System.Linq;
using UnityEngine;

public abstract class DraggableObjet : ManageableObjet
{

    [SerializeField] protected Planet _planet;

    private int _dragLayer;
    private Vector3 _lastPos;
    protected bool _grounded;
    protected bool _dragging;
    protected float _initHeight;
    protected Guid _zoneId;
    protected Sun _sun;
    protected PlanetManager _planetManager;

    public Planet Planet { get { return _planet; } set { _planet = value; } }
    public Zone Zone { get { return _zoneManager.FindById(_zoneId); } set { _zoneId = value.Id; } }
    public Guid ZoneId { get { return _zoneId; } set { _zoneId = value; } }

    protected override void Awake()
    {
        base.Awake();

        _rigidBody = GetComponent<Rigidbody>();
        _planetManager = PlanetManager.Instance;
        SetPlanet();
    }


    protected override void Start()
    {
        base.Start();
        transform.localScale = _initSize;
        _rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidBody.useGravity = false;
        _dragLayer = LayerMask.GetMask("Positionning");
        _currentSize = _initSize;
        _focusedSize = _initSize;

        //if (!(this is Twippie))
        //{
        transform.SetParent(_planet.transform);
        //}
        _planet.Face(transform);
        _sun = _planet.Sun;
        _initHeight = transform.position.x;
        if (_controls.CurrentContolMode != ControlMode.Dragging)
        {
            if (!(this is Twippie))
            {
                _zoneId = _zoneManager.GetZone(true, _zoneId, transform.position);
            }
            else
            {
                _zoneId = _zoneManager.GetZone(false, _zoneId, transform.position);
            }
        }
        else
        {
            _zoneId = _zoneManager.GetZone(false, _zoneId, transform.position);
        }

    }

    protected override void Update()
    {
        base.Update();

        if (_zoneId == default)
        {
            if (_controls.CurrentContolMode != ControlMode.Dragging)
            {
                if (!(this is Twippie))
                {
                    _zoneId = _zoneManager.GetZone(true, _zoneId, transform.position);
                }
                else
                {
                    _zoneId = _zoneManager.GetZone(false, _zoneId, transform.position);
                }
            }
            else
            {
                _zoneId = _zoneManager.GetZone(false, _zoneId, transform.position);
            }
        }

        if (_planet.Sun != null)
        {
            _timeReference = _planet.Sun.Speed;
        }

        _dragging = SetDraggingState();

        MakeAttraction();
    }


    private bool SetDraggingState()
    {
        if (_controls.CurrentContolMode != ControlMode.Dragging || _controls.FocusedObject != this)
            return false;

        _rigidBody.velocity = Vector3.zero;
        _rigidBody.angularVelocity = Vector3.zero;
        var pos = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(pos, out RaycastHit hit, 100, _dragLayer, QueryTriggerInteraction.Collide))
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

        return true;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        if (_grounded)
            return;

        if (!other.transform.CompareTag("Planet"))
            return;

        _grounded = true;
    }

    protected virtual void OnCollisionExit(Collision other)
    {
        if (!_grounded)
            return;

        if (!other.transform.CompareTag("Planet"))
            return;

        _grounded = false;
    }


    protected override void OnMouseUp()
    {
        _lastPos = Vector3.zero;
        base.OnMouseUp();
        _dragging = false;
    }

    private void SetPlanet()
    {
        _planet = _objectManager.GetAll<Planet>()?.OrderBy(p => (transform.position - p.transform.position).sqrMagnitude).FirstOrDefault() ?? _objectManager.MainPlanet;
    }

    protected virtual void MakeAttraction()
    {
        if (!_planet)
            return;

        if (_dragging)
        {
            _planet.Face(transform);
        }
    }
}
