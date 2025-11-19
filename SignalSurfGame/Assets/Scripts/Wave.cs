using System.Collections.Generic;

public class Wave
{
    public Dictionary<int, WavePoint> Points { get; private set; }

    public Wave()
    {
        Points = new();
    }

    public void AddPoint(int beatIndex, WavePoint point)
    {
        Points[beatIndex] = point;
    }

    public void RemovePoint(int beatIndex)
    {
        Points.Remove(beatIndex);
    }

    public void RemovePoint(WavePoint point)
    {
        int beatIndex = point.BeatIndex;
        Points.Remove(beatIndex);
    }

    public WavePoint GetPoint(int beatIndex)
    {
        if (!Points.ContainsKey(beatIndex))
        {
            return null;
        }

        return Points[beatIndex];
    }
}
