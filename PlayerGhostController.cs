using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostController : MonoBehaviour
{

    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject playerGhost;
    public bool makeGhost = false;

    // Start is called before the first frame update
    void Start()
    {
        ghostDelaySeconds = ghostDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if(makeGhost == true)
        {
        if(ghostDelaySeconds > 0)
        {
            ghostDelaySeconds -= Time.deltaTime;
        }
        else
        {
            //make a ghost
            GameObject currentGhost = Instantiate(playerGhost, transform.position, transform.rotation);

            //update the current ghost to make it the correct sprite(for aesthetic purposes)
            Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;

            //This is done to ensure that our ghost afterimages have the correct rotation
            currentGhost.transform.localScale = this.transform.localScale;

            currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;

            //reset the ghostDelay counter
            ghostDelaySeconds = ghostDelay;

            //destroy the current ghost(for the sake of resource managment)
            Destroy(currentGhost, 1f);
        }
        }
    }
}
