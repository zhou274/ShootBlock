using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ball")
            other.gameObject.GetComponent<Ball>().DestroyBall();
    }


}
