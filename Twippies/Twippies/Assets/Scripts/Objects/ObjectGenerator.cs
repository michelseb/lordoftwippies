using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{

    [SerializeField] private float _spawnTime;
    [SerializeField] private List<ManageableObjet> _objectFactory;
    [SerializeField] private List<Stat> _statFactory;
    [SerializeField] private List<UserAction> _actionFactory;
    [SerializeField] private int _nbTwippies;
    [SerializeField] private int _nbTrees;
    [SerializeField] private int _nbAdvancedTwippies;
    [SerializeField] private Planet _planet;
    [SerializeField] private ActionMenu _actionMenu;

    public List<ManageableObjet> ObjectFactory => _objectFactory;

    private ZoneManager _zoneManager;
    public RadialMenu RadialPanel { get; set; }
    private static ObjectGenerator _instance;
    public static ObjectGenerator Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ObjectGenerator>();

            return _instance;
        }
    }

    private void Awake()
    {
        _zoneManager = ZoneManager.Instance;
        RadialPanel = _planet.MainRadial;
        Instantiate(_actionMenu, RadialPanel.transform.parent.transform);

        // STATS DO NOT DELETE
        //foreach (var objet in _objectFactory)
        //{
        //    objet.Stats.Type = objet.Type;
        //    _objectManager.StatManagers.Add(objet.Stats);
        //    objet.Stats.Init();
        //    objet.GenerateActions();
        //}
        //RadialPanel.Arrange();

    }

    public void Generate()
    {
        StartCoroutine(GenerateWorld());
    }

    private IEnumerator GenerateWorld()
    {
        var zones = new List<Zone>();

        //for (int b = 0; b < _zoneManager.Zones.Length; b++)
        //{
        //    if (_zoneManager.Zones[b].Height > 5.8f)
        //    {
        //        zones.Add(_zoneManager.Zones[b]);
        //        _zoneManager.Zones[b].Accessible = false;
        //    }
        //}

        //foreach (var zone in _zoneManager.Zones)
        //{
        //    zone.Accessible = true;
        //}

        for (int a = 0; a < _nbTrees; a++)
        {
            var zone = _zoneManager.Zones.OrderBy(x => Guid.NewGuid()).FirstOrDefault(x => x.Accessible);
            if (zone == null)
                continue;

            var tree = Instantiate(Get<TreeObjet>(), zone.WorldPos, Quaternion.identity, transform);
            tree.Age = 5;
            zone.Accessible = false;
            zones.Add(zone);
        }

        zones.ForEach(zone => zone.Accessible = true);

        zones = new List<Zone>();
        var availableZones = _zoneManager.Zones.Where(z => !z.Taken && z.Accessible).ToList();

        for (int a = 0; a < _nbTwippies; a++)
        {
            if (availableZones.Count == 0)
                continue;

            var zIndex = UnityEngine.Random.Range(0, availableZones.Count);
            var zone = availableZones[zIndex];

            var position = zone.WorldPos + (zone.WorldPos - _planet.transform.position).normalized;
            Instantiate(Get<Twippie>(), position, Quaternion.identity);

            if (_spawnTime > 0)
                yield return new WaitForSeconds(_spawnTime);
        }

        for (int a = 0; a < _nbAdvancedTwippies; a++)
        {
            for (int b = 0; b < _zoneManager.Zones.Length; b++)
            {
                if (_zoneManager.Zones[b].Accessible)
                {
                    Instantiate(Get<AdvancedTwippie>(), _zoneManager.Zones[b].WorldPos, Quaternion.identity);
                    zones.Add(_zoneManager.Zones[b]);
                    _zoneManager.Zones[b].Accessible = false;
                    break;
                }
            }

            if (_spawnTime > 0)
                yield return new WaitForSeconds(_spawnTime);
        }

        zones.ForEach(z => z.Accessible = true);
    }

    public T Get<T>() where T : ManageableObjet
    {
        var obj = _objectFactory.FirstOrDefault(x => x is T);

        if (obj != null)
            return (T)obj;

        return null;
    }

    public T GetAction<T>() where T : UserAction
    {
        return _actionFactory.FirstOrDefault(x => x is T) as T;
    }

    public T GetStat<T>(string statType = "") where T : Stat
    {
        var stats = _statFactory.FindAll(x => x is T);

        if (stats == null || stats.Count == 0)
            return null;

        return string.IsNullOrEmpty(statType) ?
            (T)stats[0] :
            (T)stats.FirstOrDefault(x => x.Name == statType);
    }

    public List<ManageableObjet> GetObjects<T>() where T : class
    {
        return _objectFactory.FindAll(x => x is T);
    }
}
