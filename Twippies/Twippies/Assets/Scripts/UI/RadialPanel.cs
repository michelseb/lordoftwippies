using UnityEngine;

public class RadialPanel : GraphicElement {

    public Animator Animator { get; set; }
    private static RadialPanel _instance;
    public static RadialPanel Instance { get { if (_instance == null) _instance = FindObjectOfType<RadialPanel>(); return _instance; } }
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

}
