using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TwippieManager : MonoBehaviour, IManager<Twippie> {

    private static TwippieManager _instance;
    public static TwippieManager Instance { get { if (_instance == null) _instance = FindObjectOfType<TwippieManager>(); return _instance; } }

    private Dictionary<Guid, Twippie> _twippies = new Dictionary<Guid, Twippie>();

    public void Add(Twippie twippie)
    {
        if (_twippies.ContainsKey(twippie.Id))
            return;

        _twippies.Add(twippie.Id, twippie);
    }

    public Twippie FindById(Guid id)
    {
        if (!_twippies.ContainsKey(id))
            return null;

        return _twippies[id];
    }

    public bool TryGetById(Guid id, out Twippie twippie)
    {
        twippie = null;

        if (!_twippies.ContainsKey(id))
            return false;

        twippie = _twippies[id];
        return true;
    }

    public IList<Twippie> FindAll()
    {
        return _twippies.Values.ToArray(); 
    }

    public void RefreshAllPaths()
    {
        var twippies = FindAll();

        foreach (var twippie in twippies)
        {
            twippie.RefreshPath();
        }
    }
}
