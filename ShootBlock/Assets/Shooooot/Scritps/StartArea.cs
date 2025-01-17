using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartArea : MonoBehaviour
{

    GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>(); ;
    }


    private void OnTriggerEnter(Collider other)
    {
        // if obstacle enter to startarea Collider, game over. 
        if (other.gameObject.tag == "Diamond" || other.gameObject.tag == "Rectangle")
            gameManager.GameOver();

    }


}
