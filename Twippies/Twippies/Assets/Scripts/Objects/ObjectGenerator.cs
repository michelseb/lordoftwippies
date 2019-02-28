using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {

    public class GlobalStat
    {
        private StatManager _statManager;
        private string _type;

        public GlobalStat(GameObject go, string type)
        {
            _statManager = go.GetComponent<StatManager>();
            _statManager.StatsList = new List<Stat>();
            _type = type;
        }

        public StatManager StatManager
        {
            get
            {
                return _statManager;
            }
        }
        public string Type
        {
            get
            {
                return _type;
            }
        }
    }

    [SerializeField]
    private float _spawnTime;
    [SerializeField]
    public List<ManageableObjet> ObjectFactory;
    [SerializeField]
    public GameObject StatPanel;
    [SerializeField]
    public GameObject GlobalStatObject;
    [SerializeField]
    public UIContent SpecificStatPanel;
    public List<GlobalStat> globalStats;
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
        globalStats = new List<GlobalStat>();

        foreach (ManageableObjet objet in ObjectFactory)
        {
            GameObject globalStatObject = Instantiate(GlobalStatObject, transform);
            GlobalStat globalStat = new GlobalStat(globalStatObject, objet.GetType().ToString());
            Debug.Log("nouveau globalstat de type " + objet.GetType().ToString());
            globalStat.StatManager.GenerateStat<ValueStat>(mainStat: true).Populate(0, 0, 100, "Nombre de " + objet.Type.Split(' ')[0] + "s", true);
            globalStats.Add(globalStat);
            globalStat.StatManager.SetStatsActiveState(false);
            globalStat.StatManager.enabled = false;
        }

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

    public GameObject GetStat<T>(string statType = "") where T : Stat
    {
        List<Stat> stats = StatFactory.FindAll(x => x is T);
        if (stats != null)
        {
            if (stats.Count == 1 || statType == "")
                return stats[0].gameObject;
            return stats.FirstOrDefault(x => x.Name == statType).gameObject;
        }
            
        return null;
    }

    public List<ManageableObjet> GetObjects<T>() where T : class
    {
        List<ManageableObjet> result = new List<ManageableObjet>();
        result = ObjectFactory.FindAll(x => x is T);
        return result;
    }

    public void GenerateGlobalStats(ManageableObjet objet)
    {
        
        GlobalStat globalStat = globalStats.FirstOrDefault(x => x.Type == objet.GetType().ToString());
        if (globalStat != null)
        {
            if (globalStat.StatManager.StatsList.Count == 1) //Stat de base qui compte le nombre d'occurences
            {
                foreach (Stat stat in objet.Stats.StatsList)
                {
                    globalStat.StatManager.StatsList.Add(stat);
                    Debug.Log("Ajout de la stat " + stat.GetType().ToString()+ " à la global stat "+ globalStat.Type);
                }
                UpdateGlobalStat(objet.GetType().ToString(), 1);
                Debug.Log("Global stat " + globalStat.Type + " mis à jour");
            } 
        }
    }

    public bool SetGlobalStatActiveState(bool active, string type)
    {
        GlobalStat globalStat = globalStats.FirstOrDefault(x => x.Type == type);
        if (globalStat != null)
        {
            globalStat.StatManager.SetStatsActiveState(active);
            return true;
        }
        return false;
    }

    public void SetAllGlobalStatsActiveState(bool active)
    {
        foreach (GlobalStat globalStat in globalStats)
        {
            globalStat.StatManager.SetStatsActiveState(active);
        }
    }

    public void UpdateGlobalStat(string type, int value)
    {
        GlobalStat globalStat = globalStats.FirstOrDefault(x => x.Type == type);
        if (globalStat != null)
        {
            globalStat.StatManager.StatToValue(globalStat.StatManager.StatsList[0]).Value+=value;
        }
    }

}
