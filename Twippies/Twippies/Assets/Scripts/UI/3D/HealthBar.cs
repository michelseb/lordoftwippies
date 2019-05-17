using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Twippie _twippie;
    private Transform _camPos;
    [SerializeField]
    private Image _health;

    public Image Health { get { return _health; } }

    private void Awake()
    {
        _camPos = Camera.main.transform;
    }

    private void Update()
    {
        if (_twippie.Health > 50)
        {
            _health.color = Color.green;
        }else if (_twippie.Health <= 50 && _twippie.Health > 20)
        {
            _health.color = new Color(1, 1, 0);
        }
        else
        {
            _health.color = Color.red;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(_camPos);
        _health.rectTransform.sizeDelta = new Vector2(_twippie.Health, _health.rectTransform.sizeDelta.y);
        _health.rectTransform.localPosition = new Vector3(50-_twippie.Health/ 2, 0, 0);
    }

}
