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
        WOODCOST = 5 * Mathf.FloorToInt(_currentSize.x);
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
            _om.UpdateObjectList(this, false);
            if (_stats != null)
            {
                _stats.enabled = false;
            }
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
        if (_r != null)
        {
            _r.isKinematic = false;
            _r.mass = 5;
            _r.AddForce(transform.up * 10, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1);
        Collect(twippie);
    }
    public void Collect(AdvancedTwippie twippie)
    {
        Debug.Log("Tree collected");
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
        twippie.FinishExternalAction();
        _om.UpdateObjectList(this, false);
        //_stats.enabled = false;
        Destroy(_r);
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
            }
            
        }
        _spread = true;
    }

    public void Grow()
    {
        _sunAmount = UpdateValue(_sunAmount, -1);
        _waterAmount = UpdateValue(_waterAmount, -1);
        _initSize = UpdateVector(_initSize, (100 - _age) / 100 * .03f, 0, 10);
    }

    public override void GenerateStats(StatPanel statPanel)
    {
        base.GenerateStats(statPanel);
        statPanel.StatManager.GenerateStat<ValueStat>().Populate(0, 0, 100, "Water Amount", true, "Water");
        statPanel.StatManager.GenerateStat<ValueStat>().Populate(30, 0, 100, "Sun Amount", true, "Sun");
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Water"), new object[] { 0, 0, 100, "Water Amount", true, "Water" });
        _og.MainPanel.PopulateStatPanel(_stats.GetStat("Sun"), new object[] { 30, 0, 100, "Sun Amount", true, "Sun" });
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        _stats.StatToValue(_stats.GetStat("Water")).Value = _waterAmount;
        _stats.StatToValue(_stats.GetStat("Sun")).Value = _sunAmount;
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
