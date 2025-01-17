using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlusOneBall : MonoBehaviour
{

    public GameObject plusOne;

    public void plusOneText()
    {
        Destroy(Instantiate(plusOne, transform.position, Quaternion.identity), 1.0f);
    }



}
