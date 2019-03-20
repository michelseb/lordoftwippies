using UnityEngine;
using UnityEngine.UI;

public class Objet : MonoBehaviour {

    [SerializeField]
    protected Sprite _icon;

    protected Vector3 _dist, _initSize, _currentSize;
    protected ObjetManager _om;
    protected ObjectGenerator _og;
    protected float _posX, _posY;
    [SerializeField]
    protected int WOODCOST, WATERCOST, STONECOST;
    
    protected Camera _cam;

    public int WoodCost { get { return WOODCOST; } }
    public int WaterCost { get { return WATERCOST; } }
    public int StoneCost { get { return STONECOST; } }
    public Sprite Icon { get { return _icon; } }
    public Vector3 CurrentSize { get { return _currentSize; } set { _currentSize = value; } }

    protected virtual void Awake()
    {
        _om = ObjetManager.Instance;
        _og = ObjectGenerator.Instance;
        _cam = Camera.main;
    }

    protected virtual void Start()
    {
        _initSize = transform.lossyScale;
        _currentSize = _initSize;
    }

    protected bool CoinFlip(float chance = .5f)
    {
        float rand = Random.value;
        return (rand < chance);
    }

}
