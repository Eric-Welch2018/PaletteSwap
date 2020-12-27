using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //These variables are used to control specific mechanisms inside our Unity components which control the game
    private Rigidbody2D rigidbody;
    private Animator animator;
    private Collider2D collider;
    private AudioSource audio;
    private GameMaster gameMaster;

    //This is our finite state machine that will transition between our animation states
    private enum State {idleBLK, runningBLK, jumpingBLK, fallingBLK,
                        idleRED, runningRED, jumpingRED, fallingRED}
    private State state = State.idleBLK;

    //These variables will be used for our in game color transitions
    public enum CurrentColor {BLK, RED}
    public CurrentColor color = CurrentColor.BLK;

    //These variables will be used to manipulate our dash move
    [SerializeField] private float dashSpeed;
    private float dashTime;
    [SerializeField] private float startDashTime;
    private int direction = 0;
    public bool isDashing = false;
    private int numberOfDashesRemaning = 1;

    //These variables control specific attributes in our game

    //This LayerMask is used to check for player and ground collisions
    [SerializeField] private LayerMask ground;

    //This layermask is for our red platforms
    [SerializeField] private LayerMask redPlatform;


    //This is used to control the speed of the player
    [SerializeField] private float speed = 7f;

    //This is used to control the jump of the player
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private float mushroomJumpforce = 20f;
 
    //This script variable refernce will be used to control our dash afterimage effect
    public PlayerGhostController ghost;

    //These are our UI element variables
    [SerializeField] private Text dashingText;

    [SerializeField] private GameObject dashingTextObject;

    //use this variable to adjust the length of the raycast ray.
    private float rayLength = .9f;
    // private float rayLengthForDashing = 2f;
    //This will be used to establish the raycasts offset that check for ground collision.
    public Vector3 raycastOffset;

    //this variable will be used to see if our player is touching the ground
    public bool onGround = false;

    //this variable will be used to add variable height to our jumping mechanics
    public float fallMultiplier = 5f;

    //these variables will be used to enhance our jumping feature to benefit player experience
    public float jumpDelay = .25f;
    private float jumpTimer;


    //***********************************************************************************
    //Sound Effects
    public AudioClip jumpSound;
    public AudioClip dashSound;
    public AudioClip colorChangeSound;
    


    // Start is called before the first frame update
    void Start()
    {
         //These attributes are assigned to game components during its start sequence

        rigidbody  = GetComponent<Rigidbody2D>();
        animator   = GetComponent<Animator>();
        collider   = GetComponent<Collider2D>();
        audio      = GetComponent<AudioSource>();
        gameMaster = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        transform.position = gameMaster.lastCheckpointPosition;

        //This will set up our script/gameobject refernces for our prefab asset
        // dashingTextObject = GameObject.Find("dashingText");
        // dashingText = dashingTextObject.GetComponent<Text>();

        //This ensures that our dashTime is equal to our startDashTime when the game starts
        dashTime = startDashTime;
    }
    //Fixed update will be called within the same amount of time everytime. in other words, these calls will no be
    //effected by framerate changes and will have uniform synced execution.
    //Best used for: physics calculations or things that require perfect uniform synced action.
     void FixedUpdate()
    {
        // ModifyPhysics();
        // Movement();
        // AnimationState();
        // Dashing();

        // animator.SetInteger("state", (int)state);
         
        if(jumpTimer > Time.time && onGround == true)
        {
            Jump(jumpforce);
        }
    } 

    // Update is called once per frame, but is subject to framerate changes, so calls will vary depending on game performance.
    //Best used for: non-physics calculations or things that do not require perfect synced action.
    void Update()
    {
        ModifyPhysics();
        ColorChange();
        Movement();
        AnimationState();
        Dashing();
        //UIControl();

        animator.SetInteger("state", (int)state);

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
    private void Movement()
    {
        //Control player movement

        //movement variables
        float hdirection = Input.GetAxis("Horizontal");
        //float vdirection = Input.GetAxis("Vertical");

        //Move left using A or left arrow key
        if (hdirection < 0)
        {
            rigidbody.velocity = new Vector2(-speed, rigidbody.velocity.y);

            //This will flip the sprite to the left, so it can turn to the left
            transform.localScale = new Vector2(-1, 1);

           // ShootRaycast(false, rayLength);
        }

        //Move right using D or using right arrow key
        else if (hdirection > 0)
        {
            rigidbody.velocity = new Vector2(speed, rigidbody.velocity.y);

            //This will flip the sprite to the right, so it can turn to the right
            transform.localScale = new Vector2(1, 1);

          //  ShootRaycast(true, rayLength);
        }

        //Move up using Space
        if (Input.GetButtonDown("Jump"))
        {
           /*  if(collider.IsTouchingLayers(ground) || collider.IsTouchingLayers(redPlatform))
            {
            Jump(jumpforce);
            } */
            jumpTimer = Time.time + jumpDelay;

           /*  if(onGround == true)
            {
                Jump(jumpforce);
            } */
        }
       
    }

    private void ModifyPhysics()
    {
        onGround = Physics2D.Raycast(transform.position + raycastOffset, Vector2.down, 1.35f, ground) ||
                   Physics2D.Raycast(transform.position - raycastOffset, Vector2.down, 1.35f, ground);

        //set player friction based off of ground contact of raycasts.
        if(onGround == true)
        {
            rigidbody.sharedMaterial.friction = 12.5f;
            //This zeros out our movement when we are on the ground and have no player input.
            rigidbody.velocity = new Vector2(0.0f, 0.0f);
            rigidbody.gravityScale = 1f;
        }
        else
        {
            rigidbody.sharedMaterial.friction = 0;
            if(rigidbody.velocity.y < 0f)
            {
                rigidbody.gravityScale = fallMultiplier;
            }
            else if(rigidbody.velocity.y > 0f && !Input.GetButton("Jump") )
            {
                rigidbody.gravityScale = (fallMultiplier / 2);
            }
            if(collider.IsTouchingLayers(ground))
            {
                rigidbody.velocity = new Vector2(0.0f, rigidbody.velocity.y);
            }
        }


    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + raycastOffset, transform.position + Vector3.down * 1.35f + raycastOffset);
        Gizmos.DrawLine(transform.position - raycastOffset, transform.position + Vector3.down * 1.35f - raycastOffset);
    }

    private void Jump(float jumpforce)
    {
        //old jump method
        rigidbody.sharedMaterial.friction = 0;
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpforce);
        audio.PlayOneShot(jumpSound);
        jumpTimer = 0;
        

            //If we are currently BLK, then we will do  the jumpingBLK animation
            if(color == CurrentColor.BLK)
            {
                state = State.jumpingBLK;
            }
            //If we are currently RED, then we will do  the jumpingRED animation
            else if(color == CurrentColor.RED)
            {
                state = State.jumpingRED;
            }

    }

    private void AnimationState()
    {
        //This will check to make sure we are within the BLK flame state so we play the correct BLK animations
        if(color == CurrentColor.BLK)
        {
            //This is our jump state
            if(state  == State.jumpingBLK)
            {
                //rigidbody.sharedMaterial.friction = 0;
                if(rigidbody.velocity.y < .1f)
                {
                    state = State.fallingBLK;
                }
            }

            //This is our falling state
            else if(state == State.fallingBLK)
            {
                //rigidbody.sharedMaterial.friction = 0;
                if(collider.IsTouchingLayers(ground) || collider.IsTouchingLayers(redPlatform))
                {
                    state = State.idleBLK;
                }
            }

            //This is our run state
            //This if is true if we are moving at all in any x direction
            else if(Mathf.Abs(rigidbody.velocity.x) > 2f)
            { 
                //rigidbody.sharedMaterial.friction = 12.5f;
                state = State.runningBLK;
            }

            //This is our default idle state.
            else
            {
                //rigidbody.sharedMaterial.friction = 12.5f;
                state = State.idleBLK;
            }
        }

        //This will check to make sure we are within the RED flame state so we play the correct RED animations
        else if(color == CurrentColor.RED)
        {
            //This is our jump state
            if(state  == State.jumpingRED)
            {
                if(rigidbody.velocity.y < .1f)
                {
                    state = State.fallingRED;
                }
            }

            //This is our falling state
            else if(state == State.fallingRED)
            {
                if(collider.IsTouchingLayers(ground) || collider.IsTouchingLayers(redPlatform))
                {
                    state = State.idleRED;
                }
            }

            //This is our run state
            //This if is true if we are moving at all in any x direction
            else if(Mathf.Abs(rigidbody.velocity.x) > 2f)
            { 
                state = State.runningRED;
            }

            //This is our default idle state.
            else
            {
                state = State.idleRED;
            }  
        }
    }    

    private void ColorChange()
    {
        //If you are pressing the V key and on the ground, change color to black
        if (Input.GetButtonDown("Fire0") && collider.IsTouchingLayers(ground) && state == State.idleRED)
        {
            color = CurrentColor.BLK;
            audio.PlayOneShot(colorChangeSound);
        }
        //If you are pressing the B key and on the ground, change color to red
        else if(Input.GetButtonDown("Fire1") && collider.IsTouchingLayers(ground) && state == State.idleBLK)
        {
            color = CurrentColor.RED;
            audio.PlayOneShot(colorChangeSound);
        }
    }
    private void Dashing()
    {
        //float xInput = Input.GetAxis("Horizontal");

        if(collider.IsTouchingLayers(ground) || collider.IsTouchingLayers(redPlatform))
        {
            numberOfDashesRemaning = 1;
        }

        //This will track what direction the player is moving in so we know which way to dash
        if(direction == 0)
        {
           //When the player hits the dash key, it grabs the players direction from the arrow keys
           if (Input.GetButtonDown("Dash") && numberOfDashesRemaning > 0)
           {
               if(Input.GetKey("left"))
               {
                   direction = 1;
               }
               else if(Input.GetKey("right"))
               {
                   direction = 2;
               }
               else if(Input.GetKey("up"))
               {
                   direction = 3;
               }
               else if(Input.GetKey("down"))
               {
                   direction = 4;
               }
               audio.PlayOneShot(dashSound);
           }
        }
        else 
        {
            //when dashTime hits zero, we reset the dash time and zero out the player direction and player velocity
            if(dashTime <= 0)
            {
                direction = 0;
                dashTime = startDashTime;
                rigidbody.velocity = Vector2.zero;
                ghost.makeGhost = false;
                isDashing = false;
                numberOfDashesRemaning = 0;
                rigidbody.gravityScale = 1;
            }
            else
            {
                //subtract time from our dash time based on the in-game clock counter
                dashTime -= Time.deltaTime;
                rigidbody.gravityScale = 0;
                
                if(direction == 1)
                {
                    rigidbody.velocity = Vector2.left * dashSpeed;
                    //ShootRaycast(false, rayLength);
                    ghost.makeGhost = true;
                    isDashing = true;
                }
                else if(direction == 2)
                {
                    rigidbody.velocity = Vector2.right * dashSpeed;
                   // ShootRaycast(true, rayLength);
                    ghost.makeGhost = true;
                    isDashing = true;
                }
                else if(direction == 3)
                {
                    rigidbody.velocity = Vector2.up * dashSpeed;
                    ghost.makeGhost = true;
                    isDashing = true;
                }
                else if(direction == 4)
                {
                    rigidbody.velocity = Vector2.down * dashSpeed;
                    ghost.makeGhost = true;
                    isDashing = true;
                }
            }
        }
    }
    private void UIControl()
    {
        //This updates our UI text to show when dashing is true or false(testing purposes only)
        dashingText.text = isDashing.ToString();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "mushroom")
        {
            //make the player jump and reset dashing ability
            Jump(mushroomJumpforce);
            numberOfDashesRemaning = 1;
           
        }
    }
}
