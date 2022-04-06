using System.Linq;
using UnityEngine;

public class Arrival : MonoBehaviour {

    public Zone FinishZone { get; set; }

    public void SetArrival()
    {
        FinishZone = ZoneManager.Instance
            .Zones
            .OrderBy(zone => (transform.position - zone.WorldPos).sqrMagnitude)
            .FirstOrDefault();
    }
}
