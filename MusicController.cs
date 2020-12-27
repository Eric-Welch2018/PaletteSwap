using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    AudioSource audioData;
    public void Awake()
    {
        audioData = GetComponent<AudioSource>();
        audioData.Play(0);
        //DontDestroyOnLoad(this.gameObject);
    }
}
