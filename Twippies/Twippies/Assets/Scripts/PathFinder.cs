using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PathCost
{
    private Twippie _twippie;
    private int _gCost, _hCost, _fCost;
    public PathCost(Twippie twippie, int gCost, int hCost)
    {
        _twippie = twippie;
        _gCost = gCost;
        _hCost = hCost;
        _fCost = _gCost + _hCost;
    }

    public Twippie Twippie
    {
        get
        {
            return _twippie;
        }
    }

    public int FCost
    {
        get
        {
            return _fCost;
        }
    }
}

public class PathFinder : MonoBehaviour {

    private Twippie _twippie;
    private List<Zone> _openList, _closeList;
    private Arrival _destination;

	private void Start () {
        _twippie = gameObject.GetComponent<Twippie>();
        _openList = new List<Zone>();
        _closeList = new List<Zone>();
	}
	

    public Arrival Destination
    {
        get
        {
            return _destination;
        }
        set
        {
            _destination = value;
        }
    }

    public List<Zone> GetTrajet()
    {
        foreach(Zone z in _destination.ZoneManager.Zones)
        {
            int gCost = (int)Vector3.Distance(_twippie.Zone.Center, z.Center);
            int hCost = (int)Vector3.Distance(_destination.FinishZone.Center, z.Center);
            PathCost pathCost = new PathCost(_twippie, gCost, hCost);
            z.PathCosts.Add(pathCost);
        }
        return FindPath();
    }


    private List<Zone> FindPath()
    {
        Zone currentZone = _twippie.Zone; // Set zone de début = zone du twippie
        while (Vector3.Distance(currentZone.Center, _destination.FinishZone.Center) > 2) //Tant que la zone trouvée est trop loin de la zone finale
        {
            int shortestPath = int.MaxValue; 
            Zone nearestNeighbour = null; //Zone suivante la plus proche
            for (int a = 0; a < currentZone.Neighbours.Count; a++) //Toutes les zones à proximité de la zone actuelle
            {
                if (!_closeList.Contains(currentZone.Neighbours[a]) && !_openList.Contains(currentZone.Neighbours[a])) 
                {
                    _openList.Add(currentZone.Neighbours[a]); //Ajout de toutes les zones proches non-connues à la liste des zones à découvrir
                }
                for (int b = 0; b < currentZone.Neighbours[a].PathCosts.Count; b++)
                {
                    if (currentZone.Neighbours[a].PathCosts[b].Twippie == _twippie) // Vérifier qu'il s'agit bien du pathcost lié à ce twippie
                    {
                        if (currentZone.Neighbours[a].Accessible) // Vérifier que la zone est accessible
                        {
                            if (!_closeList.Contains(currentZone.Neighbours[a])) //Vérifier qu'il ne s'agit pas d'une zone déjà visitée
                            {
                                if (currentZone.Neighbours[a].PathCosts[b].FCost < shortestPath) //Recherche de la zone avec le FCost le plus petit
                                {
                                    shortestPath = currentZone.Neighbours[a].PathCosts[b].FCost;
                                    nearestNeighbour = currentZone.Neighbours[a];
                                }
                            }
                        }
                    }
                }
            }
            currentZone = nearestNeighbour; //nouvelle zone = zone avec le meilleur FCost
            _closeList.Add(currentZone); //La zone est prise
            _openList.Remove(currentZone); // La zone ne doit plus être visitée
        }
        return _closeList;
        
    }

}
