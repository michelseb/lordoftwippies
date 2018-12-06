using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrival : MonoBehaviour {

    private Zone _finishZone;
    private ZoneManager _zoneManager;


    public void SetArrival()
    {
        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        foreach (Zone z in _zoneManager.Zones)
        {
            float dist = (transform.position - z.Center).sqrMagnitude;
            if (dist < distMin)
            {
                distMin = dist;
                tempZone = z;
            }
        }

        _finishZone = tempZone;
    }

    public Zone FinishZone
    {
        get
        {
            return _finishZone;
        }
        set
        {
            _finishZone = value;
        }
    }

    public ZoneManager ZoneManager
    {
        get
        {
            return _zoneManager;
        }
        set
        {
            _zoneManager = value;
        }
    }

}
