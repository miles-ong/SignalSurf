using System;

public enum PointType
{
    Default,
    Tap,
    Hold,
    Danger,
    Switch,
    Warp
}

public class WavePoint
{
    public int BeatIndex;
    public float YCoordinate;
    public PointType Type;

    public WavePoint(int beatIndex, float y, PointType type = PointType.Default)
    {
        BeatIndex = beatIndex;
        YCoordinate = y;
        Type = type;
    }

    public void Overlap()
    {
        // implement type checking here

        throw new NotImplementedException();
    }
}