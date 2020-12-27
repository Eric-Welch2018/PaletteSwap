using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignTextController : MonoBehaviour
{
    public GameObject signText;
    // Start is called before the first frame update
    void Start()
    {
        signText.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            signText.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        signText.SetActive(false);
    }
}
