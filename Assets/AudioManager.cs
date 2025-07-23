using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource; 
    public AudioSource vfxSource;
    public AudioClip musicClip;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
