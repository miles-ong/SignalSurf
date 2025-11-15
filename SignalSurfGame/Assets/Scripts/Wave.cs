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
    private static readonly Dictionary<Difficulty, int> _difficultyToSampleStep = new()
    {
        { Difficulty.Easy, 2048 },
        { Difficulty.Medium, 1024 },
        { Difficulty.Hard, 512 }
    };
    private static readonly Dictionary<Difficulty, float> _difficultyToAmplitudeScale = new()
    {
        {Difficulty.Easy, 0.5f },
        {Difficulty.Medium, 1f },
        {Difficulty.Hard, 1.5f }
    };
    private Queue<float> _coordinates;

    public Wave(AudioClip songClip, Difficulty difficulty)
    {
        if(songClip == null)
        {
            Debug.Log("Song clip is null!");
            return;
        }

        _coordinates = new();

        float[] audioSamples = new float[songClip.samples * songClip.channels];
        songClip.GetData(audioSamples, 0);

        int sampleStep = _difficultyToSampleStep[difficulty];
        float amplitudeScale = _difficultyToAmplitudeScale[difficulty];

        for(int i = 0; i < audioSamples.Length; i += sampleStep)
        {
            float y = audioSamples[i] * amplitudeScale;
            _coordinates.Enqueue(y);
        }
    }

    public float GetNextCoordinate()
    {
        if(_coordinates.Count == 0)
        {
            return 0;
        }

        return _coordinates.Dequeue();
    }
}
