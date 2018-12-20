using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObjet : StaticObjet {

    private Vector3 _size;

    protected override void Awake()
    {
        base.Awake();
        _type = "Pomo (Plante verte)";
        _name = "Plante verte";
    }
    protected override void Start()
    {
        base.Start();
        _outline.color = 1;
        _woodCost = 5;
    }

    public Vector3 Size
    {
        get
        {
            return _size;
        }
        set
        {
            _size = value;
        }
    }
}
