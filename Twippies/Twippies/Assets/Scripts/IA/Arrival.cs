using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrival : MonoBehaviour {

    public Zone FinishZone { get; set; }
    public ZoneManager ZoneManager { get; set; }

    public void SetArrival()
    {
        float distMin = Mathf.Infinity;
        Zone tempZone = null;
        foreach (Zone z in ZoneManager.Zones)
        {
            float dist = (transform.position - z.Center).sqrMagnitude;
            if (dist < distMin)
            {
                distMin = dist;
                tempZone = z;
            }
        }
        FinishZone = tempZone;
    }
}
