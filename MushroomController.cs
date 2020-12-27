using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomController : MonoBehaviour
{

    private Animator animator;
    private bool jumpedOn = false;

    // Start is called before the first frame update
    void Start()
    {
        animator   = GetComponent<Animator>();
    }
    void Update()
    {
        animator.SetBool("jumpedOn", jumpedOn);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //make the player jump and reset dashing ability
            jumpedOn = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            //make the player jump and reset dashing ability
           jumpedOn = false;
        }
    }
}
