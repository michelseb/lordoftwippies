using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {
    [SerializeField]
    private float _spawnTime;
    [SerializeField]
    public List<ManageableObjet> ObjectFactory;
    [SerializeField]
    public List<Stat> StatFactory;
    [SerializeField]
    private int _nbTwippies;
    [SerializeField]
    private int _nbTrees;
    [SerializeField]
    private int _nbAdvancedTwippies;
    private ObjetManager _om;
    private ZoneManager _zm;

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
        _om = ObjetManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(WaitForZoneManager());
        
    }

    private IEnumerator WaitForZoneManager()
    {
        while (_zm == null)
        {
            _zm = _om.ActivePlanet.ZManager;
            yield return null;
        }
        List<Zone> z = new List<Zone>();
        for (int b = 0; b < _zm.Zones.Length; b++)
        {
            if (_zm.Zones[b].MaxHeight > 5.8f)
            {
                z.Add(_zm.Zones[b]);
                _zm.Zones[b].Accessible = false;
            }
        }

        for (int a = 0; a < _nbTrees; a++)
        {
            for (int b = 0; b < _zm.Zones.Length; b++)
            {
                if (_zm.Zones[b].Accessible)
                {
                    GameObject tree = Instantiate(GetGO<TreeObjet>(), _zm.Zones[b].Center, Quaternion.identity, transform);
                    ManageableObjet mo = tree.GetComponent<ManageableObjet>();
                    mo.Age = 1;
                    _om.allObjects.Add(mo);
                    mo.Age = 5;
                    _zm.Zones[b].Accessible = false;
                    break;
                }
            }
        }

        foreach (Zone zone in z)
        {
            zone.Accessible = true;
        }
        z = null;


        z = new List<Zone>();
        for (int a = 0; a < _nbTwippies; a++)
        {
            for (int b = 0; b < _zm.Zones.Length; b++)
            {
                if (_zm.Zones[b].Accessible)
                {
                    GameObject twippie = Instantiate(GetGO<Twippie>(), _zm.Zones[b].Center, Quaternion.identity, transform);
                    _om.allObjects.Add(twippie.GetComponent<ManageableObjet>());
                    z.Add(_zm.Zones[b]);
                    _zm.Zones[b].Accessible = false;
                    break;
                }
            }
            if (_spawnTime > 0)
                yield return new WaitForSeconds(_spawnTime);
        }
        for (int a = 0; a < _nbAdvancedTwippies; a++)
        {
            for (int b = 0; b < _zm.Zones.Length; b++)
            {
                if (_zm.Zones[b].Accessible)
                {
                    GameObject twippie = Instantiate(GetGO<AdvancedTwippie>(), _zm.Zones[b].Center, Quaternion.identity, transform);
                    _om.allObjects.Add(twippie.GetComponent<ManageableObjet>());
                    z.Add(_zm.Zones[b]);
                    _zm.Zones[b].Accessible = false;
                    break;
                }
            }
            if (_spawnTime > 0)
                yield return new WaitForSeconds(_spawnTime);
        }
        foreach (Zone zone in z)
        {
            zone.Accessible = true;
        }
        z = null;

    }

    public GameObject GetGO<T>() where T : ManageableObjet
    {
        ManageableObjet obj = ObjectFactory.FirstOrDefault(x => x is T);
        if (obj != null)
            return obj.gameObject;
        return null;
    }

    public GameObject GetStat<T>() where T : Stat
    {
        Stat stat = StatFactory.FirstOrDefault(x => x is T);
        if (stat != null)
            return stat.gameObject;
        return null;
    }

    public List<ManageableObjet> GetObjects<T>() where T : class
    {
        List<ManageableObjet> result = new List<ManageableObjet>();
        result = ObjectFactory.FindAll(x => x is T);
        return result;
    }
}
