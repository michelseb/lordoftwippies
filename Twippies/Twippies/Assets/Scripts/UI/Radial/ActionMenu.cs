using System.Collections.Generic;
using UnityEngine;

public class ActionMenu : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RadialButton _buttonPrefab;
    private List<RadialButton> _radialButtons = new List<RadialButton>();
    private float _maxSize;


    private void Awake()
    {
        _maxSize = 50f;
        _rectTransform = GetComponent<RectTransform>();
        GenerateButtons(new List<float> { 1, 1, 1, 1, 1 });
    }

    public void GenerateButtons(List<float> sizes)
    {
        for (int a = 0; a < sizes.Count; a++)
        {
            var button = Instantiate(_buttonPrefab, transform, false);
            float size = sizes[a] / (float)sizes.Count;
            float startAngle = 360f / (float)sizes.Count * a * sizes[a];
            button.Init();
            button.SetActive(true);
            button.UpdateSegment(size, startAngle);
            _radialButtons.Add(button);
        }
    }
}
