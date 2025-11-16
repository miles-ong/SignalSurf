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

    private WavePointBehaviour _movingPoint;

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
            if (_mode == EditorMode.Place && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 localPos = GetLocalPositionFromMouse();
                Debug.Log($"Local Pos: {localPos}");
                Vector2 snappedPos = GetSnappedPosition(localPos);
                Debug.Log($"Snapped Pos: {snappedPos}");
                PlacePoint(snappedPos);
            }
            
            if (Keyboard.current.digit1Key.wasPressedThisFrame) _mode = EditorMode.Place;
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) _mode = EditorMode.Move;
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) _mode = EditorMode.Delete;
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
        if(_mode == EditorMode.Delete)
        {
            DeletePoint(pointBehaviour);
        }
    }

    public void OnPointDragStart(WavePointBehaviour pointBehaviour)
    {
        if(_mode == EditorMode.Move)
        {
            _movingPoint = pointBehaviour;
        }
    }
    
    public void OnPointDrag(Vector2 screenPos)
    {
        if(_mode == EditorMode.Move && _movingPoint != null)
        {
            Vector2 localPos = GetLocalPositionFromMouse();
            Vector2 snappedPos = GetSnappedPosition(localPos);
            _movingPoint.transform.localPosition = snappedPos;
        }
    }
    
    public void OnPointDragEnd()
    {
        if(_mode == EditorMode.Move && _movingPoint != null)
        {
            Vector2 localPos = GetLocalPositionFromMouse();
            MovePoint(_movingPoint, localPos);
            _movingPoint = null;
        }
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
        int beatIndex = GetBeatIndexFromCoordinate(position.x);

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

    private void MovePoint(WavePointBehaviour pointBehaviour, Vector2 contentPos)
    {
        Wave wave = _level.Wave;
        WavePoint point = pointBehaviour.Data;
        WavePoint pointAtBeatIndex = wave.GetPoint(GetBeatIndexFromCoordinate(contentPos.x));

        if (pointAtBeatIndex != null && pointAtBeatIndex != point)
        {
            Vector2 previousPos = new Vector2(GetCoordinateFromBeatIndex(point.BeatIndex), point.YCoordinate);
            pointBehaviour.transform.localPosition = previousPos;
            return;
        }

        Vector2 snappedPos = GetSnappedPosition(contentPos);
        pointBehaviour.transform.localPosition = snappedPos;
        point.BeatIndex = GetBeatIndexFromCoordinate(snappedPos.x);
        point.YCoordinate = snappedPos.y;
    }

    private void DeletePoint(WavePointBehaviour pointBehaviour)
    {
        _level.Wave.RemovePoint(pointBehaviour.Data);
        Destroy(pointBehaviour.gameObject);
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

    private int GetBeatIndexFromCoordinate(float x)
    {
        return Mathf.RoundToInt(x / gridCellSize);
    }

    private int GetCoordinateFromBeatIndex(int beatIndex)
    {
        return beatIndex * gridCellSize;
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
