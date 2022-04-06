using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetManager : MonoBehaviour, IManager<Planet> {

    private static PlanetManager _instance;
    public static PlanetManager Instance { get { if (_instance == null) _instance = FindObjectOfType<PlanetManager>(); return _instance; } }

    private Dictionary<Guid, Planet> _planetDictionary = new Dictionary<Guid, Planet>();
    public List<Planet> Planets { get; private set; } = new List<Planet>();

    public void Add(Planet planet)
    {
        if (_planetDictionary.ContainsKey(planet.Id))
            return;

        _planetDictionary.Add(planet.Id, planet);
        Planets.Add(planet);
    }

    public Planet FindById(Guid id)
    {
        if (!_planetDictionary.ContainsKey(id))
            return null;

        return _planetDictionary[id];
    }

    public bool TryGetById(Guid id, out Planet planet)
    {
        planet = null;

        if (!_planetDictionary.ContainsKey(id))
            return false;

        planet = _planetDictionary[id];
        return true;
    }

    public IList<Planet> FindAll()
    {
        return _planetDictionary.Values.ToArray();
    }
}
