using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WaveRenderer : MonoBehaviour
{
    [SerializeField] private Sprite pointSprite;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private float scrollSpeed = 500f;
    [SerializeField] private float spawnInterval = 0.05f; // Spawn points more frequently for smooth wave
    [SerializeField] private float pointSize = 20f;
    [SerializeField] private Color pointColor = Color.white;
    [SerializeField] private int maxActivePoints = 100;

    private Wave _wave;
    private List<RectTransform> _activePoints = new();
    private float _spawnTimer = 0f;
    private float _canvasWidth;

    void Start()
    {
        if (canvas == null)
        {
            canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            if (canvas == null)
            {
                Debug.LogError("WaveRenderer: Could not find Canvas! Please ensure WaveRenderer is a child of a Canvas.");
                enabled = false;
                return;
            }
        }
        UpdateCanvasWidth();
    }

    void Update()
    {
        if (_wave == null)
            return;

        // Update wave completion status
        _wave.UpdateComplete();

        if (_wave.Complete)
            return;

        UpdateCanvasWidth();
        _spawnTimer += Time.deltaTime;

        // Spawn new point at regular intervals
        if (_spawnTimer >= spawnInterval)
        {
            // Safety check: prevent infinite spawning
            if (_activePoints.Count >= maxActivePoints)
            {
                // Silently drop oldest point when at max
                if (_activePoints.Count > 0 && _activePoints[0] != null)
                {
                    Destroy(_activePoints[0].gameObject);
                    _activePoints.RemoveAt(0);
                }
            }

            // Get current wave position (real-time spectrum analysis)
            float y = _wave.GetNextCoordinate();
            SpawnPoint(y);

            _spawnTimer = 0f;
        }

        // Move and cleanup points
        for (int i = _activePoints.Count - 1; i >= 0; i--)
        {
            RectTransform point = _activePoints[i];
            if (point == null)
            {
                _activePoints.RemoveAt(i);
                continue;
            }

            point.anchoredPosition += Vector2.left * scrollSpeed * Time.deltaTime;

            // Remove if scrolled off left edge
            if (point.anchoredPosition.x < -1000f)
            {
                Destroy(point.gameObject);
                _activePoints.RemoveAt(i);
            }
        }
    }

    private void SpawnPoint(float y)
    {
        // Skip if no sprite assigned
        if (pointSprite == null)
        {
            Debug.LogWarning("WaveRenderer: pointSprite is null. Please assign a sprite in the inspector.");
            return;
        }

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

    // Get the wave Y position at a specific X canvas position
    public float GetWaveYAtCanvasX(float canvasX)
    {
        if (_activePoints.Count == 0)
            return 540f; // Middle of canvas as default

        // Find the closest wave point to the given canvas X position
        float closestDistance = float.MaxValue;
        float closestY = 540f;

        foreach (var point in _activePoints)
        {
            float distance = Mathf.Abs(point.anchoredPosition.x - canvasX);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestY = point.anchoredPosition.y;
            }
        }

        return closestY;
    }
}