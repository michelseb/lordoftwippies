using UnityEngine;

public abstract class AerialObjet : DraggableObjet {


    protected override void MakeAttraction()
    {
        if (_planet)
        {
            if (!Input.GetMouseButton(1))
            {
                _planet.Face(transform);
            }
        }
    }

}
