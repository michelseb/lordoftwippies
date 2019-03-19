using UnityEngine;

public class WaterObjet : ManageableObjet {


    private float _previousRadius;
    [SerializeField]
    private Planete _planet;
    

    public override void GenerateStats(StatPanel statPanel, string type)
    {
        base.GenerateStats(statPanel, type);
        statPanel.StatManager.GenerateStat<BoolStat>(type).Populate(false, "hard/soft", "Water");
        statPanel.StatManager.GenerateStat<ValueStat>(type).Populate(10, 0, 20, "niveau", false, "Level");
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
        _outline.color = 0;
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
                _planet.ZManager.GetZoneInfo();
                _previousRadius = Radius;
            }
        }
    }

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
