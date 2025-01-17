using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    Vector3 originPos;


    void Start()
    {
        originPos = transform.localPosition;
    }


    public void CallShakeCoroutine(float _amount, float _duration)
    {
        StartCoroutine(Shake(_amount, _duration));
    }


    public IEnumerator Shake(float _amount, float _duration)
    {
        float fullDuration = _duration;
        float currentDuration = _duration;

        while (currentDuration >= 0)
        {
            transform.localPosition = (Vector3)Random.insideUnitCircle * _amount * (currentDuration / fullDuration) + originPos;
            currentDuration -= Time.deltaTime;
            yield return 0;
        }
        transform.localPosition = originPos;

        yield break;
    }










}
