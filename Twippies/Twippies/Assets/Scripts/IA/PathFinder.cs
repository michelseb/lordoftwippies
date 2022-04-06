using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//public class Step
//{
//    public Guid ZoneId { get; }
//    private Transform _zoneTransform;
//    public GameObject GameObject { get; set; }
//    public Vector3 WorldPos => _zoneTransform.position;

//    public Step(Guid zoneId, GameObject gameObject = null)
//    {
//        ZoneId = zoneId;
//        GameObject = gameObject;
//        _zoneTransform = ZoneManager.Instance.FindById(zoneId).transform;
//    }
//}

public struct PathCost
{
    public float FCost { get; set; }
    public float HCost { get; }
    public float GCost { get; set; }
    public Guid Parent { get; }

    public PathCost(float gCost, float hCost, Guid parent)
    {
        GCost = gCost;
        HCost = hCost;
        FCost = GCost + HCost;
        Parent = parent;
    }
}

public class PathFinder : MonoBehaviour
{

    [SerializeField] private bool _displayPath;

    private Guid _twippieId;
    public List<GameObject> Path { get; private set; }
    private IEnumerator _findPath;
    private ZoneManager _zoneManager;
    private Guid _destination;
    private GameObject _destinationVisual;

    public bool HasDestination => _destination != default;

    private void Awake()
    {
        _twippieId = GetComponent<Twippie>().Id;
        _zoneManager = ZoneManager.Instance;

        // Destination 
        _destinationVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _destinationVisual.GetComponent<MeshRenderer>().material.color = Color.red;
        _destinationVisual.GetComponent<Collider>().isTrigger = true;
        _destinationVisual.transform.localScale = Vector3.one * .2f;
        _destinationVisual.transform.SetParent(TwippieManager.Instance.FindById(_twippieId).Planet.transform);
    }

    public IEnumerator FindPath(Guid currentZoneId, Guid destinationId, Action<Zone> result)
    {
        if (_findPath != null)
        {
            StopCoroutine(_findPath);
        }

        _destination = destinationId;
        _destinationVisual.transform.position = _zoneManager.FindById(destinationId).WorldPos;

        _findPath = DoFindPath(currentZoneId, destinationId, zone =>
        {
            result(zone);
        });

        yield return StartCoroutine(_findPath);
    }

    public IEnumerator RefreshPath(Guid currentZoneId, Action<Zone> result)
    {
        if (!HasDestination)
            yield break;

        if (_findPath != null)
        {
            StopCoroutine(_findPath);
        }

        _findPath = DoFindPath(currentZoneId, _destination, zone =>
        {
            result(zone);
        });

        yield return StartCoroutine(_findPath);
    }

    private IEnumerator DoFindPath(Guid currentZoneId, Guid destinationId, Action<Zone> result)
    {
        var openList = new List<Guid> { currentZoneId };
        var closeList = new List<Guid>();

        var currentZone = _zoneManager.FindById(currentZoneId);
        var destination = _zoneManager.FindById(destinationId);

        UpdateZonePathCost(currentZone, 0, Vector3.Distance(destination.WorldPos, currentZone.WorldPos), Guid.Empty);

        while (openList.Count > 0)
        {
            currentZone = openList
                .Select(x => _zoneManager.FindById(x))
                .OrderBy(z => z.PathCosts[_twippieId].FCost)
                .ThenBy(z => z.PathCosts[_twippieId].HCost)
                .FirstOrDefault();

            currentZoneId = currentZone.Id;

            if (currentZoneId == destinationId)
            {
                CreatePath(currentZoneId);
                ClearPath(openList, _twippieId);
                ClearPath(closeList, _twippieId);
                result(currentZone);
                yield break;
            }

            openList.Remove(currentZoneId);
            closeList.Add(currentZoneId);

            var neighbourIds = currentZone.NeighbourIds
                .Where(n => !closeList.Contains(n))
                .Select(x => _zoneManager.FindById(x))
                .Where(z => z.Accessible && !z.Taken)
                .ToList();

            if (neighbourIds == null || neighbourIds.Count == 0)
            {
                continue;
            }

            var currentGCost = currentZone.PathCosts[_twippieId].GCost;

            foreach (var neighbour in neighbourIds)//Toutes les zones à proximité de la zone actuelle
            {
                var gCost = currentGCost + Vector3.Distance(currentZone.WorldPos, neighbour.WorldPos);

                if (openList.Contains(neighbour.Id))
                {
                    var neighbourPathCost = neighbour.PathCosts[_twippieId];

                    var hCost = neighbourPathCost.HCost;

                    if ((gCost + hCost) < neighbourPathCost.FCost)
                    {
                        UpdateZonePathCost(neighbour, gCost, hCost, currentZoneId);
                    }

                }
                else
                {
                    var hCost = Vector3.Distance(destination.WorldPos, neighbour.WorldPos);
                    UpdateZonePathCost(neighbour, gCost, hCost, currentZoneId);
                    openList.Add(neighbour.Id);
                }
            }

            //yield return null;
        }

        // Reached if path could not be found
        CreatePath(currentZoneId);
        ClearPath(openList, _twippieId);
        ClearPath(closeList, _twippieId);
        result(currentZone);
    }

    public void UpdateZonePathCost(Zone zone, float gCost, float hCost, Guid parent)
    {
        if (zone.PathCosts.ContainsKey(_twippieId))
        {
            zone.PathCosts[_twippieId] = new PathCost(gCost, hCost, parent);
        }
        else
        {
            zone.PathCosts.Add(_twippieId, new PathCost(gCost, hCost, parent));
        }
    }

    public void CreatePath(Guid zoneId)
    {
        var path = SetPath(zoneId);
        SetSteps(path);
    }

    private void ClearPath(List<Guid> path, Guid id)
    {
        foreach (var zone in path.Select(z => _zoneManager.FindById(z)).ToArray())
        {
            zone.PathCosts.Remove(id);
        }
    }

    private List<Guid> SetPath(Guid currentZoneId)
    {
        var result = new List<Guid>();

        while (true)
        {
            var currentZone = _zoneManager.FindById(currentZoneId);

            result.Insert(0, currentZoneId);

            if (!currentZone.PathCosts.ContainsKey(_twippieId))
                break;

            var parent = currentZone.PathCosts[_twippieId].Parent;

            if (parent == default)
                break;

            currentZoneId = parent;
        }

        return result;
    }

    private void SetSteps(List<Guid> zoneIds)
    {
        var planet = TwippieManager.Instance.FindById(_twippieId).Planet.transform;
        Path = zoneIds.Select(z =>
        {
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.GetComponent<Collider>().isTrigger = true;
            obj.transform.localScale = Vector3.one * .05f;
            obj.transform.position = _zoneManager.FindById(z).WorldPos;
            obj.transform.SetParent(planet);

            return obj;
        })
        .ToList();
    }

}
