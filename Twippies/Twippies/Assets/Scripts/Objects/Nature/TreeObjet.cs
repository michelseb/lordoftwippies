using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObjet : StaticObjet, IConsumable {

    private bool _spread;
    [SerializeField]
    private GameObject _childTree;

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
        _zone.Ressource.ressourceType = Ressources.RessourceType.Food;
        _zone.Ressource.consumableObject = this as IConsumable;
    }

    protected override void Update()
    {
        base.Update();
        transform.localScale = _currentSize;
        if (_age > 0.1f && !_spread)
        {
            Spread();
        }
        _currentSize = UpdateVector(_currentSize, (100-_age)/100 * .2f, 10);
    }

    public bool Consuming(float hunger)
    {
        _currentSize = UpdateVector(_currentSize, -1);
        if (_currentSize.x > 0 && hunger > 20)
            return true;
        return false;
    }

    public void Consume()
    {
        if (_currentSize.x <= 0)
        {
            _zone.Ressource.ressourceType = Ressources.RessourceType.None;
            Debug.Log("Arbre mangé");
            _om.allObjects.Remove(this);
            Destroy(gameObject);
        }
    }

    public void Spread()
    {
        foreach(Zone zone in _zone.Neighbours)
        {
            if (zone.Accessible && zone.Ressource.ressourceType == Ressources.RessourceType.None && CoinFlip() && zone.MaxHeight < 5.8f)
            {
                zone.Ressource.ressourceType = Ressources.RessourceType.Food;
                var tree = Instantiate(_childTree, zone.Center, Quaternion.identity);
                tree.transform.localScale = Vector3.zero;
                zone.Ressource.consumableObject = tree.GetComponent<IConsumable>();
                _om.allObjects.Add(tree.GetComponent<ManageableObjet>());
            }
            
        }
        _spread = true;
    }
}
