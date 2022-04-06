using UnityEngine;

public class CharacterPosition : MonoBehaviour
{
    private Planet _planet;

    public void Init(Twippie twippie)
    {
        _planet = twippie.Planet;
        transform.position = twippie.transform.position;
        transform.SetParent(_planet.transform);
    }
}
