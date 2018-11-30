using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject _twippie;
    [SerializeField]
    private int _nbObjets;
    private ObjetManager _om;

    private void Awake()
    {
        _om = ObjetManager.Instance;
    }

    private void Start()
    {
        for (int a = 0; a <_nbObjets; a++)
        {
            GameObject twippie = Instantiate(_twippie, Random.insideUnitSphere.normalized * 10, Quaternion.identity, transform);
            _om.allObjects.Add(twippie.GetComponent<ManageableObjet>());
        }
    }
}
