using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set;}
    private AudioSource audioSource;

    public bool sound = true;

    private void Awake() {
        MakeSingleton();
        audioSource = GetComponent<AudioSource>();
    }

    private void MakeSingleton () {
        if(Instance != null) {
            Destroy(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SoundOnOff () {
        sound = !sound;
    }

    public void PlaySoundFx (AudioClip clip, float volume) {
        if(sound) {
            audioSource.PlayOneShot (clip, volume);
        }
    }
}
