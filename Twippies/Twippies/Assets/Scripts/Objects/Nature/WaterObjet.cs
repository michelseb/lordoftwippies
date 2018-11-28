using UnityEngine;

public class WaterObjet : ManageableObjet {


    private float _radius;

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
    }

    protected override void Update()
    {
        base.Update();
        float scale = _stats.StatToValue(_stats.StatsList[3]).Value;
        transform.localScale = new Vector3(scale, scale, scale);
        SphereCollider s = (SphereCollider)_coll;
        _radius = s.radius * transform.lossyScale.magnitude;
    }

    public float Radius
    {
        get
        {
            return _radius;
        }
    }

}
