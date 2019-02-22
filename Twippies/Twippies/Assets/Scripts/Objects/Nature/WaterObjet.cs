using UnityEngine;

public class WaterObjet : ManageableObjet {


    private float _radius, _previousRadius;
    [SerializeField]
    private Planete _planet;
    

    public override void GenerateStats()
    {
        base.GenerateStats();
        _stats.GenerateStat<BoolStat>().Populate(false, "hard/soft");
        _stats.GenerateStat<ValueStat>().Populate(10, 0, 20, "niveau", false);
    }
    protected override void Awake()
    {
        base.Awake();
        _type = "Glougl (Océan)";
        _name = "Eau potable";
    }

    protected override void Start()
    {
        base.Start();
        _outline.color = 0;
        _outline.eraseRenderer = true;
        SetRadius();
        _previousRadius = _radius;
    }

    protected override void Update()
    {
        base.Update();
        SetRadius();
        if (!Input.GetMouseButton(0))
        {
            if (Mathf.Abs(_previousRadius - _radius) > .1f)
            {
                _planet.ZManager.GetZoneInfo();
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
        SphereCollider s = (SphereCollider)_coll;
        _radius = s.radius * transform.lossyScale.magnitude;
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        float scale = _stats.StatToValue(_stats.StatsList[4]).Value;
        transform.localScale = new Vector3(scale, scale, scale);

    }

}
