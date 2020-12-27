using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private GameMaster gm;
    private Animator animator;
    private AudioSource audio;

    private bool playerContact = false;

    public AudioClip checkpointSound;

    void Start()
    {
         gm = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
         animator   = GetComponent<Animator>();
         audio      = GetComponent<AudioSource>();

    }
    void Update()
    {
        animator.SetBool("playerContact", playerContact);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
       if(other.gameObject.tag == "Player")
       {
           gm.lastCheckpointPosition = transform.position;
           playerContact = true;
           audio.PlayOneShot(checkpointSound);

       }
    }
}
