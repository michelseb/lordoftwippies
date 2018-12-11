using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objet : MonoBehaviour {

    [SerializeField]
    protected Sprite _icon;

    protected Vector3 _dist, _initSize, _currentSize;
    protected ObjetManager _o;
    protected float _posX, _posY;
    protected int _woodCost, _waterCost, _stoneCost;
    
    protected Camera _cam;

    protected virtual void Awake()
    {
        _o = ObjetManager.Instance;
        _cam = Camera.main;
    }

    protected virtual void Start()
    {
        _initSize = transform.localScale;
        _currentSize = _initSize;
    }

    public int WoodCost
    {
        get
        {
            return _woodCost;
        }
    }

    public int WaterCost
    {
        get
        {
            return _waterCost;
        }
    }

    public int StoneCost
    {
        get
        {
            return _stoneCost;
        }
    }

    public Sprite Icon
    {
        get
        {
            return _icon;
        }
    }

}
