using UnityEngine;

public class Song
{
    public string Title { get; private set; }
    public string Artist { get; private set; }
    public AudioClip Audio { get; private set; }
    public int BPM { get; private set; }
    public int TotalBeats { get; private set; }

    public Song(SongData data)
    {
        Title = data.Title;
        Artist = data.Artist;
        Audio = data.Audio;
        BPM = data.BPM;

        CalculateTotalBeats();
    }

    public Song(string title, string artist, AudioClip audio, int bpm)
    {
        Title = title;
        Artist = artist;
        Audio = audio;
        BPM = bpm;

        CalculateTotalBeats();
    }
    
    private void CalculateTotalBeats()
    {
        if (Audio == null || BPM <= 0)
        {
            TotalBeats = 0;
            return;
        }

        float lengthInSeconds = Audio.length;
        float beatsPerSecond = BPM / 60f;
        TotalBeats = Mathf.RoundToInt(lengthInSeconds * beatsPerSecond);
    }
}