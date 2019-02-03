using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject _twippie;
    [SerializeField]
    private GameObject _tree;
    [SerializeField]
    private int _nbTwippies;
    [SerializeField]
    private int _nbTrees;
    private ObjetManager _om;
    private ZoneManager _zm;

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
                    GameObject tree = Instantiate(_tree, _zm.Zones[b].Center, Quaternion.identity, transform);
                    ManageableObjet mo = tree.GetComponent<ManageableObjet>();
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
                    GameObject twippie = Instantiate(_twippie, _zm.Zones[b].Center, Quaternion.identity, transform);
                    _om.allObjects.Add(twippie.GetComponent<ManageableObjet>());
                    z.Add(_zm.Zones[b]);
                    _zm.Zones[b].Accessible = false;
                    break;
                }
            }
            yield return new WaitForSeconds(1);
        }
        foreach (Zone zone in z)
        {
            zone.Accessible = true;
        }
        z = null;

    }
}
