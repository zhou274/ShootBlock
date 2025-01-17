using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{

    void Start()
    {
        //StartCoroutine(GoToMainScene());
    }

    public void Play()
    {
        StartCoroutine(GoToMainScene());
    }
    IEnumerator GoToMainScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        yield break;
    }

}
