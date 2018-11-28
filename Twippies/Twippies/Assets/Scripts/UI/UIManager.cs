using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Camera _previewCam, _UICam;
    [SerializeField]
    private GameObject _eye;
    [SerializeField]
    private Texture _infoGUITex;
    [SerializeField]
    private Texture _infoGUITexFont;
    [SerializeField]
    private GUIStyle _guiStyleInfo;

    private ManageableObjet _managedObject;
    private Renderer _managedRenderer;

    private Camera _cam;
    private bool _infoGUI;
    public bool InfoGUI
    {
        get { return _infoGUI; }
        set { _infoGUI = value; }
    }

    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        _cam = Camera.main;
        _previewCam.enabled = false;
    }

    private void Update()
    {
        if (_previewCam.enabled)
        {
            if (_managedRenderer != null)
            {
                _previewCam.transform.position = _managedObject.transform.position - Vector3.forward * 1.5f * Vector3.Distance(_managedRenderer.bounds.center, _managedRenderer.bounds.max);
                _previewCam.transform.LookAt(_managedObject.transform);
            }
        }
    }

    private void LateUpdate()
    {
        _UICam.fieldOfView = _cam.fieldOfView;
    }

    public void EnablePreviewCam()
    {
        _previewCam.enabled = true;
        _eye.SetActive(false);
    }
    public void DisablePreviewCam()
    {
        _previewCam.enabled = false;
        _eye.SetActive(true);
    }

    public void SetPreviewCam(ManageableObjet m)
    {
        EnablePreviewCam();
        _managedObject = m;
        _managedRenderer = _managedObject.gameObject.GetComponent<Renderer>();
        if (_managedRenderer == null)
        {
            foreach (Transform child in m.transform)
            {
                _managedRenderer = child.gameObject.GetComponent<Renderer>();
            }
        }
        
    }

    private void OnGUI()
    {
        if (_infoGUI)
        {
            GUI.depth = 5;
            float screenSizeX = Screen.width;
            float screenSizeY = Screen.height;
            float yStart = Screen.height * 2 / 3;
            float ySize = (Screen.height / 3) - 20f;
            float xStart = 10;
            float xSize = Screen.width - 20;
            GUI.DrawTextureWithTexCoords(new Rect(xStart-5, yStart-5, xSize+10, ySize+10), _infoGUITexFont, new Rect((2 * (xStart-5)) / screenSizeX, (yStart-5) / screenSizeY, (xSize+10) / screenSizeX, (ySize+10) / screenSizeY));
            GUI.DrawTextureWithTexCoords(new Rect(xStart, yStart, xSize, ySize), _infoGUITex, new Rect((2*xStart)/screenSizeX, yStart/screenSizeY, xSize/screenSizeX, ySize/screenSizeY));//Screen.height/(Screen.width*1/3)));
            float a = (Screen.height / 3) * _infoGUITex.width / Screen.width;
            _previewCam.Render();
            _UICam.Render();

            
        }
    }
}
