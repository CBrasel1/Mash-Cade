using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerDK : MonoBehaviour {

    [SerializeField]
    private AudioClip music;

    [SerializeField]
    private AudioClip death;

    private AudioSource source;

    private void Start () {
        source = GetComponent<AudioSource>();
        source.PlayOneShot(music);
	}

    internal void PlayDeath() {
        source.Pause();
        source.PlayOneShot(death);
    }

    internal void PlayMusic() {
        source.UnPause();
    }

    internal void PauseMusic() {
        source.Pause();
    }

}
