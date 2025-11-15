using UnityEngine;

public class WaveTracker : MonoBehaviour
{
    [SerializeField] private AudioClip song;
    [SerializeField] private int songBPM;
    [SerializeField] private Difficulty difficulty;
    private Wave _currentWave;

    private float _timer = 0f;
    private float _logInterval = 0.5f; // seconds

    void Start()
    {
        _currentWave = new Wave(song, songBPM, difficulty);
    }

    void Update()
    {
        if (_currentWave == null || _currentWave.Complete)
            return;

        _timer += Time.deltaTime;

        if (_timer >= _logInterval)
        {
            float nextCoordinate = _currentWave.GetNextCoordinate();
            Debug.Log(nextCoordinate);
            _timer = 0f; 
        }
    }
}
