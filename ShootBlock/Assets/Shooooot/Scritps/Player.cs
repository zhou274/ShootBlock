using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    static public int ballCountComeBack;

    public GameObject Prefab_Ball;
    public TextMesh ballCounterText;
    public GameObject GO_rotator;
    public Generator obstacleGenerator;
    [HideInInspector] public bool isShooting;
    [HideInInspector] public bool isShootAllBalls = false;

    private float dragDistance;
    private bool isDragging;
    private Vector2 touchStartPosition;
    private Vector2 dragPosition;
    private int countBalls = 1;



    void Start()
    {
        isDragging = false;
        isShooting = false;
        ballCounterText.text = countBalls.ToString();

        ballCountComeBack = 0;

        GO_rotator.SetActive(true);
    }


    void Update()
    {
        AimAndShoot();
    }


    public void ResetBallCounter()
    {
        // Show line of sight
        GO_rotator.SetActive(true);
        GO_rotator.transform.rotation = Quaternion.Euler(0, 0, 0);

        isShooting = false;


        countBalls = ballCountComeBack;
        ballCounterText.text = countBalls.ToString();

        ballCountComeBack = 0;
    }

    void AimAndShoot()
    {
        // Touch Start
        if (Input.GetMouseButtonDown(0) == true && isShooting == false && IsPointerOverUIObject() == false)
        {
            isDragging = true;
            touchStartPosition = Input.mousePosition;
        }

        // Dragging
        if (isDragging)
        {
            dragPosition = Input.mousePosition;
            dragDistance = -(dragPosition.x - touchStartPosition.x) / 3f;
            dragDistance = Mathf.Clamp(dragDistance, -70, 70);
            GO_rotator.transform.rotation = Quaternion.Euler(0, dragDistance, 0);
        }

        // Touch End
        if (isDragging == true && Input.GetMouseButtonUp(0) == true)
        {
            // Hide line of sight
            GO_rotator.SetActive(false);

            isShooting = true;
            isDragging = false;

            StartCoroutine(ShootBall());
        }
    }



    IEnumerator ShootBall()
    {
        isShootAllBalls = false;

        while (countBalls > 0)
        {
            //wait 0.1s 
            yield return new WaitForSeconds(0.1f);
            // instantiate new ball
            GameObject newBall = Instantiate(Prefab_Ball, transform.position + new Vector3(0, 0, 0), Quaternion.identity);

            // set angle
            Vector3 newAngle = Quaternion.AngleAxis(GO_rotator.transform.rotation.eulerAngles.y, Vector3.forward) * Vector3.right;
            newBall.GetComponent<Ball>().SetVelocity(new Vector3(newAngle.y * 20, 0, newAngle.x * 20));

            // recalculate ball count and redisplay
            countBalls--;
            ballCounterText.text = countBalls.ToString();
        }

        isShootAllBalls = true;

        yield break;
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }



}