using UnityEngine;

public class Controls : MonoBehaviour {

    public enum ControlMode
    {
        Clicking,
        Dragging,
        Checking,
        Waiting
    }

    public enum ClicMode
    {
        RightClic,
        LeftClic,
        CentralClic,
        None
    }

    [SerializeField]
    private float _zoomSpeed;

    [SerializeField]
    private GameObject _dragCollider;

    [SerializeField]
    private GameObject _aerialDragCollider;

    [SerializeField]
    private GameObject _planeCollider;

    [SerializeField]
    private Camera _frontCam;

    private float _zoomSensitivity = 15;
    
    private Vector2 _originClic;
    private float _initZoomAmount, _zoomAmount, _maxZoom = 50;
    private ManageableObjet _focusedObject;
    private int _focusedLayer;
    private Camera _cam;
    private UIManager _ui;
    private UIResources _uiR;
    private ObjetManager _om;


    public ClicMode clic;
    public ControlMode ctrl;

    const float DIST_TO_DRAG = 10.0f;

    private static Controls _instance;
    public static Controls Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<Controls>();

            return _instance;
        }
    }



    private void Awake()
    {
        _cam = Camera.main;
        _ui = UIManager.Instance;
        _uiR = UIResources.Instance;
        _om = ObjetManager.Instance;
    }

    private void Start()
    {
        ctrl = ControlMode.Waiting;
        _initZoomAmount = _cam.fieldOfView;
        _zoomAmount = _cam.fieldOfView;
    }



    private void Update()
    {
        float wheelSpeed = Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
        _zoomAmount -= wheelSpeed;
        _zoomAmount = Mathf.Clamp(_zoomAmount, _initZoomAmount - _maxZoom, _initZoomAmount + _maxZoom);
        _cam.fieldOfView = Mathf.Lerp(_cam.fieldOfView, _zoomAmount, Time.deltaTime * _zoomSpeed);
        _frontCam.fieldOfView = _cam.fieldOfView;
        
        switch (ctrl)
        {

            case ControlMode.Waiting:

                if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
                {
                    DefineOriginClick();
                }

                break;


            case ControlMode.Clicking:
                if (_focusedObject != null)
                {
                    _focusedLayer = _focusedObject.gameObject.layer;
                }
                if (Vector2.Distance(_originClic, Input.mousePosition) > DIST_TO_DRAG)
                {
                    ctrl = ControlMode.Dragging;
                }
                if ((Input.GetMouseButtonUp(0)|| Input.GetMouseButtonUp(1)) && _focusedObject != null)
                {
                    _focusedObject.Stats.enabled = true;
                    _ui.SetPreviewCam(_focusedObject);
                    _ui.InfoGUI = true;
                    ctrl = ControlMode.Checking;
                }
                break;


            case ControlMode.Dragging:

                if (Input.GetButton("Fire1")) // Clic gauche
                {
                    if (!_dragCollider.activeSelf)
                    {
                        clic = ClicMode.LeftClic;
                        _dragCollider.SetActive(true);
                    }

                    if (_focusedObject != null)
                    {
                        if (_focusedObject is AerialObjet)
                        {
                            _aerialDragCollider.SetActive(true);
                        }
                        if (_focusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)_focusedObject;
                            if (d.Coll.enabled)
                            {
                                d.Coll.enabled = false;
                            }
                            if (Input.mousePosition.x < Screen.width * 4 / 10 ||
                                Input.mousePosition.x > Screen.width * 6 / 10 ||
                                Input.mousePosition.y < Screen.height * 3 / 8 ||
                                Input.mousePosition.y > Screen.height * 5 / 8)
                            {
                                if (d.P != null)
                                {
                                    d.P.AbsRotateObjet();
                                }
                            }
                        }
                    }

                }
                else if (Input.GetButton("Fire2") && _focusedObject is DraggableObjet) // Clic droit sur un objet draggable
                {
                    if (!_planeCollider.activeSelf)
                    {
                        clic = ClicMode.RightClic;
                        _focusedObject.gameObject.layer = 15;
                        foreach (Transform child in _focusedObject.transform)
                        {
                            child.gameObject.layer = 15;
                        }
                        _uiR.PlayMouthOpen();
                        _planeCollider.SetActive(true);
                    }
                    if (_focusedObject != null)
                    {
                        if (_focusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)_focusedObject;
                            if (d.Coll.enabled)
                            {
                                d.Coll.enabled = false;
                            }

                        }
                    }
                }
                else // Clic laché
                {
                    if (clic == ClicMode.RightClic)
                    {
                        if (_uiR.MouseOver && _focusedObject is DraggableObjet)
                        {
                            Debug.Log("Resources added: "+_focusedObject.WoodCost+" Wood, "+_focusedObject.WaterCost+" Water, "+_focusedObject.StoneCost+" Stone");
                            _uiR.AddResources(_focusedObject.WoodCost, _focusedObject.WaterCost, _focusedObject.StoneCost);
                            _om.allObjects.Remove(_focusedObject);
                            Destroy(_focusedObject.gameObject);
                        }
                    }
                    clic = ClicMode.None;
                    if (_planeCollider.activeSelf)
                    {
                        _focusedObject.gameObject.layer = _focusedLayer;
                        foreach (Transform child in _focusedObject.transform)
                        {
                            child.gameObject.layer = _focusedLayer;
                        }
                        _uiR.PlayMouthClose();
                        _planeCollider.SetActive(false);
                    }
                    _aerialDragCollider.SetActive(false);
                    _dragCollider.SetActive(false);
                    
                    if (_focusedObject)
                    {
                        if (_focusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)_focusedObject;
                            if (!d.Coll.enabled)
                            {
                                d.Coll.enabled = true;
                            }
                        }
                    }
                    _focusedObject = null;
                    ctrl = ControlMode.Waiting;
                }

                break;


            case ControlMode.Checking:
                if (_focusedObject is Planete)
                {
                    Planete planete = (Planete)_focusedObject;
                    if (planete.Shaping)
                    {
                        return;
                    }
                }
                if ((Input.GetMouseButtonDown(0)||Input.GetMouseButtonDown(1)) && Input.mousePosition.y > Screen.height * 1/3)
                {
                    _ui.DisablePreviewCam();
                    _ui.InfoGUI = false;
                    foreach (ManageableObjet m in _om.AllObjects<ManageableObjet>())
                    {
                        m.Stats.enabled = false;
                    }
                    if (!Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition)))
                    {
                        _focusedObject = null;
                        ctrl = ControlMode.Waiting;
                    }
                    else
                    {
                        DefineOriginClick();
                    }
                }
                break; 

        }
        
    }

    private void DefineOriginClick()
    {
        _originClic = Input.mousePosition;
        ctrl = ControlMode.Clicking;
        
    }


    public ManageableObjet FocusedObject
    {
        get
        {
            return _focusedObject;
        }

        set
        {
            _focusedObject = value;
        }
    }

    public Vector2 OriginClic
    {
        get
        {
            return _originClic;
        }
    }
}
