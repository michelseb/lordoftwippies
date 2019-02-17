using UnityEngine;
using System.Linq;
using System.Collections;

public class TreeObjet : StaticObjet, IConsumable, ICollectable, ILightnable {

    private bool _spread;
    [SerializeField]
    private GameObject _childTree;
    protected float _sunAmount;
    protected float _waterAmount;
    [SerializeField]
    private LayerMask _mask;

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
        _zone.Accessible = true;
    }

    protected override void Update()
    {
        base.Update();
        if (_age > 1 && _currentSize.x > 1 && !_spread)
        {
            _zone.Accessible = false;
            _zone.Ressources.Add(new Ressource(Ressource.RessourceType.Food, this as IConsumable, 0));
            Spread();
        }

        if (GetLight())
        {
            _sunAmount = UpdateValue(_sunAmount, 3f);
        }

        // Check currzone + neighbours for water
        if (_zone.Ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Drink))
        {
            _zone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Drink).Consume(_zone);
            _waterAmount = UpdateValue(_waterAmount, 2f);
        }
        else
        {
            Zone waterZone = _zManager.GetZoneByRessourceInList(_zone.Neighbours, Ressource.RessourceType.Drink);
            if (waterZone != null) 
            {
                waterZone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Drink).Consume(waterZone);
                _waterAmount = UpdateValue(_waterAmount, 2f);
            }
        }

        if (_waterAmount > 0 && _sunAmount > 0)
        {
            Grow();
        }
        
        _stats.StatToValue(_stats.StatsList[3]).Value = _waterAmount;
        _stats.StatToValue(_stats.StatsList[4]).Value = _sunAmount;
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
        _zone.Taken = false;
        if (_currentSize.x <= .1f)
        {
            Ressource food = _zone.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Food);
            if (food != null)
            {
                _zone.Ressources.Remove(food);
            }
            _om.allObjects.Remove(this);
            _stats.enabled = false;
            Destroy(this);
        }
        else
        {
            _zone.Accessible = false;
            Liberate();
        }
    }

    public void Reserve()
    {
        _renderer.material.color = Color.blue;
    }

    public void Liberate()
    {
        _renderer.material.color = Color.white;
    }

    public IEnumerator Collecting(AdvancedTwippie twippie)
    {
        _r.isKinematic = false;
        _r.AddTorque(Vector3.up, ForceMode.Impulse);
        yield return new WaitForSeconds(1);
        Collect(twippie);
    }
    public void Collect(AdvancedTwippie twippie)
    {
        twippie.Collecting = null;
        _zone.Taken = false;
        Ressource ressource = twippie.Ressources.FirstOrDefault(x => x.ressourceType == Ressource.RessourceType.Food);
        if (ressource != null)
        {
            ressource.quantity += WOODCOST;
        }
        else
        {
            twippie.Ressources.Add(new Ressource(Ressource.RessourceType.Food, WOODCOST));
        }
        Destroy(this);
    }

    public void Spread()
    {
        foreach(Zone zone in _zone.Neighbours)
        {
            if (zone.Accessible && CoinFlip() && zone.MaxHeight < 5.8f)
            {
                var tree = Instantiate(_childTree, zone.Center, Quaternion.identity);
                zone.Ressources.Add(new Ressource(Ressource.RessourceType.Food, tree.GetComponent<IConsumable>(), 0));
                tree.transform.localScale = Vector3.zero;
                _om.allObjects.Add(tree.GetComponent<ManageableObjet>());
            }
            
        }
        _spread = true;
    }

    public void Grow()
    {
        _sunAmount = UpdateValue(_sunAmount, -1);
        _waterAmount = UpdateValue(_waterAmount, -1);
        _currentSize = UpdateVector(_currentSize, (100 - _age) / 100 * .03f, 0, 10);
    }

    protected override void GenerateStats()
    {
        base.GenerateStats();

        _stats.StatsList[3] = new ValueStat(0, 0, 100, "Water Amount", true);
        _stats.StatsList[4] = new ValueStat(30, 0, 100, "Sun Amount", true);
    }

    public bool GetLight()
    {
        RaycastHit hit;
        if (_planetSun != null)
        {
            if (Physics.Linecast(_planetSun.transform.position, transform.position + transform.up / 2, out hit, _mask))
            {
                return false;
            }
        }
        return true;
    }
}
