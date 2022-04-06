using UnityEngine;

public class WaterObjet : ManageableObjet {


    [SerializeField] private Planet _planet;

    private float _previousRadius;
    
    public override void GenerateStatsForActions()
    {
        base.GenerateStatsForActions();
        Stats.GenerateWorldStat<BoolStat>().Populate("Water", false, "hard/soft", false);
        Stats.GenerateWorldStat<ValueStat>().Populate("Level", 10, 0, 20, "niveau", false);
    }

    public float Radius { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        _name = "Eau potable";
    }

    protected override void Start()
    {
        base.Start();
        _outline.Color = 0;
        _outline.eraseRenderer = true;
        SetRadius();
        _previousRadius = Radius;
    }

    protected override void Update()
    {
        base.Update();
        SetRadius();
        if (!Input.GetMouseButton(0))
        {
            if (Mathf.Abs(_previousRadius - Radius) > .1f)
            {
                _zoneManager.SetZoneInfos();
                _previousRadius = Radius;
            }
        }
    }

    //protected override Vector3 SetCurrentSize()
    //{
    //    return _initSize;
    //}

    private void SetRadius()
    {
        SphereCollider s = (SphereCollider)_coll;
        Radius = s.radius * transform.lossyScale.magnitude;
    }

    protected override void UpdateStats()
    {
        base.UpdateStats();
        float scale = _stats.StatToValue(_stats.GetStat("Level")).Value;
        transform.localScale = new Vector3(scale, scale, scale);

    }

}
