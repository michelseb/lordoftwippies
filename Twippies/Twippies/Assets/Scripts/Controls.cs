﻿using System.Collections.Generic;
using System.Linq;
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
    private float _initZoomAmount, _zoomAmount, _maxZoom = 50;
    private Camera _cam;
    private UIManager _ui;
    private UIResources _uiR;
    private ObjetManager _om;
    private MainPanel _mainPanel;
    private Texture2D _whiteTexture;
    [SerializeField]
    private RadialPanel _radialPanel;

    public ClicMode clic;
    public ControlMode ctrl;

    const float DIST_TO_DRAG = 10.0f;

    public ManageableObjet FocusedObject { get; set; }
    public bool NewObject { get; set; }
    public int FocusedLayer { get; set; }
    public List<ManageableObjet> FocusedObjects { get; private set; }
    public Vector2 OriginClic { get; private set; }

    private static Controls _instance;
    public static Controls Instance { get { if (_instance == null) _instance = FindObjectOfType<Controls>(); return _instance; } }

    private void Awake()
    {
        _cam = Camera.main;
        _ui = UIManager.Instance;
        _uiR = UIResources.Instance;
        _om = ObjetManager.Instance;
        _radialPanel = RadialPanel.Instance;
        _mainPanel = MainPanel.Instance;
    }

    private void Start()
    {
        ctrl = ControlMode.Waiting;
        _initZoomAmount = _cam.fieldOfView;
        _zoomAmount = _cam.fieldOfView;
        FocusedObjects = new List<ManageableObjet>();
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

                if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && FocusedObject != null)
                {
                    CheckObject();
                }

                if (Vector2.Distance(OriginClic, Input.mousePosition) > DIST_TO_DRAG)
                {
                    if (FocusedObject != null && (clic != ClicMode.CentralClic))
                    {
                        FocusedLayer = FocusedObject.gameObject.layer;
                        ctrl = ControlMode.Dragging;
                    }
                    else if (clic == ClicMode.CentralClic)
                    {
                        ctrl = ControlMode.Selecting;
                        FocusedObject = null;
                    }
                }
                
                break;


            case ControlMode.Dragging:

                if (NewObject && !_planeCollider.activeSelf)
                {
                    _planeCollider.SetActive(true);
                    if ((FocusedObject is AerialObjet) == false)
                    {
                        {
                            foreach (Zone z in _om.ActivePlanet.ZManager.Zones)
                            {
                                z.Display = Zone.DisplayMode.Accessible;
                            }
                        }
                    }
                }

                if (FocusedObject is DraggableObjet) {
                    DraggableObjet d = (DraggableObjet)FocusedObject;
                    if (FocusedObject is Twippie == false)
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

                    if (FocusedObject != null)
                    {
                        if (FocusedObject is AerialObjet)
                        {
                            _aerialDragCollider.SetActive(true);
                        }
                        if (FocusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)FocusedObject;
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
                else if (Input.GetButton("Fire2") && FocusedObject is DraggableObjet) // Clic droit sur un objet draggable
                {
                    if (!_planeCollider.activeSelf)
                    {
                        FocusedObject.gameObject.layer = 15;
                        foreach (Transform child in FocusedObject.transform)
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
                    if (FocusedObject != null)
                    {
                        if (FocusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)FocusedObject;
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
                        if (_uiR.MouseOver && FocusedObject is DraggableObjet)
                        {
                            Debug.Log("Resources added: "+FocusedObject.WoodCost+" Wood, "+FocusedObject.WaterCost+" Water, "+FocusedObject.StoneCost+" Stone");
                            _uiR.AddResources(FocusedObject.WoodCost, FocusedObject.WaterCost, FocusedObject.StoneCost);
                            _om.UpdateObjectList(FocusedObject, false);
                            Destroy(FocusedObject.gameObject);
                        }
                    }
                    clic = ClicMode.None;
                    if (_planeCollider.activeSelf)
                    {
                        FocusedObject.gameObject.layer = FocusedLayer;
                        foreach (Transform child in FocusedObject.transform)
                        {
                            child.gameObject.layer = FocusedLayer;
                        }
                        _planeCollider.SetActive(false);
                    }
                    _aerialDragCollider.SetActive(false);
                    _dragCollider.SetActive(false);
                    
                    if (FocusedObject)
                    {
                        if (FocusedObject is DraggableObjet)
                        {
                            DraggableObjet d = (DraggableObjet)FocusedObject;
                            if (!d.Coll.enabled)
                            {
                                d.Coll.enabled = true;
                            }
                        }
                    }

                    if (FocusedObject != null)
                    {
                        if (FocusedObject is DraggableObjet)
                        {
                            if (FocusedObject is AerialObjet)
                            {
                                AerialObjet aerialObjet = (AerialObjet)FocusedObject;
                                aerialObjet.Zone = aerialObjet.ZoneManager.GetAerialZone(aerialObjet.transform);
                            }
                            else
                            {
                                DraggableObjet draggableObjet = (DraggableObjet)FocusedObject;
                                draggableObjet.Zone = draggableObjet.ZoneManager.GetZone(false, draggableObjet.Zone, draggableObjet.transform);
                                if (draggableObjet is IConsumable)
                                {
                                    draggableObjet.Zone.Ressources.Add(new Ressource(Ressource.RessourceType.Food, draggableObjet.GetComponent<IConsumable>(), 0));
                                }
                            }
                        }
                    }

                    if (NewObject)
                    {
                        if (FocusedObject is DraggableObjet)
                        {
                            if ((FocusedObject is AerialObjet) == false)
                            {
                                DraggableObjet draggableObjet = (DraggableObjet)FocusedObject;
                                if (!draggableObjet.Zone.Accessible)
                                {
                                    _om.UpdateObjectList(FocusedObject, false);
                                    Destroy(FocusedObject.gameObject);
                                    FocusedObject = null;
                                    NewObject = false;
                                    ctrl = ControlMode.Waiting;
                                }
                            }
                        }
                    }

                    if (FocusedObject != null)
                    {
                        if (FocusedObject is DraggableObjet && (FocusedObject is AerialObjet) == false &&(FocusedObject is Twippie) == false)
                        {
                            DraggableObjet draggableObjet = (DraggableObjet)FocusedObject;
                            draggableObjet.Zone = draggableObjet.ZoneManager.GetZone(true, draggableObjet.Zone, draggableObjet.transform);
                        }
                    }

                    FocusedObject = null;
                    NewObject = false;
                    ctrl = ControlMode.Waiting;
                }

                break;

            case ControlMode.Selecting:
                if (!Input.GetButton("Fire3"))
                {
                    clic = ClicMode.None;
                    if (FocusedObjects.Count > 0)
                    {
                        if (FocusedObjects.Count == 1)
                        {
                            FocusedObject = FocusedObjects[0];
                            FocusedObjects.Clear();
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

                if (FocusedObject is Planete)
                {
                    Planete planete = (Planete)FocusedObject;
                    if (planete.Shaping)
                    {
                        return;
                    }
                }
                if ((Input.GetButtonDown("Fire1")||Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) && VoidClic())
                {
                    _ui.DisablePreviewCam();
                    _ui.InfoGUI = false;
                    _mainPanel.SetAllStatPanelsActiveState(false);
                    _radialPanel.Animator.SetTrigger("Close");
                    if (!Physics.Raycast(_cam.ScreenPointToRay(Input.mousePosition), float.MaxValue, ~(1<<16)))
                    {
                        if (FocusedObject is Twippie)
                        {
                            Twippie t = (Twippie)FocusedObject;
                            if (t != null)
                            {
                                t.LineRenderer.enabled = false;
                            }
                        }
                        FocusedObject.SetSelectionActive(false);
                        MainPanel.Instance.SetActive(false);
                        FocusedObject = null;
                        ctrl = ControlMode.Waiting;
                    }
                    else
                    {
                        DefineOriginClick();
                    }
                }
                break;
            case ControlMode.CheckingMultiple:
                if ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) && VoidClic())
                {
                    foreach (ManageableObjet obj in FocusedObjects)
                    {
                        if (obj is Twippie)
                        {
                            Twippie t = (Twippie)obj;
                            t.LineRenderer.enabled = false;
                        }
                        obj.SetSelectionActive(false);
                    }
                    FocusedObjects.Clear();
                    _ui.InfoGUI = false;
                    _mainPanel.SetAllStatPanelsActiveState(false);
                    _radialPanel.Animator.SetTrigger("Close");
                    MainPanel.Instance.SetActive(false);
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
        OriginClic = Input.mousePosition;
        switch (clic)
        {
            case ClicMode.LeftClic:
            case ClicMode.RightClic:
                if (FocusedObject != null)
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

    private bool VoidClic()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return !Physics.Raycast(ray);
    }

    private void OnGUI()
    {
        if (ctrl == ControlMode.Selecting)
        {
            Vector3 currPos = Input.mousePosition;
            Vector2 invertedCurrent = new Vector2(currPos.x, Screen.height - currPos.y);
            Vector2 invertedOrigin = new Vector2(OriginClic.x, Screen.height - OriginClic.y);
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
            if (!FocusedObjects.Exists(x=>x is Twippie))
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
        if (FocusedObject is Twippie)
        {
            Twippie t = (Twippie)FocusedObject;
            t.LineRenderer.enabled = true;
        }
        _radialPanel.Animator.SetTrigger("Open");
        _mainPanel.SetStatPanelActiveState(true, FocusedObject.Type);
        StatPanel activePanel = _mainPanel.StatPanels.Find(x => x.Active);
        FocusedObject.GetStatManager();
        FocusedObject.PopulateStats();
        activePanel.Tab.SetFocus(true);
        activePanel.StatManager.GetStat("Amount").SetActive(false);
        MainPanel.Instance.SetActive(true);
        _ui.SetPreviewCam(FocusedObject);
        _ui.InfoGUI = true;
        ctrl = ControlMode.Checking;
    }

    private void CheckObjects()
    {
        foreach (ManageableObjet obj in FocusedObjects)
        {
            if (obj is Twippie)
            {
                Twippie t = (Twippie)obj;
                if (t.LineRenderer != null)
                {
                    t.LineRenderer.enabled = true;
                }
            }
            if (!_mainPanel.SetStatPanelActiveState(true, obj.Type)) { Debug.Log("global stat not found"); } else { Debug.Log("global stat "+obj.GetType().ToString()+ " a été updaté !"); }
        }
        List<StatPanel> activePanels = _mainPanel.StatPanels.FindAll(x => x.Active);
        MainPanel.Instance.SetActive(true);
        activePanels[0].Tab.SetFocus(true);
        _ui.InfoGUI = true;
        ctrl = ControlMode.CheckingMultiple;
    }


}
