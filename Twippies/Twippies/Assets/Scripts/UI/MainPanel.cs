using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : GraphicElement {

    public List<StatPanel> StatPanels;
    private static MainPanel _instance;
    public static MainPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainPanel>();
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        StatPanels = new List<StatPanel>();
    }

}
