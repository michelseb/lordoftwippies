using UnityEngine;
using UnityEngine.UI;

public class UIResources : MonoBehaviour {

    private Animator mouthAnim, bodyAnim;
    [SerializeField]
    SpriteRenderer[] sprites;
    [SerializeField]
    private ObjetManager _om;
    private ThingBuilder _builder;
    private Controls _c;
    [SerializeField]
    private Text _woodText, _waterText, _stoneText;
    private bool _selected;
    private bool _mouseOver;
    private bool _resourcesShowing;
    private int _woodAmount, _waterAmount, _stoneAmount;


    private static UIResources _instance;
    public static UIResources Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIResources>();

            return _instance;
        }
    }

    private void Start()
    {
        _woodText.text = "Wood : " + _woodAmount;
        _waterText.text = "Water : " + _waterAmount;
        _stoneText.text = "Stone : " + _stoneAmount;
        mouthAnim = transform.Find("bottom").GetComponent<Animator>();
        bodyAnim = transform.Find("body").GetComponent<Animator>();
        _builder = ThingBuilder.Instance;
        _om = ObjetManager.Instance;
        _c = Controls.Instance;
    }


    public void PlayMouthOpen()
    {
        mouthAnim.SetTrigger("Open");
        bodyAnim.SetTrigger("Increase");
    }

    public void PlayMouthClose()
    {
        mouthAnim.SetTrigger("Close");
        bodyAnim.SetTrigger("Decrease");
    }

    private void Update()
    {
        if (_selected)
        {
            if (Input.GetMouseButtonDown(0) && _mouseOver == false)
            {
                PlayMouthClose();
                foreach (SpriteRenderer s in sprites)
                {
                    s.material.color = new Color(1, 1, 1);
                }
                _selected = false;
            }
        }
    }

    public void AddResources(int wood, int water, int stone)
    {
        _woodAmount += wood;
        _waterAmount += water;
        _stoneAmount += stone;
        _woodText.text = "Wood : " + _woodAmount;
        _waterText.text = "Water : " + _waterAmount;
        _stoneText.text = "Stone : " + _stoneAmount;
    }


    private void OnMouseOver()
    {
        _mouseOver = true;
        if (_selected == false)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(0, .6f, .8f);
            }
        }
    }

    private void OnMouseExit()
    {
        _mouseOver = false;
        if (_selected == false)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(1, 1, 1);
            }
        }
    }

    private void OnMouseDown()
    {
        if (_selected == false)
        {
            _selected = true;
            PlayMouthOpen();
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(0, .8f, 1f);
            }
        }
    }

    private void OnGUI()
    {
        if (_selected)
        {
            if (mouthAnim.GetCurrentAnimatorStateInfo(0).IsName("MouthOpen"))
            {
                Event evt = Event.current;
                for (int a = 0; a < _builder.Constructables.Length; a++)
                {
                    Rect rect = new Rect(40, 140 + a * 60, 50, 50);
                    GUI.DrawTexture(rect, _builder.Constructables[a].Icon);
                    if (rect.Contains(evt.mousePosition))
                    {
                        if (Input.GetMouseButtonDown(0) && _c.ctrl != Controls.ControlMode.Dragging)
                        {
                            _c.ctrl = Controls.ControlMode.Dragging;
                            //Cursor.lockState = CursorLockMode.Locked;
                            GameObject go = Instantiate(_builder.Constructables[a].gameObject, Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
                            if (go.GetComponent<ManageableObjet>() != null)
                            {
                                _c.FocusedObject = go.GetComponent<ManageableObjet>();
                                _om.allObjects.Add(_c.FocusedObject);
                            }
                            //Cursor.lockState = CursorLockMode.None;
                            
                        }
                    }
                }
            }
        }
    }

    public bool MouseOver
    {
        get
        {
            return _mouseOver;
        }
    }



}
