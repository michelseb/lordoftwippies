using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Step
{
    private GameObject _go;
    private Zone _zone;
    public Step(GameObject go, Zone zone)
    {
        _go = go;
        _zone = zone;
    }

    public Step(Zone zone)
    {
        _go = null;
        _zone = zone;
    }

    public GameObject Go
    {
        get
        {
            return _go;
        }
    }

    public Zone Zone
    {
        get
        {
            return _zone;
        }
    }

}

public struct PathCost
{
    private Twippie _twippie;
    private int _gCost, _hCost, _fCost;
    private Zone _parent;
    public PathCost(Twippie twippie, int gCost, int hCost)
    {
        _twippie = twippie;
        _gCost = gCost;
        _hCost = hCost;
        _fCost = _gCost + _hCost;
        _parent = null;
    }
    public PathCost(Twippie twippie, int gCost, int hCost, Zone parent)
    {
        _twippie = twippie;
        _gCost = gCost;
        _hCost = hCost;
        _fCost = _gCost + _hCost;
        _parent = parent;
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
        set
        {
            _fCost = value;
        }
    }

    public int HCost
    {
        get
        {
            return _hCost;
        }
    }

    public int GCost
    {
        get
        {
            return _gCost;
        }
        set
        {
            _gCost = value;
        }
    }

    public Zone Parent
    {
        get
        {
            return _parent;
        }
    }
}

public class PathFinder : MonoBehaviour {

    private Twippie _twippie;
    private List<Zone> _openList, _closeList;
    private Arrival _destination;
    private List<Step> _steps;
    private void Awake () {
        _twippie = gameObject.GetComponent<Twippie>();
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

    public List<Step> Steps
    {
        get
        {
            return _steps;
        }
    }

    public void FindPath()
    {
        _openList = new List<Zone>();
        _closeList = new List<Zone>();
        Zone currentZone = _twippie.Zone; // Set zone de début = zone du twippie
        _openList.Add(currentZone);
        currentZone.PathCosts.Add(new PathCost(_twippie, 0, (int)Vector3.Distance(_destination.FinishZone.Center, currentZone.Center)));
        
        while (true) //Tant que la zone trouvée est trop loin de la zone finale
        {
            float smallestFCost = float.PositiveInfinity;
            foreach (Zone z in _openList)
            {
                for (int b = 0; b < currentZone.PathCosts.Count; b++)
                {
                    if (currentZone.PathCosts[b].Twippie != _twippie)
                    {
                        continue;
                    }
                    for (int c = 0; c < z.PathCosts.Count; c++)
                    {
                        if (z.PathCosts[c].Twippie != _twippie)
                        {
                            continue;
                        }

                        if (z.PathCosts[c].FCost == smallestFCost)
                        {
                            if (z.PathCosts[c].HCost < currentZone.PathCosts[b].HCost)
                            {
                                currentZone = z;
                            }
                        }
                        if (z.PathCosts[c].FCost < smallestFCost)
                        {
                            smallestFCost = z.PathCosts[c].FCost;
                            currentZone = z;
                        }
                    }
                }
            }
            _openList.Remove(currentZone);
            _closeList.Add(currentZone);
            if (currentZone == _destination.FinishZone)
            {
                Debug.Log("Chemin trouvé");
                break;
            }
            int possibilities = 0;
            for (int a = 0; a < currentZone.Neighbours.Count; a++) //Toutes les zones à proximité de la zone actuelle
            {
                if (_closeList.Contains(currentZone.Neighbours[a]) || !currentZone.Neighbours[a].Accessible)
                {
                    if (a == currentZone.Neighbours.Count - 1 && possibilities == 0)
                    {
                        List<Zone> result = SetPath(currentZone);
                        DisplaySteps(result);
                        ClearPath();
                        return;
                    }
                    continue;
                }
                possibilities++;
                if (_openList.Contains(currentZone.Neighbours[a]))
                {
                    for (int b = 0; b < currentZone.Neighbours[a].PathCosts.Count; b++)
                    {
                        if (currentZone.Neighbours[a].PathCosts[b].Twippie != _twippie)
                        {
                            continue;
                        }
                        for (int c = 0; c < currentZone.PathCosts.Count; c++)
                        {
                            if (currentZone.PathCosts[c].Twippie != _twippie)
                            {
                                continue;
                            }
                            int gCost = currentZone.PathCosts[c].GCost + (int)Vector3.Distance(currentZone.Center, currentZone.Neighbours[a].Center);
                            int hCost = currentZone.Neighbours[a].PathCosts[b].HCost;

                            if ((gCost + hCost) < currentZone.Neighbours[a].PathCosts[b].FCost)
                            {
                                PathCost pathCost = new PathCost(_twippie, gCost, hCost, currentZone);
                                currentZone.Neighbours[a].PathCosts[b] = pathCost;
                            }
                        }
                    }
                }
                else
                {
                    for (int b = 0; b < currentZone.PathCosts.Count; b++)
                    {
                        if (currentZone.PathCosts[b].Twippie != _twippie)
                        {
                            continue;
                        }
                        int gCost = currentZone.PathCosts[b].GCost + (int)Vector3.Distance(currentZone.Center, currentZone.Neighbours[a].Center);
                        int hCost = (int)Vector3.Distance(_destination.FinishZone.Center, currentZone.Neighbours[a].Center);
                        PathCost pathCost = new PathCost(_twippie, gCost, hCost, currentZone);
                        currentZone.Neighbours[a].PathCosts.Add(pathCost);
                        _openList.Add(currentZone.Neighbours[a]);
                    }
                }
            }
            
        }

        List<Zone> res = SetPath(currentZone);
        DisplaySteps(res);
        ClearPath();
    }
    private void ClearPath()
    {
        foreach (Zone z in _openList)
        {
            for (int b = 0; b < z.PathCosts.Count; b++)
            {
                if (z.PathCosts[b].Twippie != _twippie)
                {
                    continue;
                }
                z.PathCosts.RemoveAt(b);
            }
        }

        foreach (Zone z in _closeList)
        {
            for (int b = 0; b < z.PathCosts.Count; b++)
            {
                if (z.PathCosts[b].Twippie != _twippie)
                {
                    continue;
                }
                z.PathCosts.RemoveAt(b);
            }
        }
    }
    private List<Zone> SetPath(Zone currentZone)
    {
        List<Zone> result = new List<Zone>();
        while (true)
        {
            Zone parent = null;
            for (int b = 0; b < currentZone.PathCosts.Count; b++)
            {
                if (currentZone.PathCosts[b].Twippie != _twippie)
                {
                    continue;
                }
                result.Add(currentZone);
                parent = currentZone.PathCosts[b].Parent;
            }
            if (parent != null)
            {
                currentZone = parent;
            }
            else
            {
                break;
            }
        }
        return result;
    }

    private void DisplaySteps(List<Zone> result)
    {
        Color col = new Color(Random.value, Random.value, Random.value);
        _steps = new List<Step>();
        foreach (Zone z in result)
        {
            //GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //go.GetComponent<MeshRenderer>().material.color = col;
            //go.transform.position = z.Center;
            //go.GetComponent<SphereCollider>().isTrigger = true;
            //go.transform.parent = _twippie.P.transform;
            Step step = new Step(z);
            _steps.Add(step);
        }
    }

}
