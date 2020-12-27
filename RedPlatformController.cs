using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedPlatformController : MonoBehaviour
{
    //These variables will be used to detect if our platform collides with our player
    private Collider2D collider;
    private bool playerCollision = false;

    //This LayerMask is used to check for player and platform collisions
    [SerializeField] private LayerMask playerLayer;

    //These are our UI element variables
    [SerializeField] private Text collisionText;
    [SerializeField] private GameObject collisionTextObject;

    //This vaiable will be used to check if we are a solid platform or a hollow platform
    [SerializeField] private bool isSolid = false;

    //This script variable refernce will be used to check if our player is dashing
    public PlayerController player;
    [SerializeField] private GameObject playerObject;

    //This will be a variable to check if our player is dashing at any given momment
    private bool playerDashing;

    //These variables will be used to add a slight delay to our hollw to solid platform transition
    [SerializeField] private float solidDelaySeconds = .5f;

    //These will be our sprites that will be used to show our hollow platform and solid platform
    public Sprite hollowPlatform;
    public Sprite solidPlatform;

    // Start is called before the first frame update
    void Start()
    {
        //if()
        collider  = GetComponent<Collider2D>();

        //These will set up our gameobject/script refernces for our prefab asset
        //collisionTextObject = GameObject.Find("collisionText");
        //collisionText = collisionTextObject.GetComponent<Text>();

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerCollision();
       // StateChange();
        StartCoroutine(StateChange());
    }
    private void PlayerCollision()
    {
        if(collider.IsTouchingLayers(playerLayer))
        {
            playerCollision = true;
           // this.gameObject.GetComponent<SpriteRenderer>().sprite = solidPlatform;
        }
        else
        {
            playerCollision = false;
           // this.gameObject.GetComponent<SpriteRenderer>().sprite = hollowPlatform;
        }
         //collisionText.text = playerCollision.ToString();
        // collisionTextObject.gameObject.text = playerCollision.ToString();
    } 


    //private void StateChange()
    IEnumerator StateChange()
    {
        playerDashing = player.isDashing;

        //State chage from hollow to solid
        if(isSolid == false)
        {
             this.gameObject.GetComponent<SpriteRenderer>().sprite = hollowPlatform;
             collider.isTrigger = true;
             gameObject.layer = 12;

            if(playerDashing == true && playerCollision == true && player.color == PlayerController.CurrentColor.RED)
            {
                    //This yield statement add a delay to our platform becoming solid
                    yield return new WaitForSeconds(solidDelaySeconds);
                    isSolid = true;
                    collider.isTrigger = false;
                    //This sets our layer to red platfrom, so it acts just like the ground layer
                    gameObject.layer = 8;
            }
        }
        //State chage from solid to hollow
        else if(isSolid == true)
        {
            this.gameObject.GetComponent<SpriteRenderer>().sprite = solidPlatform;
            collider.isTrigger = false;
            gameObject.layer = 8;

            if(playerDashing == true && playerCollision == true && player.color == PlayerController.CurrentColor.BLK)
            {
                isSolid = false;
                collider.isTrigger = true;
                //This sets our layer to hollow, so our player wont interact with a hollow platform
                gameObject.layer = 12;
            }
        }
    }
}
