using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Step
{
    public GameObject Go { get; }
    public Zone Zone { get; }

    public Step(Zone zone, GameObject go = null)
    {
        Go = go;
        Zone = zone;
    }
}

public struct PathCost
{
    public Twippie Twippie { get; }
    public int FCost { get; set; }
    public int HCost { get; }
    public int GCost { get; set; }
    public Zone Parent { get; }

    public PathCost(Twippie twippie, int gCost, int hCost)
    {
        Twippie = twippie;
        GCost = gCost;
        HCost = hCost;
        FCost = GCost + HCost;
        Parent = null;
    }
    public PathCost(Twippie twippie, int gCost, int hCost, Zone parent)
    {
        Twippie = twippie;
        GCost = gCost;
        HCost = hCost;
        FCost = GCost + HCost;
        Parent = parent;
    }
}

public class PathFinder : MonoBehaviour {

    [SerializeField]
    private bool _displayPath;
    private Twippie _twippie;
    private List<Zone> _openList, _closeList;
    public List<Step> Steps { get; private set; }

    private void Awake () {
        _twippie = gameObject.GetComponent<Twippie>();
	}

    public Zone FindPath(Zone destination)
    {
        _openList = new List<Zone>();
        _closeList = new List<Zone>();
        Zone currentZone = _twippie.Zone; // Set zone de début = zone du twippie
        _openList.Add(currentZone);
        currentZone.PathCosts.Add(new PathCost(_twippie, 0, (int)Vector3.Distance(destination.Center, currentZone.Center)));
        
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
            if (currentZone == destination)
            {
                break;
            }
            int possibilities = 0;
            for (int a = 0; a < currentZone.Neighbours.Count; a++) //Toutes les zones à proximité de la zone actuelle
            {
                if (_closeList.Contains(currentZone.Neighbours[a]) || !currentZone.Neighbours[a].Accessible || currentZone.Neighbours[a].Taken)
                {
                    if (a == currentZone.Neighbours.Count - 1 && possibilities == 0)
                    {
                        ClearPath();
                        return null;
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
                        int hCost = (int)Vector3.Distance(destination.Center, currentZone.Neighbours[a].Center);
                        PathCost pathCost = new PathCost(_twippie, gCost, hCost, currentZone);
                        currentZone.Neighbours[a].PathCosts.Add(pathCost);
                        _openList.Add(currentZone.Neighbours[a]);
                    }
                }
            }
            
        }
        return currentZone;
    }

    public void CreatePath(Zone zone)
    {
        List<Zone> res = SetPath(zone);
        DisplaySteps(res, _displayPath);
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

    private void DisplaySteps(List<Zone> result, bool obj)
    {
        Color col = new Color(Random.value, Random.value, Random.value);
        Steps = new List<Step>();
        Step step;
        foreach (Zone z in result)
        {
            if (obj)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.transform.localScale = Vector3.one * .3f;
                go.GetComponent<MeshRenderer>().material.color = col;
                go.transform.position = z.Center;
                go.GetComponent<SphereCollider>().isTrigger = true;
                go.transform.parent = _twippie.P.transform;
                step = new Step(z, go);
            }
            else
            {
                step = new Step(z);
            }
            Steps.Add(step);
        }
    }

}
