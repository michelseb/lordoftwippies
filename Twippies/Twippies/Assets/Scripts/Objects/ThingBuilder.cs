using System.Collections.Generic;
using UnityEngine;

public class ThingBuilder : MonoBehaviour {

    [SerializeField]
    private Objet[] _constructables;

    private static ThingBuilder _instance;
    public static ThingBuilder Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ThingBuilder>();

            return _instance;
        }
    }

    public Objet[] Constructables
    {
        get
        {
            return _constructables;
        }
    }

}
