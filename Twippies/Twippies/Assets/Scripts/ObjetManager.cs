using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjetManager : MonoBehaviour {

    public List<ManageableObjet> allObjects;
    private Planete _activePlanet;

    private static ObjetManager _instance;
    public static ObjetManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ObjetManager>();

            return _instance;
        }
    }

    private void Awake()
    {
        foreach (ManageableObjet m in FindObjectsOfType<ManageableObjet>())
        {
            allObjects.Add(m);
        }
        _activePlanet = FindObjectOfType<Planete>();
    }

    public List<ManageableObjet> AllObjects<T>()
    {
        List<ManageableObjet> result = new List<ManageableObjet>();
        result = allObjects.FindAll(o => o is T);
        return result;
    }

    public Planete ActivePlanet
    {
        get
        {
            return _activePlanet;
        }
        set
        {
            _activePlanet = value;
        }
    }

    public void OnGUI()
    {
        int nbTwippies = AllObjects<Twippie>().Count;
        GUI.Label(new Rect(Screen.width/2 - 50, 10, 100, 20), "Twippies : "+nbTwippies.ToString());
    }
}
