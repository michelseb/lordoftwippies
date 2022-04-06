using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

public enum ZoomMode
{
    Initial,
    Intermediate,
    Focused
}


public class Controls : MonoBehaviour
{
    [SerializeField] private float _zoomSpeed;
    [SerializeField] private GameObject _dragCollider;
    [SerializeField] private GameObject _aerialDragCollider;
    [SerializeField] private GameObject _planeCollider;
    [SerializeField] private Camera _frontCam;
    [SerializeField] private Planet _planet;

    private ObjetManager _objectManager;
    private ZoneManager _zoneManager;
    private ObjectGenerator _objectGenerator;

    private float _wheelSpeed;
    private float _zoomSensitivity = 15;
    private float _initZoomAmount, _zoomAmount;
    private ZoomMode _autoZoom;
    private Camera _camera;
    private UIManager _uiManager;
    private UIResources _uiResources;
    private MainPanel _mainPanel;
    private Texture2D _whiteTexture;
    private RadialMenu _radialPanel;
    private Coroutine _zoomTask;

    public ClicMode CurrentClicMode { get; private set; }
    public ControlMode CurrentContolMode { get; private set; }

    const float DIST_TO_DRAG = 10f;
    const float MAX_ZOOM = 80f;

    public ManageableObjet FocusedObject { get; set; }
    public GraphicElement FocusedUI { get; set; }
    public bool NewObject { get; set; }
    public int FocusedLayer { get; set; }
    public List<ManageableObjet> FocusedObjects { get; private set; }
    public Vector2 OriginClic { get; private set; }
    private static Controls _instance;
    public static Controls Instance { get { if (_instance == null) _instance = FindObjectOfType<Controls>(); return _instance; } }

    private void Awake()
    {
        _camera = Camera.main;
        _uiManager = UIManager.Instance;
        _uiResources = UIResources.Instance;
        _objectManager = ObjetManager.Instance;
        _zoneManager = ZoneManager.Instance;
        _objectGenerator = ObjectGenerator.Instance;
        _radialPanel = _planet.MainRadial;
        _mainPanel = MainPanel.Instance;
    }

    private void Start()
    {
        CurrentContolMode = ControlMode.Waiting;
        CurrentClicMode = ClicMode.None;
        _initZoomAmount = _camera.fieldOfView;
        _zoomAmount = _camera.fieldOfView;
        FocusedObjects = new List<ManageableObjet>();
        _whiteTexture = new Texture2D(1, 1);
    }



