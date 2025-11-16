using UnityEngine;

[CreateAssetMenu(fileName = "NewSong", menuName = "Song")]
public class SongData : ScriptableObject
{
    public string Title;
    public string Artist;
    public AudioClip Audio;
    public int BPM; 
}
