using UnityEngine;
using System.Collections.Generic;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public class Wave
{
    public bool Complete { get; private set; } = false;

    private static readonly int CanvasHeight = 1080;
    private static readonly float CanvasMargin = 100;
    private static readonly Dictionary<Difficulty, int> _difficultyToBeatsPerPoint = new()
    {
        { Difficulty.Easy, 12 },
        { Difficulty.Medium, 8 },
        { Difficulty.Hard, 4 }
    };
    private static readonly Dictionary<Difficulty, float> _difficultyToAmplitudeScale = new()
    {
        {Difficulty.Easy, 0.5f },
        {Difficulty.Medium, 1f },
        {Difficulty.Hard, 1.5f }
    };
    private Queue<float> _coordinates;

    public Wave(AudioClip songClip, float songBPM, Difficulty difficulty)
    {
        if(songClip == null)
        {
            Debug.Log("Song clip is null!");
            return;
        }

        _coordinates = new();
        Complete = false;

        float[] audioSamples = new float[songClip.samples * songClip.channels];
        songClip.GetData(audioSamples, 0);

        float amplitudeScale = _difficultyToAmplitudeScale[difficulty];

        int beatsPerPoint = _difficultyToBeatsPerPoint[difficulty];
        float secondsPerBeat = 60f / songBPM;
        int samplesPerBeat = Mathf.RoundToInt(secondsPerBeat * songClip.frequency);
        int sampleStep = samplesPerBeat * beatsPerPoint;
         
        for(int i = 0; i < audioSamples.Length; i += sampleStep)
        {
            float y = audioSamples[i] * amplitudeScale;
            float normalizedY = Mathf.Lerp(CanvasMargin, CanvasHeight - CanvasMargin, (y + 1f) / 2f);
            _coordinates.Enqueue(normalizedY);
        }
    }

    public float GetNextCoordinate()
    {
        if(_coordinates.Count == 0)
        {
            Complete = true;
            return 0;
        }

        return _coordinates.Dequeue();
    }
}
