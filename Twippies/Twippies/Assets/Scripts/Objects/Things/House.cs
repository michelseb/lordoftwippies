using System.Collections;
using UnityEngine;

public class House : StaticObjet, IBuildable
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

    public IEnumerator Build()
    {
        _currentSize = Vector3.zero;
        while (_currentSize.x < 1)
        {
            _currentSize = UpdateVector(_currentSize);
            yield return null;
        }
    }
}
