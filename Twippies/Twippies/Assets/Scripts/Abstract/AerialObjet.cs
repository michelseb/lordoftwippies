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

    public override Zone GetZone(bool take, Zone[] zones = null, bool checkTaken = false, bool shouldAccess = true)
    {
        return null;
    }

}
