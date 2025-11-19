using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditorScroller : MonoBehaviour
{
    public RectTransform Viewport;
    public RectTransform Content;
    [SerializeField] private Scrollbar scrollbar;

    private Vector2 _previousMousePosition;
    private float _viewportWidth;
    private float _contentWidth;


    void Start()
    {
        UpdateSizes();

        if (scrollbar != null)
        {
            scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
            UpdateScrollBar();
        }
    }

    void Update()
    {
        if(Mouse.current.rightButton.isPressed)
        {
            Vector2 currentMouse = Mouse.current.position.ReadValue();
            if (_previousMousePosition != Vector2.zero)
            {
                Vector2 delta = currentMouse - _previousMousePosition;

                Vector2 pos = Content.anchoredPosition;
                pos.x += delta.x;
                pos.x = Mathf.Clamp(pos.x, _viewportWidth - _contentWidth, 0);
                Content.anchoredPosition = pos;

                UpdateScrollBar();
            }

            _previousMousePosition = currentMouse;
        }
        else
        {
            _previousMousePosition = Vector2.zero;
        }
    }
    
    public void UpdateSizes()
    {
        _viewportWidth = Viewport.rect.width;
        _contentWidth = Content.rect.width;

        if(scrollbar != null)
        {
            float size = Mathf.Clamp01(_viewportWidth / _contentWidth);
            scrollbar.size = size;
        }
    }

    private void OnScrollbarChanged(float value)
    {
        value = 1f - value;

        float x = Mathf.Lerp(_viewportWidth - _contentWidth, 0, value);
        Vector2 pos = Content.anchoredPosition;
        pos.x = x;
        Content.anchoredPosition = pos;
    }

    private void UpdateScrollBar()
    {
        if (scrollbar != null)
        {
            float raw = Mathf.InverseLerp(_viewportWidth - _contentWidth, 0, Content.anchoredPosition.x);
            scrollbar.value = 1f - raw;
        }
    }
}