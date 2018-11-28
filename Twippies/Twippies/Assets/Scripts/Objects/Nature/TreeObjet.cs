using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeObjet : StaticObjet {

    protected override void GenerateStats()
    {
        _stats.StatsList = new Stat[3];
        _stats.StatsList[0] = new LabelStat("Pomo (Plante verte)");
        _stats.StatsList[1] = new TextStat("Plante verte", 20);
        _stats.StatsList[2] = new ValueStat(4, 2, 20, "height", false);
    }

    protected override void Start()
    {
        base.Start();
        _outline.color = 1;
        _woodCost = 5;
    }

    protected override void Update()
    {
        base.Update();
        ValueStat scale = (ValueStat)_stats.StatsList[2];
        transform.localScale = new Vector3(scale.Value, scale.Value, scale.Value);
    }

}
