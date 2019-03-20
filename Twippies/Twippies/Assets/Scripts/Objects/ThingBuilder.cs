using System.Collections.Generic;
using UnityEngine;

public class ThingBuilder : MonoBehaviour {

    [SerializeField]
    private GameObject[] _constructables;
    private static ThingBuilder _instance;

    public GameObject[] Constructables { get { return _constructables; } }
    public static ThingBuilder Instance { get { if (_instance == null) _instance = FindObjectOfType<ThingBuilder>(); return _instance; } }
}