    private void Update()
    {
        //if (FocusedUI == null)
        //{
        //    if (_uiManager.InfoGUI)
        //    {
        //        //if (_autoZoom != ZoomMode.Intermediate)
        //        //{
        //        //    _autoZoom = ZoomMode.Intermediate;
        //        //    StartCoroutine(AutoZoom(10));
        //        //}
        //    }
        //    else
        //    {
        //        //if (_autoZoom != ZoomMode.Initial)
        //        //{
        //        //    StartCoroutine(AutoZoom(-1, true));
        //        //}
        //        //else
        //        //{
        //            //_wheelSpeed = Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
        //        //}
        //    }
        //}
        //else
        //{
        //    //if (_autoZoom != ZoomMode.Focused)
        //    //{
        //    //    StartCoroutine(AutoZoom(20));
        //    //    _autoZoom = ZoomMode.Focused;
        //    //}
        //}

        _wheelSpeed = Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;

        if (_wheelSpeed != 0)
        {
            if (_zoomTask != null) StopCoroutine(_zoomTask);
            _zoomTask = StartCoroutine(ZoomTo(_camera.fieldOfView - _wheelSpeed * _zoomSpeed * _camera.fieldOfView, .2f));
        }


        if (AnyClic() && MousePosVoid())
        {
            _uiManager.DisablePreviewCam();
            _uiManager.InfoGUI = false;
            FocusedUI = null;
            StartCoroutine(_radialPanel.SetAllButtonsActiveStateWithDelay(false, 1));
            _mainPanel.SetAllStatPanelsActiveState(false);
            _radialPanel.Close();

            foreach (var obj in FocusedObjects)
            {
                if (obj is Twippie twippie)
                {
                    twippie.LineRenderer.enabled = false;
                }

                obj.SetSelectionActive(false);
            }
            FocusedObjects.Clear();

            if (FocusedObject != null)
            {
                if (FocusedObject is Twippie twippie)
                {
                    twippie.LineRenderer.enabled = false;
                }

                FocusedObject.SetSelectionActive(false);
                FocusedObject = null;
            }

            MainPanel.Instance.SetActive(false);
            CurrentContolMode = ControlMode.Waiting;
        }
        else if (AnyClic())
        {
            DefineOriginClick();
        }

        switch (CurrentContolMode)
        {

            case ControlMode.Waiting:
                if (Input.GetButton("Fire1"))
                {
                    CurrentClicMode = ClicMode.LeftClic;
                    DefineOriginClick();
                }
                else if (Input.GetButton("Fire2"))
                {
                    CurrentClicMode = ClicMode.RightClic;
                    DefineOriginClick();
                }
                else if (Input.GetButton("Fire3"))
                {
                    CurrentClicMode = ClicMode.CentralClic;
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
                    if (FocusedObject != null && (CurrentClicMode != ClicMode.CentralClic) && AnyClicHold())
                    {
                        FocusedLayer = FocusedObject.gameObject.layer;
                        CurrentContolMode = ControlMode.Dragging;
                    }
                    else if (CurrentClicMode == ClicMode.CentralClic)
                    {
                        CurrentContolMode = ControlMode.Selecting;
                        FocusedObject = null;
                    }
                }

                break;


            case ControlMode.Dragging:

                if (NewObject && !_planeCollider.activeSelf)
                {
                    _planeCollider.SetActive(true);

                    if (!(FocusedObject is AerialObjet))
                    {
                        foreach (var zone in _zoneManager.Zones)
                        {
                            zone.SetDisplayMode(DisplayMode.Accessible);
                        }
                    }
                }

                if (FocusedObject is DraggableObjet draggable)
                {
                    if (!(FocusedObject is Twippie))
                    {
                        if (draggable.Zone != null)
                        {
                            draggable.Zone.Accessible = true;
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
                        if (FocusedObject is DraggableObjet d)
                        {
                            if (d.Coll.enabled)
                            {
                                d.Coll.enabled = false;
                            }
                            if (Input.mousePosition.x < Screen.width * 4 / 10 ||
                                Input.mousePosition.x > Screen.width * 6 / 10 ||
                                Input.mousePosition.y < Screen.height * 3 / 8 ||
                                Input.mousePosition.y > Screen.height * 5 / 8)
                            {
                                if (d.Planet != null)
                                {
                                    d.Planet.AbsRotateObjet();
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
                        if (!_uiResources.Selected)
                        {
                            _uiResources.PlayMouthOpen();
                            _uiResources.Selected = true;
                        }
                        _planeCollider.SetActive(true);
                    }
                    if (FocusedObject != null)
                    {
                        if (FocusedObject is DraggableObjet d2)
                        {
                            if (d2.Coll.enabled)
                            {
                                d2.Coll.enabled = false;
                            }

                        }
                    }
                }
                else // Clic laché
                {
                    if (CurrentClicMode == ClicMode.RightClic)
                    {
                        if (_uiResources != null && _uiResources.MouseOver && FocusedObject is DraggableObjet)
                        {
                            Debug.Log("Resources added: " + FocusedObject.WoodCost + " Wood, " + FocusedObject.WaterCost + " Water, " + FocusedObject.StoneCost + " Stone");
                            _uiResources.AddResources(FocusedObject.WoodCost, FocusedObject.WaterCost, FocusedObject.StoneCost);
                            _objectManager.RemoveObject(FocusedObject);
                            Destroy(FocusedObject.gameObject);
                        }
                    }
                    CurrentClicMode = ClicMode.None;
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
                                aerialObjet.Zone = _zoneManager.GetAerialZone(aerialObjet.transform);
                            }
                            else
                            {
                                DraggableObjet draggableObjet = (DraggableObjet)FocusedObject;
                                draggableObjet.ZoneId = _zoneManager.GetZone(false, draggableObjet.Zone.Id, draggableObjet.transform.position);
                                if (draggableObjet is IConsumable)
                                {
                                    draggableObjet.Zone.Resources.Add(new Resource(ResourceType.Food, draggableObjet.GetComponent<IConsumable>(), 0));
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
                                    _objectManager.RemoveObject(FocusedObject);
                                    Destroy(FocusedObject.gameObject);
                                    FocusedObject = null;
                                    NewObject = false;
                                    CurrentContolMode = ControlMode.Waiting;
                                }
                            }
                        }
                    }

                    if (FocusedObject != null)
                    {
                        if (!(FocusedObject is AerialObjet) && !(FocusedObject is Twippie) && FocusedObject is DraggableObjet d3)
                        {
                            d3.ZoneId = _zoneManager.GetZone(true, d3.Zone.Id, d3.transform.position);
                        }
                    }

                    FocusedObject = null;
                    NewObject = false;
                    CurrentContolMode = ControlMode.Waiting;
                }

                break;

            case ControlMode.Selecting:
                if (!Input.GetButton("Fire3"))
                {
                    CurrentClicMode = ClicMode.None;
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
                        CurrentContolMode = ControlMode.Waiting;
                    }
                }
                break;

            case ControlMode.Checking:

                if (FocusedObject is Planet planet)
                {
                    if (planet.Shaping)
                        return;
                }
                break;
            case ControlMode.CheckingMultiple:
                break;
        }

    }

