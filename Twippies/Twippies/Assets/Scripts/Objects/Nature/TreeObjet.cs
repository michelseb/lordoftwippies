using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObjet : StaticObjet, IConsumable {

    private float _size;

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
        _size = _initSize.x * 2;
        _zone.Ressource.ressourceType = Ressources.RessourceType.Food;
        _zone.Ressource.consumableObject = this as IConsumable;
    }

    public float Size
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

    protected override void Update()
    {
        base.Update();
        transform.localScale = _size * Vector3.one;
    }

    public bool Consuming()
    {
        _size -= Time.deltaTime;
        if (_size > 0)
            return true;
        return false;
    }

    public void Consume()
    {
        _zone.Ressource.ressourceType = Ressources.RessourceType.None;
        _om.allObjects.Remove(this);
        Destroy(gameObject);
        
    }
}
