using UnityEngine;
using UnityEngine.UI;

public class UIResources : MonoBehaviour {

    private Animator mouthAnim, bodyAnim;
    [SerializeField]
    SpriteRenderer[] sprites;
    [SerializeField]
    private GameObject _body;
    private ThingBuilder _builder;
    [SerializeField]
    private Text _woodText, _waterText, _stoneText;
    private bool _resourcesShowing;
    private int _woodAmount, _waterAmount, _stoneAmount;
    private static UIResources _instance;

    public bool Selected { get; set; }
    public bool MouseOver { get; private set; }
    public static UIResources Instance { get { if (_instance == null) _instance = FindObjectOfType<UIResources>(); return _instance; } }

    private void Start()
    {
        _woodText.text = "Wood : " + _woodAmount;
        _waterText.text = "Water : " + _waterAmount;
        _stoneText.text = "Stone : " + _stoneAmount;
        mouthAnim = transform.Find("bottom").GetComponent<Animator>();
        bodyAnim = transform.Find("body").GetComponent<Animator>();
        _builder = ThingBuilder.Instance;
        for (int a = 0; a < _builder.Constructables.Length; a++)
        {
            GameObject uiThing = Instantiate(_builder.Constructables[a]);
            uiThing.transform.SetParent(_body.transform, true);
            
            uiThing.transform.localScale = Vector3.one;
            uiThing.transform.localPosition = Vector3.zero;
            uiThing.layer = 14;
        }

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
        if (Selected)
        {
            if (Input.GetMouseButtonDown(0) && MouseOver == false)
            {
                PlayMouthClose();
                foreach (SpriteRenderer s in sprites)
                {
                    s.material.color = new Color(1, 1, 1);
                }
                Selected = false;
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
        MouseOver = true;
        if (Selected == false)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(0, .6f, .8f);
            }
        }
    }

    private void OnMouseExit()
    {
        MouseOver = false;
        if (Selected == false)
        {
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(1, 1, 1);
            }
        }
    }

    private void OnMouseDown()
    {
        if (Selected == false)
        {
            Selected = true;
            PlayMouthOpen();
            foreach (SpriteRenderer s in sprites)
            {
                s.material.color = new Color(0, .8f, 1f);
            }
        }
    }
}
