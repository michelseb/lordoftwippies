using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetManager : MonoBehaviour
{
    public List<ManageableObjet> AllObjects { get; private set; } = new List<ManageableObjet>();
    private static ObjetManager _instance;

    public List<ObjectStatManager> StatManagers { get; internal set; } = new List<ObjectStatManager>();
    private Planet _mainPlanet;
    public Planet MainPlanet
    {
        get { if (_mainPlanet == null) _mainPlanet = FindObjectOfType<Planet>(); return _mainPlanet; }
    }

    public static ObjetManager Instance { get { if (_instance == null) _instance = FindObjectOfType<ObjetManager>(); return _instance; } }

    public List<T> GetAll<T>() where T : ManageableObjet
    {
        return AllObjects.FindAll(o => o is T) as List<T>;
    }

    //public void OnGUI()
    //{
    //    int nbTwippies = GetAll<Twippie>().Count;
    //    GUI.Label(new Rect(Screen.width / 2 - 50, 10, 100, 20), "Twippies : " + nbTwippies.ToString());
    //}

    public void AddObject(ManageableObjet obj)
    {
        if (AllObjects.Contains(obj))
            return;

        AllObjects.Add(obj);
        //Debug.Log(obj.name + " ajouté à la liste globale");
    }

    public void RemoveObject(ManageableObjet obj)
    {
        AllObjects.Remove(obj);
        //StatPanel statPanel = _og.MainPanel.StatPanels.FirstOrDefault(x => x.Type == obj.Type);
        //_og.MainPanel.UpdateGlobalStat(statPanel, -1);
    }

    public IEnumerator WaitFor<T>(T obj, Action action)
    {
        Debug.Log("en attente de " + typeof(T).ToString());
        yield return new WaitUntil(() => obj != null);
        Debug.Log(obj.GetType().ToString() + " est arrivé !");
        action();
    }
}
