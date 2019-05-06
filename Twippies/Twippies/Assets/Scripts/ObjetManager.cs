using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjetManager : MonoBehaviour {
    public List<ManageableObjet> allObjects;
    private ObjectGenerator _og;
    private static ObjetManager _instance;

    public List<StatManager> StatManagers { get; internal set; }
    public Planete ActivePlanet { get; set; }
    public static ObjetManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ObjetManager>(); return _instance; } }

    private void Awake()
    {
        _og = ObjectGenerator.Instance;
        ActivePlanet = FindObjectOfType<Planete>();
        StatManagers = new List<StatManager>();
    }

    public List<ManageableObjet> AllObjects<T>()
    {
        List<ManageableObjet> result = new List<ManageableObjet>();
        result = allObjects.FindAll(o => o is T);
        return result;
    }

    public void OnGUI()
    {
        int nbTwippies = AllObjects<Twippie>().Count;
        GUI.Label(new Rect(Screen.width/2 - 50, 10, 100, 20), "Twippies : "+nbTwippies.ToString());
    }

    public void UpdateObjectList(ManageableObjet obj, bool add)
    {
        if (add)
        {
            allObjects.Add(obj);
            Debug.Log(obj.name + " ajouté à la liste globale");
        }
        else
        {
            allObjects.Remove(obj);
            //StatPanel statPanel = _og.MainPanel.StatPanels.FirstOrDefault(x => x.Type == obj.Type);
            //_og.MainPanel.UpdateGlobalStat(statPanel, -1);
        }
    }

    public IEnumerator WaitFor<T>(T obj, Action action)
    {
        Debug.Log("en attente de " + typeof(T).ToString());
        yield return new WaitUntil(() => obj != null);
        Debug.Log(obj.GetType().ToString() + " est arrivé !");
        action();
    }
}
