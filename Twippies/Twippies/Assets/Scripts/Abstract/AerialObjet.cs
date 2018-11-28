using UnityEngine;

public abstract class AerialObjet : DraggableObjet {


    protected override void FixedUpdate()
    {
        if (_p)
        {
            if (!Input.GetMouseButton(1))
            {
                _p.Face(transform);
            }
        }
    }

}
