using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EditorMode
{
    Place,
    Move,
    Delete
}

public class LevelEditor : MonoBehaviour
{
    [SerializeField] private EditorScroller levelEditorScroller;
    [SerializeField] private GameObject levelEditorUIContent;
    [SerializeField] private int gridCellSize;
    [SerializeField] private GameObject wavePointPrefab;
    [SerializeField] private SongData testSong;

    private RectTransform _contentRect;

    private Level _level;
    private EditorMode _mode;

    void Start()
    {
        _contentRect = levelEditorUIContent.GetComponent<RectTransform>();
        Level test = new Level(Difficulty.Easy, new Song(testSong));
        LoadLevel(test);
    }

    void Update()
    {
        if(_level != null && IsMouseInsideViewport())
        {
            if(_mode == EditorMode.Place && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 localPos = GetLocalPositionFromMouse();
                Debug.Log($"Local Pos: {localPos}");
                Vector2 snappedPos = GetSnappedPosition(localPos);
                Debug.Log($"Snapped Pos: {snappedPos}");
                PlacePoint(snappedPos);
            }
        }
    }

    public void LoadLevel(Level level)
    {
        _level = level;
        _mode = EditorMode.Place;
        LoadGrid();
    }

    public void OnPointClicked(WavePointBehaviour pointBehaviour)
    {

    }

    public void OnPointDragStart(WavePointBehaviour pointBehaviour)
    {

    }
    
    public void OnPointDrag(Vector2 screenPos)
    {

    }
    
    public void OnPointDragEnd()
    {

    }

    private void SaveLevel()
    {
        // json here
        throw new NotImplementedException();
    }

    private void LoadGrid()
    {
        int totalBeats = _level.Song.TotalBeats;
        int contentWidth = totalBeats * gridCellSize;

        Vector2 size = _contentRect.sizeDelta;
        size.x = contentWidth;
        _contentRect.sizeDelta = size;

        levelEditorScroller.UpdateSizes();
    }

    private void PlacePoint(Vector2 position)
    {
        int beatIndex = GetBeatIndex(position.x);

        if (_level.Wave.GetPoint(beatIndex) != null)
        {
            return;
        }

        WavePoint point = new WavePoint(beatIndex, position.y);
        GameObject pointObj = Instantiate(wavePointPrefab, levelEditorUIContent.transform);

        WavePointBehaviour pointBehaviour = pointObj.GetComponent<WavePointBehaviour>();
        pointBehaviour.Initialize(this, point);
        pointBehaviour.transform.localPosition = position;
        Debug.Log($"Placed At: {position}");

        _level.Wave.AddPoint(beatIndex, point);
    }

    private void MovePoint(WavePointBehaviour pointBehaviour, Vector2 canvasPos)
    {

    }

    private void DeletePoint(WavePointBehaviour pointBehaviour)
    {

    }

    private Vector2 GetSnappedPosition(Vector2 position)
    {
        float x = Mathf.RoundToInt(position.x / gridCellSize) * gridCellSize;
        float y = Mathf.RoundToInt(position.y / gridCellSize) * gridCellSize;

        return new Vector2(x, y);
    }

    private Vector2 GetLocalPositionFromMouse()
    {
        Canvas canvas = _contentRect.GetComponentInParent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _contentRect,
            Mouse.current.position.ReadValue(),
            cam,
            out localPoint))
        {
            return localPoint;
        }

        return Vector2.zero;
    }

    private int GetBeatIndex(float x)
    {
        return Mathf.RoundToInt(x / gridCellSize);
    }
    
    private bool IsMouseInsideViewport()
    {
        RectTransform viewport = levelEditorScroller.Viewport;
        Canvas canvas = viewport.GetComponentInParent<Canvas>();
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        Vector2 localMouse;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            viewport,
            Mouse.current.position.ReadValue(),
            cam,
            out localMouse))
        {
            return viewport.rect.Contains(localMouse);
        }

        return false;
    }
}
