using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; 
    public AudioSource vfxSource;
    public AudioClip musicClip;
    
    void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    void Update()
    {
        
    }
}
