using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using UnityEngine.UI;

public abstract class ManageableObjet : Objet {

    [SerializeField]
    protected StatManager _stats;

    

    protected Controls _c;
    protected float _rotSpeedX, _rotSpeedY, _rotSpeedMultiplier = 10;
    protected float _sizeMultiplier = 2.3f;
    protected cakeslice.Outline _outline;
    protected Collider _coll;
    protected Renderer _renderer;


    protected abstract void GenerateStats();

    protected override void Awake()
    {
        base.Awake();
        _coll = GetComponent<Collider>();
        _renderer = GetComponent<Renderer>();
        if (_coll == null)
        {
            foreach (Transform child in transform)
            {
                _coll = child.GetComponent<Collider>();
                _renderer = child.GetComponent<Renderer>();
                if (_coll != null)
                {
                    continue;
                }
            }
        }
        _c = Controls.Instance;
        _outline = GetComponent<cakeslice.Outline>();
        if (_outline == null)
        {
            if (GetComponent<Collider>() != null)
            {
                _outline = gameObject.AddComponent<cakeslice.Outline>();
            }
            else
            {
                foreach(Transform child in transform)
                {
                    if (child.gameObject.GetComponent<Collider>() != null && child.gameObject.GetComponent<cakeslice.Outline>() == null)
                    {
                        _outline = child.gameObject.AddComponent<cakeslice.Outline>();
                    }
                    else if (child.gameObject.GetComponent<cakeslice.Outline>() != null)
                    {
                        _outline = child.gameObject.GetComponent<cakeslice.Outline>();
                    }
                }
            }
        }
        if (_outline == null)
        {
            gameObject.AddComponent<Collider>();
            _outline = gameObject.AddComponent<cakeslice.Outline>();
        }
    }

    protected override void Start()
    {
        base.Start();
        GenerateStats();
        _outline.enabled = false;
        _stats.enabled = false;
    }


    protected virtual void Update()
    {
        transform.RotateAround(transform.position, _cam.transform.up, -_rotSpeedX * _rotSpeedMultiplier);
        transform.RotateAround(transform.position, _cam.transform.right, _rotSpeedY * _rotSpeedMultiplier);
        _rotSpeedX = Mathf.Lerp(_rotSpeedX, 0, .05f);
        _rotSpeedY = Mathf.Lerp(_rotSpeedY, 0, .05f);

        if (Input.GetMouseButtonDown(0))
        {
            if (_c.FocusedObject != this && _outline.enabled && Input.mousePosition.y > Screen.height * 1/3)
            {
                _outline.enabled = false;
            }
        }

        
    }


    protected virtual void OnMouseEnter()
    {
        _outline.enabled = true;
    }


    protected virtual void OnMouseExit()
    {
        if (_c.FocusedObject != this)
        {
            _outline.enabled = false;
        }
        transform.localScale = _initSize;
    }


    protected virtual void OnMouseUp()
    {
        if (_c.FocusedObject != this)
        {
            _outline.enabled = false;
        }
        transform.localScale = _initSize;
    }

    private void OnMouseDown()
    {
        SetFocus();
    }

    protected virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            SetFocus();
        }
    }

    protected virtual void OnMouseDrag()
    {
        RotateObjet();
    }

    public void RotateObjet()
    {
        _rotSpeedX = Input.GetAxis("Mouse X");
        _rotSpeedY = Input.GetAxis("Mouse Y");
    }

    public void AbsRotateObjet()
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 rotationVector = (screenCenter - new Vector2(Input.mousePosition.x, Input.mousePosition.y)).normalized;
        _rotSpeedX = rotationVector.x*.1f;
        _rotSpeedY = rotationVector.y*.1f;
    }

    public Collider Coll
    {
        get
        {
            return _coll;
        }
    }

    public StatManager Stats
    {
        get
        {
            return _stats;
        }

        set
        {
            _stats = value;
        }
    }

    private void SetFocus()
    {
        
        _c.FocusedObject = this;
        _dist = _cam.WorldToScreenPoint(transform.position);
        _posX = Input.mousePosition.x - _dist.x;
        _posY = Input.mousePosition.y - _dist.y;
        Debug.Log("Focused : "+_c.FocusedObject);
    }
    

}
