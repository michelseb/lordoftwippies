﻿using UnityEngine;

public abstract class AerialObjet : DraggableObjet {


    protected override void MakeAttraction()
    {
        if (_p)
        {
            if (!Input.GetMouseButton(1))
            {
                _p.Face(transform);
            }
        }
    }

    public override void GetZone(bool take)
    {
        return;
    }

}
