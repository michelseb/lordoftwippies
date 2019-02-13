using UnityEngine;

public class House : StaticObjet
{
    protected override void Awake()
    {
        base.Awake();
        _type = "Talho (Maison)";
        _name = "Maison twippie";
    }
    protected override void Start()
    {
        base.Start();
        _outline.color = 1;
        _woodCost = 50;
    }
}
