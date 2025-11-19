using UnityEngine;
using System.Collections;

public class WaveTracker : MonoBehaviour
{
    [SerializeField] private WaveRenderer waveRenderer;
    [SerializeField] private Player player;
    [SerializeField] private AudioClip song;
    [SerializeField] private Difficulty difficulty;

    [Header("Alignment Settings")]
    [SerializeField] private float safeZoneRadius = 1f;
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private float scorePerSecond = 10f;

    private Wave _currentWave;
    private AudioSource _songSource;
    private float _currentWaveY = 0f;
    private bool _isInitialized = false;

    void Start()
    {
        StartCoroutine(InitializeAsync());
    }

    private IEnumerator InitializeAsync()
    {
        if (!ValidateDependencies())
        {
            yield break;
        }

        SetupAudioSource();

        // Wait one frame to let Unity finish scene initialization
        yield return null;

        // Create wave with AudioSource reference (no heavy processing!)
        _currentWave = new Wave(_songSource, difficulty);

        if (_currentWave == null || _currentWave.Complete)
        {
            Debug.LogError("WaveTracker: Failed to create wave!");
            yield break;
        }

        waveRenderer.Initialize(_currentWave);

        // Wait another frame before starting audio
        yield return null;

        _songSource.Play();
        _isInitialized = true;
    }

    private bool ValidateDependencies()
    {
        if (song == null)
        {
            Debug.LogError("WaveTracker: No AudioClip assigned. Please assign a song in the Inspector.");
            return false;
        }

        if (waveRenderer == null)
        {
            Debug.LogError("WaveTracker: No WaveRenderer assigned. Please assign WaveRenderer in the Inspector.");
            return false;
        }

        return true;
    }

    private void SetupAudioSource()
    {
        _songSource = GetComponent<AudioSource>();
        if (_songSource == null)
        {
            _songSource = gameObject.AddComponent<AudioSource>();
        }

        _songSource.clip = song;
        _songSource.playOnAwake = false;
    }


    void Update()
    {
        if (!_isInitialized || _currentWave == null || _currentWave.Complete)
            return;

        CheckPlayerAlignment();
    }

    private void CheckPlayerAlignment()
    {
        if (player == null || !player.IsAlive)
            return;

        // Get wave Y position at player's location
        _currentWaveY = GetWaveYAtPlayerPosition();

        // Convert player world position to canvas/screen Y for comparison
        float playerY = WorldToCanvasY(player.transform.position.y);

        // Calculate distance from wave
        float distance = Mathf.Abs(playerY - _currentWaveY);

        // Apply damage if outside safe zone, or award points if aligned
        if (distance > safeZoneRadius)
        {
            float damage = damagePerSecond * Time.deltaTime;
            player.TakeDamage(damage);
        }
        else
        {
            // Player is aligned with the wave, award points
            float points = scorePerSecond * Time.deltaTime;
            player.AddScore(points);
        }
    }

    private float GetWaveYAtPlayerPosition()
    {
        if (waveRenderer == null)
            return 540f; // Middle of canvas as default

        // Convert player X position to canvas X
        // Player is at world X (e.g., -7), we need to find where that is on the canvas
        // For simplicity, we'll check at a fixed canvas X position on the left side
        float canvasX = -400f; // Left side of canvas where player roughly aligns

        return waveRenderer.GetWaveYAtCanvasX(canvasX);
    }

    private float WorldToCanvasY(float worldY)
    {
        // Convert world Y (-4 to 4) to canvas Y (100 to 980 for 1080p with margins)
        // This matches the Wave.cs normalization
        float normalizedY = (worldY - (-4f)) / (4f - (-4f)); // 0 to 1
        return Mathf.Lerp(100f, 980f, normalizedY);
    }
}
