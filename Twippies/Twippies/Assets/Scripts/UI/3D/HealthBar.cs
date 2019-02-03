using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    [SerializeField]
    private Twippie _twippie;
    private Transform _camPos;
    [SerializeField]
    private Image _health;

    private void Awake()
    {
        _camPos = Camera.main.transform;
    }

    private void Update()
    {
        if (_twippie.Health > 50)
        {
            _health.material.color = Color.green;
        }else if (_twippie.Health <= 50 && _twippie.Health > 20)
        {
            _health.material.color = new Color(1, 1, 0);
        }
        else
        {
            _health.material.color = Color.red;
        }
    }

    private void LateUpdate()
    {
        transform.LookAt(_camPos);
        _health.rectTransform.sizeDelta = new Vector2(_twippie.Health, _health.rectTransform.sizeDelta.y);
        _health.rectTransform.localPosition = new Vector3(50-_twippie.Health/ 2, 0, 0);

        //Vector3 relative = transform.InverseTransformPoint(_camPos.position);
        //float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

}