    private void DefineOriginClick()
    {
        OriginClic = Input.mousePosition;
        switch (CurrentClicMode)
        {
            case ClicMode.LeftClic:
            case ClicMode.RightClic:
                if (FocusedObject != null)
                {
                    CurrentContolMode = ControlMode.Clicking;
                }
                else
                {
                    CurrentContolMode = ControlMode.Waiting;
                }
                break;
            case ClicMode.CentralClic:
                CurrentContolMode = ControlMode.Clicking;
                break;
            default:
                CurrentContolMode = ControlMode.Waiting;
                break;
        }


    }

    private bool MousePosVoid()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return false;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return !Physics.Raycast(ray);
    }

    private bool AnyClic()
    {
        return Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3");
    }

    private bool AnyClicHold()
    {
        return Input.GetButton("Fire1") || Input.GetButton("Fire2") || Input.GetButton("Fire3");
    }

    private void OnGUI()
    {
        if (CurrentContolMode == ControlMode.Selecting)
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
            if (!FocusedObjects.Exists(x => x is Twippie))
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
        _radialPanel.Open();
        _radialPanel.SetAllButtonsActiveState(false, FocusedObject.Type);
        _radialPanel.SetButtonsActiveState(true, FocusedObject.Type, true);
        //FocusedObject.GetStatManager();
        //FocusedObject.PopulateStats();
        _uiManager.SetPreviewCam(FocusedObject);
        _uiManager.InfoGUI = true;
        CurrentContolMode = ControlMode.Checking;
    }

    private void CheckObjects()
    {
        foreach (ManageableObjet obj in _objectGenerator.ObjectFactory)
        {
            if (!FocusedObjects.Contains(obj))
            {
                _radialPanel.SetButtonsActiveState(false, obj.Type);
            }
        }
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
            _radialPanel.SetButtonsActiveState(true, obj.Type, true);
            if (!_mainPanel.SetStatPanelActiveState(true, obj.Type)) { Debug.Log("global stat not found"); } else { Debug.Log("global stat " + obj.GetType().ToString() + " a été updaté !"); }
        }
        _uiManager.InfoGUI = true;
        CurrentContolMode = ControlMode.CheckingMultiple;
    }


    public IEnumerator AutoZoom(float amout, bool giveControl = false, float speed = 1)
    {
        float bigger = _initZoomAmount + amout + speed;
        float smaller = _initZoomAmount + amout - speed;
        _wheelSpeed = -speed * Mathf.Sign(amout);
        while (!((_zoomAmount < bigger) && (_zoomAmount > smaller)))
        {
            yield return null;
        }
        _wheelSpeed = 0;
        if (giveControl)
        {
            _autoZoom = ZoomMode.Initial;
        }
    }

    public void SetControlMode(ControlMode mode)
    {
        CurrentContolMode = mode;
    }

    private IEnumerator ZoomTo(float zoom, float duration)
    {
        float elapsed = 0;
        var initZoom = _camera.fieldOfView;
        zoom = Mathf.Clamp(zoom, _initZoomAmount - MAX_ZOOM, _initZoomAmount + MAX_ZOOM);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _camera.fieldOfView = Mathf.Lerp(initZoom, zoom, elapsed / duration);
            _frontCam.fieldOfView = _camera.fieldOfView;

            yield return null;
        }

        _camera.fieldOfView = zoom;
    }
}