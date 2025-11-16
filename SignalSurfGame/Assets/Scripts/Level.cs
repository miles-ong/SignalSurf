using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class Level
{
    public Difficulty Difficulty { get; private set; }
    public Song Song { get; private set; }
    public Wave Wave { get; private set; }

    public Level(Difficulty difficulty, Song song, Wave wave)
    {
        Difficulty = difficulty;
        Song = song;
        Wave = wave;
    }

    public Level(Difficulty difficulty, Song song)
    {
        Difficulty = difficulty;
        Song = song;
        Wave = new();
    }
}
