using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip collisionClip;
    public AudioClip breakObstacleeClip;

    public GameObject obstacleDestroyPrefab;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void playCollisionAudio()
    {
        if (audioSource.isPlaying == false) audioSource.PlayOneShot(collisionClip);
    }


    public void playBreakObstacleAudio()
    {
        // this is normal way to play audio
        // audioSource.PlayOneShot(breakObstacleeClip);

        // But sometimes audio doesn't play properly, so I decided to use this method.
        Destroy(Instantiate(obstacleDestroyPrefab, Vector3.zero, Quaternion.identity), 0.05f);
    }
}
