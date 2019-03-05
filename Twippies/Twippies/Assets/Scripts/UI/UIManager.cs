using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField]
    private Camera _previewCam, _UICam;
    [SerializeField]
    private GameObject _eye;
    [SerializeField]
    private GameObject _mainStatPanel;

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
        _mainStatPanel.SetActive(false);
    }

    private void Update()
    {

        if (_infoGUI)
        {
            if (!_mainStatPanel.activeSelf)
            {
                _mainStatPanel.SetActive(true);
            }
            _previewCam.Render();
            _UICam.Render();
        }
        else
        {
            if (_mainStatPanel.activeSelf)
            {
                _mainStatPanel.SetActive(false);
            }
        }


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

    public Camera UICam { get { return _UICam; } }
}
