using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour {

    public enum ControlMode
    {
        Clicking,
        Dragging,
        Checking,
        CheckingMultiple,
        Selecting,
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
    private List<ManageableObjet> _focusedObjects;
    private int _focusedLayer;
    private Camera _cam;
    private UIManager _ui;
    private UIResources _uiR;
    private ObjetManager _om;
    private ObjectGenerator _og;
    private MainPanel _mainPanel;
    private bool _newObject;
    private Texture2D _whiteTexture;

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
        _og = ObjectGenerator.Instance;
        _mainPanel = MainPanel.Instance;
    }

    private void Start()
    {
        ctrl = ControlMode.Waiting;
        _initZoomAmount = _cam.fieldOfView;
        _zoomAmount = _cam.fieldOfView;
        _focusedObjects = new List<ManageableObjet>();
        _whiteTexture = new Texture2D(1, 1);
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
                if (Input.GetButton("Fire1"))
                {
                    clic = ClicMode.LeftClic;
                    DefineOriginClick();
                }
                else if (Input.GetButton("Fire2"))
                {
                    clic = ClicMode.RightClic;
                    DefineOriginClick();
                }
                else if (Input.GetButton("Fire3"))
                {
                    clic = ClicMode.CentralClic;
                    DefineOriginClick();
                }
                break;


            case ControlMode.Clicking:

                if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && _focusedObject != null)
                {
                    CheckObject();
                }

                if (Vector2.Distance(_originClic, Input.mousePosition) > DIST_TO_DRAG)
                {
                    if (_focusedObject != null && (clic != ClicMode.CentralClic))
                    {
                        _focusedLayer = _focusedObject.gameObject.layer;
                        ctrl = ControlMode.Dragging;
                    }
                    else if (clic == ClicMode.CentralClic)
                    {
                        ctrl = ControlMode.Selecting;
                        _focusedObject = null;
                    }
                }
                
                break;


            case ControlMode.Dragging:

                if (_newObject && !_planeCollider.activeSelf)
                {
                    _planeCollider.SetActive(true);
                    if ((_focusedObject is AerialObjet) == false)
                    {
                        {
                            foreach (Zone z in _om.ActivePlanet.ZManager.Zones)
                            {
                                z.Display = Zone.DisplayMode.Accessible;
                            }
                        }
                    }
                }

                if (_focusedObject is DraggableObjet) {
                    DraggableObjet d = (DraggableObjet)_focusedObject;
                    if (_focusedObject is Twippie == false)
                    {
                        if (d.Zone != null)
                        {
                            d.Zone.Accessible = true;
                        }
                    }
                }

                if (Input.GetButton("Fire1")) // Clic gauche
                {
                    if (!_dragCollider.activeSelf)
                    {
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
                        _focusedObject.gameObject.layer = 15;
                        foreach (Transform child in _focusedObject.transform)
                        {
                            child.gameObject.layer = 15;
                        }
                        if (!_uiR.Selected)
                        {
                            _uiR.PlayMouthOpen();
                            _uiR.Selected = true;
                        }
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
                            _om.UpdateObjectList(_focusedObject, false);
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

                    if (_focusedObject != null)
                    {
                        if (_focusedObject is DraggableObjet)
                        {
                            if (_focusedObject is AerialObjet)
                            {
                                AerialObjet aerialObjet = (AerialObjet)_focusedObject;
                                aerialObjet.Zone = aerialObjet.ZoneManager.GetAerialZone(aerialObjet.transform);
                            }
                            else
                            {
                                DraggableObjet draggableObjet = (DraggableObjet)_focusedObject;
                                draggableObjet.Zone = draggableObjet.ZoneManager.GetZone(false, draggableObjet.Zone, draggableObjet.transform);
                                if (draggableObjet is IConsumable)
                                {
                                    IConsumable consumable = (IConsumable)draggableObjet;
                                    draggableObjet.Zone.Ressources.Add(new Ressource(Ressource.RessourceType.Food, draggableObjet.GetComponent<IConsumable>(), 0));
                                }
                            }
                        }
                    }

                    if (_newObject)
                    {
                        if (_focusedObject is DraggableObjet)
                        {
                            if ((_focusedObject is AerialObjet) == false)
                            {
                                DraggableObjet draggableObjet = (DraggableObjet)_focusedObject;
                                if (!draggableObjet.Zone.Accessible)
                                {
                                    _om.UpdateObjectList(_focusedObject, false);
                                    Destroy(_focusedObject.gameObject);
                                    _focusedObject = null;
                                    _newObject = false;
                                    ctrl = ControlMode.Waiting;
                                }
                            }
                        }
                    }

                    if (_focusedObject != null)
                    {
                        if (_focusedObject is DraggableObjet && (_focusedObject is AerialObjet) == false &&(_focusedObject is Twippie) == false)
                        {
                            DraggableObjet draggableObjet = (DraggableObjet)_focusedObject;
                            draggableObjet.Zone = draggableObjet.ZoneManager.GetZone(true, draggableObjet.Zone, draggableObjet.transform);
                        }
                    }

                    _focusedObject = null;
                    _newObject = false;
                    ctrl = ControlMode.Waiting;
                }

                break;

            case ControlMode.Selecting:
                if (!Input.GetButton("Fire3"))
                {
                    clic = ClicMode.None;
                    if (_focusedObjects.Count > 0)
                    {
                        if (_focusedObjects.Count == 1)
                        {
                            _focusedObject = _focusedObjects[0];
                            _focusedObjects.Clear();
                            CheckObject();
                        }
                        else
                        {
                            CheckObjects();
                        }
                    }
                    else
                    {
                        ctrl = ControlMode.Waiting;
                    }
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
                if ((Input.GetButtonDown("Fire1")||Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) && Input.mousePosition.x < Screen.width * 2/3)
                {
                    _ui.DisablePreviewCam();
                    _ui.InfoGUI = false;
                    _mainPanel.SetAllStatPanelsActiveState(false);
                    if (!Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), float.MaxValue, ~(1<<16)))
                    {
                        if (_focusedObject is Twippie)
                        {
                            Twippie t = (Twippie)_focusedObject;
                            if (t != null)
                            {
                                t.LineRenderer.enabled = false;
                            }
                        }
                        _focusedObject.SetSelectionActive(false);
                        _focusedObject = null;
                        ctrl = ControlMode.Waiting;
                    }
                    else
                    {
                        DefineOriginClick();
                    }
                }
                break;
            case ControlMode.CheckingMultiple:
                if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) && Input.mousePosition.x < Screen.width * 2 / 3)
                {
                    foreach (ManageableObjet obj in _focusedObjects)
                    {
                        if (obj is Twippie)
                        {
                            Twippie t = (Twippie)obj;
                            t.LineRenderer.enabled = false;
                        }
                        obj.SetSelectionActive(false);
                    }
                    _mainPanel.SetAllStatPanelsActiveState(false);
                    _focusedObjects.Clear();
                    _ui.InfoGUI = false;
                    _mainPanel.SetAllStatPanelsActiveState(false);
                    if (!Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), float.MaxValue, ~(1 << 16)))
                    {
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
        switch (clic)
        {
            case ClicMode.LeftClic:
            case ClicMode.RightClic:
                if (_focusedObject != null)
                {
                    ctrl = ControlMode.Clicking;
                }
                else
                {
                    ctrl = ControlMode.Waiting;
                }
                break;
            case ClicMode.CentralClic:
                ctrl = ControlMode.Clicking;
                break;
            default:
                ctrl = ControlMode.Waiting;
                break;
        }
        
        
    }

    private void OnGUI()
    {
        if (ctrl == ControlMode.Selecting)
        {
            Vector3 currPos = Input.mousePosition;
            Vector2 invertedCurrent = new Vector2(currPos.x, Screen.height - currPos.y);
            Vector2 invertedOrigin = new Vector2(_originClic.x, Screen.height - _originClic.y);
            Vector2 mixX = new Vector2(invertedCurrent.x, invertedOrigin.y);
            Vector2 mixY = new Vector2(invertedOrigin.x, invertedCurrent.y);
            Rect rect = new Rect();
            if (invertedCurrent.x > invertedOrigin.x && invertedCurrent.y > invertedOrigin.y)
            {
                rect = new Rect(invertedOrigin, invertedCurrent - invertedOrigin);
            }
            else if (invertedCurrent.x > invertedOrigin.x)
            {
                rect = new Rect(mixY, mixX - mixY);
            }
            else if (invertedCurrent.y > invertedOrigin.y)
            {
                rect = new Rect(mixX, mixY - mixX);
            }
            else
            {
                rect = new Rect(invertedCurrent, invertedOrigin - invertedCurrent);
            }
            if (!_focusedObjects.Exists(x=>x is Twippie))
            {
                DrawScreenRect(rect, new Color(1, 1, 1, .4f));
                DrawScreenRectBorder(rect, 2f, Color.white);
            }
            else
            {
                DrawScreenRect(rect, new Color(1, .7f, .5f, .4f));
                DrawScreenRectBorder(rect, 2f, new Color(1, .7f, .5f));
            }
        }
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, _whiteTexture);
        GUI.color = Color.white;
    }

    private void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    private void CheckObject()
    {
        if (_focusedObject is Twippie)
        {
            Twippie t = (Twippie)_focusedObject;
            t.LineRenderer.enabled = true;
        }
        _focusedObject.Stats = _focusedObject.GetStatManager();
        _mainPanel.SetStatPanelActiveState(true, _focusedObject.Type);
        _ui.SetPreviewCam(_focusedObject);
        _ui.InfoGUI = true;
        ctrl = ControlMode.Checking;
    }

    private void CheckObjects()
    {
        foreach (ManageableObjet obj in _focusedObjects)
        {
            if (obj is Twippie)
            {
                Twippie t = (Twippie)obj;
                t.LineRenderer.enabled = true;
            }
            if (!_mainPanel.SetStatPanelActiveState(true, obj.Type)) { Debug.Log("global stat not found"); } else { Debug.Log("global stat "+obj.GetType().ToString()+ " a été updaté !"); }
            
        }
        _mainPanel.StatPanels[0].Tab.SetFocus(true);
        _ui.InfoGUI = true;
        ctrl = ControlMode.CheckingMultiple;
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

    public List<ManageableObjet> FocusedObjects
    {
        get
        {
            return _focusedObjects;
        }
    }

    public Vector2 OriginClic
    {
        get
        {
            return _originClic;
        }
    }

    public bool NewObject
    {
        get
        {
            return _newObject;
        }
        set
        {
            _newObject = value;
        }
    }

    public int FocusedLayer
    {
        get
        {
            return _focusedLayer;
        }
        set
        {
            _focusedLayer = value;
        }
    }
}
