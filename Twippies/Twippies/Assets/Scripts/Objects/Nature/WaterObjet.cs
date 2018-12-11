using UnityEngine;

public class WaterObjet : ManageableObjet {


    private float _radius, _previousRadius;
    [SerializeField]
    private Planete _planet;
    

    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[4];
        _stats.StatsList[0] = new LabelStat("Glougl (Océan)");
        _stats.StatsList[1] = new TextStat("Eau potable", 20);
        _stats.StatsList[2] = new BoolStat(true, "Hard/Soft");
        _stats.StatsList[3] = new ValueStat(10, 0, 20, "niveau", false);
    }

    protected override void Start()
    {
        base.Start();
        _outline.color = 0;
        _outline.eraseRenderer = true;
        SetRadius();
        _previousRadius = _radius;
        _displayIntervals = 10;
    }

    protected override void Update()
    {
        base.Update();
        SetRadius();
        if (Time.frameCount % _displayIntervals == 0 && !Input.GetMouseButton(0))
        {
            if (Mathf.Abs(_previousRadius - _radius) > .1f)
            {
                _planet.ZManager.SetTriangles();
                _planet.ZManager.GenerateZoneObjects();
                _previousRadius = _radius;
            }
        }
    }

    public float Radius
    {
        get
        {
            return _radius;
        }
    }

    private void SetRadius()
    {
        float scale = _stats.StatToValue(_stats.StatsList[3]).Value;
        transform.localScale = new Vector3(scale, scale, scale);
        SphereCollider s = (SphereCollider)_coll;
        _radius = s.radius * transform.lossyScale.magnitude;
        Debug.Log(_radius + " " + _previousRadius);
    }

}
