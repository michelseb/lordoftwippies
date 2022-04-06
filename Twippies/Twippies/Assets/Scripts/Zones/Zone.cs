using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DisplayMode
{
    None = 0,
    Population = 1,
    Height = 2,
    Accessible = 3,
    Water = 4,
    Food = 5,
    Needs = 6,
    Groups = 7
}

public class Zone : Objet
{
    private ObjetManager _objectManager;
    private ZoneManager _zoneManager;
    private VertexManager _vertexManager;
    private Transform _transform;

    public Color Col { get; private set; }
    public List<Guid> VerticeIds { get; set; }
    public bool Accessible { get; set; }
    public bool Taken { get; set; }
    public float MinHeight { get; private set; }
    public float MaxHeight { get; private set; }
    public float MeanHeight { get; set; }
    public float DeltaHeight { get; set; }
    public float Height { get; set; }
    public DisplayMode Display => _zoneManager.Planet.Display;
    public List<Guid> NeighbourIds { get; set; }
    public Dictionary<Guid, PathCost> PathCosts { get; set; }
    public List<Resource> Resources { get; set; }
    public List<Guid> TwippieIds { get; private set; }
    public Color Color { get { return Col; } set { Col = value; } }
    public Vector3 WorldPos => _transform.position;

    protected override void Awake()
    {
        base.Awake();

        _objectManager = ObjetManager.Instance;
        _zoneManager = ZoneManager.Instance;
        _vertexManager = VertexManager.Instance;
        _transform = transform;

        VerticeIds = new List<Guid>();
        NeighbourIds = new List<Guid>();
        PathCosts = new Dictionary<Guid, PathCost>();
        Resources = new List<Resource>();
        TwippieIds = new List<Guid>();
        Col = UnityEngine.Random.ColorHSV();
    }

    private bool SetColor(Color col)
    {
        var newCol = new Color(Mathf.Lerp(Col.r, col.r, Time.deltaTime * 3),
            Mathf.Lerp(Col.g, col.g, Time.deltaTime * 3),
            Mathf.Lerp(Col.b, col.b, Time.deltaTime * 3));

        if (Col == newCol)
            return false;
        
        Col = newCol;
        
        return true;
    }

    public void SetHeights(Vector3 origin)
    {
        Height = Vector3.Distance(origin, transform.position);

        MinHeight = VerticeIds.Select(v => _vertexManager.FindById(v)).Min(x => Vector3.Distance(x.Position, origin));
        MaxHeight = VerticeIds.Select(v => _vertexManager.FindById(v)).Max(x => Vector3.Distance(x.Position, origin));

        MeanHeight = (MinHeight + MaxHeight) / 2;
        DeltaHeight = MaxHeight - MinHeight;
    }

    public void AddNeighbour(Guid neighbourId)
    {
        if (NeighbourIds.Contains(neighbourId))
            return;

        NeighbourIds.Add(neighbourId);
    }

    public void AddTwippie(Guid twippieId)
    {
        if (TwippieIds.Contains(twippieId))
            return;

        TwippieIds.Add(twippieId);
    }

    public void RemoveTwippie(Guid twippieId)
    {
        if (!TwippieIds.Contains(twippieId))
            return;

        TwippieIds.Remove(twippieId);
    }

    public void SetDisplayMode(DisplayMode mode)
    {
        if (mode == Display)
            return;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Color color;
        switch (Display)
        {
            case DisplayMode.None:
                color = new Color(1, 1, 1);
                break;

            case DisplayMode.Population:
                color = new Color(TwippieIds.Count * 50 / _objectManager.AllObjects.Count, .3f, .3f + (TwippieIds.Count * 100) / _objectManager.AllObjects.Count);
                break;

            case DisplayMode.Height:
                color = new Color(1, (Height / 2 - 4) / 2, 0);
                break;

            case DisplayMode.Needs:
                color = new Color(1, 1, 1);
                break;

            case DisplayMode.Groups:
                color = new Color(1, 1, 1);
                break;

            case DisplayMode.Accessible:
                if (Accessible) { color = new Color(0, 1, 1); }
                else { color = new Color(1, 0, 0); }
                break;

            case DisplayMode.Water:
                if (HasResource(ResourceType.Drink)) { color = new Color(0, .8f, 1); }
                else { color = new Color(1, 1, 1); }
                break;

            case DisplayMode.Food:
                if (HasResource(ResourceType.Food)) { color = new Color(0, 1, 0); }
                else { color = new Color(1, 1, 1); }
                break;
            default:
                color = new Color(1, 1, 1);
                break;
        }

        //if (SetColor(color))
        //{
        //    foreach (var vertexId in VerticeIds)
        //    {
        //        _zoneManager.Colors[vertexId] = Col;
        //    }
        //    _zoneManager.SetColors();
        //}
    }

    public Resource GetResourceByType(ResourceType resourceType)
    {
        return Resources.FirstOrDefault(r => r.ResourceType == resourceType);
    }

    public bool HasResource(ResourceType resourceType)
    {
        return GetResourceByType(resourceType) != null;
    }
}
