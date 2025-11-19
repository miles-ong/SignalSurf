using UnityEngine;
using UnityEngine.EventSystems;

public class WavePointBehaviour : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public WavePoint Data { get; private set; }
    
    private LevelEditor _editor;

    public void Initialize(LevelEditor builder, WavePoint point)
    {
        _editor = builder;
        Data = point;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _editor.OnPointClicked(this);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _editor.OnPointDragStart(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _editor.OnPointDrag(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _editor.OnPointDragEnd();
    }
}