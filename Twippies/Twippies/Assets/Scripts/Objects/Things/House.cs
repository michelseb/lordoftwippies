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
    }

    public IEnumerator Build(int sizeFactor)
    {
        _currentSize = Vector3.zero;
        while (_currentSize.x < sizeFactor)
        {
            _currentSize = UpdateVector(_currentSize);
            yield return null;
        }
    }
}
