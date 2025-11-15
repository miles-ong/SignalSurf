using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WaveRenderer : MonoBehaviour
{
    [SerializeField] private Sprite pointSprite; // Sprite for wave points
    [SerializeField] private RectTransform canvas; // Reference to Canvas RectTransform
    [SerializeField] private float scrollSpeed = 500f; // Canvas units per second
    [SerializeField] private float spawnInterval = 0.1f;
    [SerializeField] private float pointSize = 20f; // Canvas units
    [SerializeField] private Color pointColor = Color.white;

    private Wave _wave;
    private List<RectTransform> _activePoints = new();
    private float _spawnTimer = 0f;
    private float _canvasWidth;

    void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
        UpdateCanvasWidth();
    }

    void Update()
    {
        if (_wave == null || _wave.Complete)
            return;

        UpdateCanvasWidth();
        _spawnTimer += Time.deltaTime;

        // Spawn new point
        if (_spawnTimer >= spawnInterval)
        {
            float y = _wave.GetNextCoordinate();
            SpawnPoint(y);
            _spawnTimer = 0f;
        }

        // Move and cleanup points
        for (int i = _activePoints.Count - 1; i >= 0; i--)
        {
            RectTransform point = _activePoints[i];
            point.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

            // Remove if off left edge
            if (point.anchoredPosition.x < -1000)
            {
                Destroy(point.gameObject);
                _activePoints.RemoveAt(i);
            }
        }
    }

    private void SpawnPoint(float y)
    {
        // Create GameObject with Image component
        GameObject point = new GameObject("WavePoint");
        point.transform.SetParent(canvas, false);

        // Add and configure Image component
        Image image = point.AddComponent<Image>();
        image.sprite = pointSprite;
        image.color = pointColor;
        image.raycastTarget = false; // Optimize - no need for raycasts

        // Setup RectTransform
        RectTransform rectTransform = point.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(_canvasWidth / 2f, y);
        rectTransform.sizeDelta = Vector2.one * pointSize;

        _activePoints.Add(rectTransform);
    }

    private void UpdateCanvasWidth()
    {
        _canvasWidth = canvas.rect.width;
    }

    public void Initialize(Wave w)
    {
        _wave = w;
        
        // Clear any existing points
        foreach (var point in _activePoints)
        {
            if (point != null)
                Destroy(point.gameObject);
        }
        _activePoints.Clear();
    }
}