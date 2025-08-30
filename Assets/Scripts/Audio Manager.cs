using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Music Settings")]
    [SerializeField] private UnityEvent _onSwitchedSong;
    [SerializeField] private List<AudioClip> _songs;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _mutedMusicVolume;
    
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _dialogueSource;
    
    private AudioClip _currentSong;
    private bool _canSwitch = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        _musicSource = GetComponent<AudioSource>();
        
        _currentSong = _songs[Random.Range(0, _songs.Count)];
        _musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_musicSource.isPlaying)
            SwitchSong();
        
        _musicSource.volume = _musicVolume;
    }

    private void SwitchSong()
    {
        int index = _songs.IndexOf(_currentSong);
        if (index == _songs.Count - 1)
            _currentSong = _songs[0];
        else
            _currentSong = _songs[index + 1];
        
        _musicSource.clip = _currentSong; 
        _musicSource.Play();
    }

    public void MuteMusic()
    {
        _musicSource.volume = _mutedMusicVolume;
    }

    public void UnmuteMusic()
    {
        _musicSource.volume = _musicVolume;
    }

    public void PlayDialogueClip(AudioClip clip)
    {
        _dialogueSource.clip = clip;
        _dialogueSource.Play();
    }
    
    public bool IsDialogueClipFinished()
    {
        return _dialogueSource.isPlaying;
    }

    IEnumerator PlaySong()
    {
        _canSwitch = false;
        _musicSource.Play();
        yield return new WaitForSeconds(2f);
        _canSwitch = true;
    }
}
