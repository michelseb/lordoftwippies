using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeObjet : StaticObjet, IConsumable, ICollectable, ILightnable {

    private bool _spread;
    [SerializeField]
    private GameObject _childTree;
    protected float _sunAmount;
    protected float _waterAmount;
    protected List<Zone> _waterZones;

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
        _outline.Color = 1;
        Zone.Accessible = true;
    }

    protected override void Update()
    {
        base.Update();
        var zone = Zone;

        transform.localScale = Vector3.one * _ageProgression / 10;

        if (_ageProgression > 1 && !_spread)
        {
            zone.Accessible = false;
            zone.Resources.Add(new Resource(ResourceType.Food, this, 0));
            Spread();
        }

        if (GetLight())
        {
            _sunAmount = UpdateValue(_sunAmount, 3f);
        }

        var waterZone = _waterZones.FirstOrDefault();

        if (waterZone != null)
        {
            if (!waterZone.GetResourceByType(ResourceType.Drink).Consume(zone))
            {
                _waterZones.Remove(waterZone);
            }
            else
            {
                _waterAmount = UpdateValue(_waterAmount, 2f);
            }
        }

        if (_waterAmount > 0 && _sunAmount > 0)
        {
            Grow();
        }
        _woodCost = 5 * Mathf.FloorToInt(_currentSize.x);
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
        var zone = Zone;
        zone.Taken = false;
        if (_currentSize.x <= .1f)
        {
            var food = zone.GetResourceByType(ResourceType.Food);
            if (food != null)
            {
                zone.Resources.Remove(food);
            }
            _objectManager.RemoveObject(this);
            if (_stats != null)
            {
                _stats.enabled = false;
            }
            Destroy(this);
        }
        else
        {
            zone.Accessible = false;
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
        if (_rigidBody != null)
        {
            _rigidBody.isKinematic = false;
            _rigidBody.mass = 5;
            _rigidBody.AddForce(transform.up * 10, ForceMode.Impulse);
        }
        yield return new WaitForSeconds(1);
        Collect(twippie);
    }
    public void Collect(AdvancedTwippie twippie)
    {
        Debug.Log("Tree collected");
        twippie.Collecting = null;
        Zone.Taken = false;
        var resource = twippie.Ressources.FirstOrDefault(x => x.ResourceType == ResourceType.Food);

        if (resource != null)
        {
            resource.Quantity += _woodCost;
        }
        else
        {
            twippie.Ressources.Add(new Resource(ResourceType.Food, _woodCost));
        }

        twippie.FinishExternalAction();
        _objectManager.RemoveObject(this);
        //_stats.enabled = false;
        Destroy(_rigidBody);
        Destroy(this);
    }

    public void Spread()
    {
        foreach(var zoneId in Zone.NeighbourIds)
        {
            if (!Utils.CoinFlip())
                continue;

            var zone = _zoneManager.FindById(zoneId);

            if (zone.Accessible)
            {
                var tree = Instantiate(_childTree, zone.WorldPos, Quaternion.identity);
                zone.Resources.Add(new Resource(ResourceType.Food, tree.GetComponent<IConsumable>(), 0));
                tree.transform.localScale = Vector3.zero;
            } 
        }

        _spread = true;
    }

    public void Grow()
    {
        _sunAmount = UpdateValue(_sunAmount, -1);
        _waterAmount = UpdateValue(_waterAmount, -1);
        _initSize = UpdateVector(_initSize, (100 - _ageProgression) / 100 * .03f, 0, 10);
    }

    public override void GenerateActions()
    {
        Stats.GenerateRadialAction<ModificationAction>(this);
        base.GenerateActions();
    }

    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<ValueStat>().Populate("Water", 0, 0, 100, "Water Amount", true);
        Stats.GenerateWorldStat<ValueStat>().Populate("Sun", 30, 0, 100, "Sun Amount", true);
    }

    public override void PopulateStats()
    {
        base.PopulateStats();
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Water", 0, 0, 100, "Water Amount", true });
        _objectGenerator.RadialPanel.PopulateStatPanel(_stats, new object[] { "Sun", 30, 0, 100, "Sun Amount", true });
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        _stats.StatToValue(_stats.GetStat("Water")).Value = _waterAmount;
        _stats.StatToValue(_stats.GetStat("Sun")).Value = _sunAmount;
    }

    public bool GetLight()
    {
        return _sun != null && Vector3.Dot(transform.position - _planet.transform.position, transform.position - _sun.transform.position) > 0f;
        //RaycastHit hit;
        //if (_sun != null)
        //{
        //    if (Physics.Linecast(_sun.transform.position, transform.position + transform.up / 2, out hit, _mask))
        //    {
        //        return false;
        //    }
        //}
        //return true;
    }

    protected override void OnZoneChanged(Zone zone)
    {
        _waterZones = zone.NeighbourIds.Select(n => _zoneManager.FindById(n)).Where(x => x.HasResource(ResourceType.Drink)).ToList();

        if (zone.HasResource(ResourceType.Drink))
        {
            _waterZones.Add(zone);
        }
    }
}
