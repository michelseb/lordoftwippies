using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour {

    [SerializeField]
    private int _id;
    //private MeshRenderer _renderer;
    private ObjetManager _om;
    private DisplayMode _previousDisplay;

    public Color Col { get; private set; }
    public List<int> VerticeIds { get; set; }
    public bool Accessible { get; set; }
    public bool Taken { get; set; }
    public Vector3 Center { get; set; }
    public float MinHeight { get; private set; }
    public float MaxHeight { get; private set; }
    public float MeanHeight { get; set; }
    public float DeltaHeight { get; set; }
    public int CenterId { get; set; }
    //public GameObject ZoneObject { get { return _zoneObject; } set { _zoneObject = value; } }
    public DisplayMode Display { get; set; }
    public ZoneManager ZManager { get; set; }
    public List<Zone> Neighbours { get; set; }
    public List<PathCost> PathCosts { get; set; }
    public int Id { get { return _id; } set { _id = value; } }
    public List<Ressource> Ressources { get; set; }
    public List<Twippie> Twippies { get; private set; }
    public Color Color { get { return Col; } set { Col = value; } }

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

    private void Awake()
    {
        VerticeIds = new List<int>();
        Neighbours = new List<Zone>();
        _om = ObjetManager.Instance;
        PathCosts = new List<PathCost>();
        Ressources = new List<Ressource>();
        Twippies = new List<Twippie>();
        Col = new Color(Random.value, Random.value, Random.value);
    }

    private void Update()
    {

        foreach (Zone z in Neighbours)
        {
            Debug.DrawLine(Center, z.Center);
        }

        Center = ZManager.transform.TransformPoint(ZManager.Vertices[CenterId]);
        //if (_collider == null)
        //{
        //    if (_zoneObject != null) {
        //        _collider = _zoneObject.GetComponent<MeshCollider>();
        //        _renderer = _zoneObject.GetComponent<MeshRenderer>();
        //    }
        //}

        //if (!_renderer.enabled)
        //{
        //    _renderer.enabled = true;
        //}
        switch (Display)
        {
            case DisplayMode.None:
                Col = SetColor(1, 1, 1);
                break;
            case DisplayMode.Population:

                Col = SetColor(Twippies.Count * 50 / _om.allObjects.Count, .3f, .3f + (Twippies.Count * 100) / _om.allObjects.Count);
                //_renderer.material.color = new Color(Mathf.Lerp(_renderer.material.color.r, (Twippies.Count * 50) / _om.allObjects.Count, Time.deltaTime * 2), 
                //                                    .3f, 
                //                                    Mathf.Lerp(_renderer.material.color.b, .3f + (Twippies.Count * 100) / _om.allObjects.Count, Time.deltaTime * 2), 
                //                                    .4f);

                break;


            case DisplayMode.Height:
                Col = SetColor(1, ((MinHeight + MaxHeight) / 2 - 4) / 2, 0);
                //_renderer.material.color = new Color(1, ((MinHeight+MaxHeight)/2 - 4) / 2, 0, .4f);
                break;


            case DisplayMode.Needs:
                break;


            case DisplayMode.Groups:
                break;

            case DisplayMode.Accessible:

                if (Accessible)
                {
                    Col = SetColor(0, 1, 1);
                    //_renderer.material.color = new Color(0, 1, 1, .4f);
                }
                else
                {
                    Col = SetColor(1, 0, 0);
                    //_renderer.material.color = new Color(1, 0, 0, .4f);
                }

                break;

            case DisplayMode.Water:
                if (Ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Drink))
                {
                    Col = SetColor(0, .8f, 1);
                    //_renderer.material.color = new Color(0, .8f, 1, .6f);
                }
                else
                {
                    Col = SetColor(1, 1, 1);
                    //_renderer.material.color = new Color(0, 0, 0, .3f);
                }
                break;

            case DisplayMode.Food:
                if (Ressources.Exists(x => x.ressourceType == Ressource.RessourceType.Food))
                {
                    Col = SetColor(0, 1, 0);
                    //_renderer.material.color = new Color(0, 1, 0, .6f);
                }
                else
                {
                    Col = SetColor(1, 1, 1);
                    //_renderer.material.color = new Color(0, 0, 0, .3f);
                }
                break;
               
        }

            
        foreach (int vertexId in VerticeIds)
        {
            ZManager.Colors[vertexId] = Col;
        }
        ZManager.SetColors();
        _previousDisplay = Display;
    }

    public bool CheckRessourceInNeighbours(Ressource.RessourceType ressourceType)
    {
        if (Ressources.Exists(x => x.ressourceType == ressourceType))
            return true;

        foreach (Zone neighbour in Neighbours)
        {
            if (neighbour.Ressources.Exists(x => x.ressourceType == ressourceType))
                return true;
        }

        return false;
    }

    public void SetMinHeight(Vector3 center)
    {
        float height = float.PositiveInfinity;
        foreach (int v in VerticeIds)
        {
            float distance = Vector3.Distance(ZManager.Vertices[v], center);
            if (distance < height)
            {
                height = distance;
            }
        }
        MinHeight = height;
    }


    public void SetMaxHeight(Vector3 center)
    {
        float height = 0;
        foreach (int v in VerticeIds)
        {
            float distance = Vector3.Distance(ZManager.Vertices[v], center);
            if (distance > height)
            {
                height = distance;
            }
        }
        MaxHeight = height;
    }

    private Color SetColor(float r, float g, float b)
    {
        return new Color(Mathf.Lerp(Col.r, r, Time.deltaTime * 3),
            Mathf.Lerp(Col.g, g, Time.deltaTime * 3),
            Mathf.Lerp(Col.b, b, Time.deltaTime * 3));
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Twippie")
    //    {
    //        Twippie twippie = other.transform.parent.GetComponent<Twippie>();
    //        if (twippie != null)
    //        {
    //            if (!Twippies.Contains(twippie))
    //            {
    //                Twippies.Add(twippie);
    //            }
    //        }
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Twippie")
    //    {
    //        Twippie twippie = other.transform.parent.GetComponent<Twippie>();
    //        if (twippie != null)
    //        {
    //            if (Twippies.Contains(twippie))
    //            {
    //                Twippies.Remove(twippie);
    //            }
    //        }
    //    }
    //}
}
