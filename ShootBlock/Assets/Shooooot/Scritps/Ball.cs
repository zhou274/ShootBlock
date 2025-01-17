using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject FX_collision;
    public GameObject FX_destroy;

    private Rigidbody rb;
    private GameManager gameManager;
    private AudioManager audioManager;
    private int counter_wallCollision;

    float velocity_normal = 20;
    float velocity_Max = 40;


    void Start()
    {
        audioManager = GameObject.FindWithTag("GameManager").GetComponent<AudioManager>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        counter_wallCollision = 0;
        gameManager.IncreaseBallCounter();
    }


    private void Update()
    {
        // Keep the speed constant
        KeepSpeedConstand();
    }


    void KeepSpeedConstand()
    {
        if (velocity_normal < velocity_Max) velocity_normal += 0.02f;
        // float velocity_normal = 20;

        if (rb.velocity.magnitude < velocity_normal || rb.velocity.magnitude > velocity_normal)
        {
            float v = velocity_normal / rb.velocity.magnitude;
            rb.velocity = rb.velocity * v;
        }
    }

    // Set Velocity of this ball
    public void SetVelocity(Vector3 vel)
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = vel;
    }


    private void OnCollisionEnter(Collision other)
    {
        // Play Collision Audio
        if (PlayerPrefs.GetString("AudioSetting") == "On") audioManager.playCollisionAudio();

        // Collision Particle Effect
        Destroy(Instantiate(FX_collision, other.contacts[0].point, Quaternion.identity), 1f);

        if (other.gameObject.tag == "Rectangle" || other.gameObject.tag == "Diamond")
        {
            other.gameObject.GetComponent<Obstacle>().Damage(1);
            other.gameObject.GetComponent<Obstacle>().ChangeColor();
        }
        else if (other.gameObject.tag == "Bomb")
        {
            other.gameObject.GetComponent<Obstacle>().Damage(1);
        }


        if (other.gameObject.tag == "Wall")
        {
            // If the ball continues to hit the wall, return the ball to the starting point.
            counter_wallCollision++;
            if (counter_wallCollision > 10) rb.AddForce(new Vector3(0, 0, -700));
        }
        else
        {
            counter_wallCollision = 0;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        // When hit "AddBall" Item
        if (other.gameObject.tag == "AddBall")
        {
            // Instantiate new ball
            GameObject newBall = Instantiate(gameObject, transform.position, Quaternion.identity);
            // Set Random Velocity to new ball
            Vector3 newAngle = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.right;
            newBall.GetComponent<Ball>().SetVelocity(new Vector3(newAngle.y * 20, 0, newAngle.x * 20));
            // call effect (+1 text)
            other.GetComponent<PlusOneBall>().plusOneText();

            Destroy(other.gameObject);
        }
    }


    public void DestroyBall()
    {
        Destroy(Instantiate(FX_destroy, transform.position, Quaternion.identity), 1f);
        Player.ballCountComeBack++;
        gameManager.DecreaseBallCounter();
        Destroy(gameObject);
    }




}