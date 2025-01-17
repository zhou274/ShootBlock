using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Obstacle : MonoBehaviour
{

    public GameObject FX_Bomb_Explosion;
    public GameObject PF_DestroyEffect;
    public GameObject PF_DestroyEffect2;
    public TextMesh lifeCounter;
    [HideInInspector] public int life = 1;

    private Color originalObstacleColor;
    private Color originalTextColor;
    private AudioManager audioManager;
    private GameManager gameManager;
    private bool isAlreadyDeadEffect = false;


    void Start()
    {
        //Get Components
        audioManager = GameObject.FindWithTag("GameManager").GetComponent<AudioManager>();
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        originalObstacleColor = gameObject.GetComponent<Renderer>().material.color;

        if (gameObject.tag != "Bomb")
        {
            lifeCounter.text = life.ToString();
            originalTextColor = gameObject.transform.GetChild(0).GetComponent<TextMesh>().color;
        }
    }


    public void Damage(int damage)
    {
        // Set life of this obstacle
        life -= damage;

        // Update life counter of this obstacle
        if (lifeCounter != null) lifeCounter.text = life.ToString();

        // Increase Total score
        gameManager.AddScore(damage);

        if (life <= 0 && isAlreadyDeadEffect == false)
        {
            isAlreadyDeadEffect = true;
            DeadEffect();

            if (gameObject.tag == "Diamond")
                Destroy(transform.parent.gameObject);
            else if (gameObject.tag == "Rectangle")
                Destroy(gameObject);
            else if (gameObject.tag == "Bomb")
                DestroyBomb();
        }
    }


    void DeadEffect()
    {
        // Camera Shake Effect
        Camera.main.GetComponent<CameraManager>().CallShakeCoroutine(0.4f, 0.15f);
        // Play Audio
        if (PlayerPrefs.GetString("AudioSetting") == "On") audioManager.playBreakObstacleAudio();

        // Instantiate Particle Effect
        GameObject newEffect = Instantiate(PF_DestroyEffect, transform.position, Quaternion.identity);
        GameObject newEffect2 = Instantiate(PF_DestroyEffect2, transform.position, Quaternion.identity);
        newEffect.GetComponent<Renderer>().material.color = originalObstacleColor;
        Destroy(newEffect, 5f);
        Destroy(newEffect2, 1f);
    }


    void DestroyBomb()
    {
        // Camera Shake Effect
        Camera.main.GetComponent<CameraManager>().CallShakeCoroutine(1f, 0.5f);
        // Bomb Animation
        GetComponent<Animator>().SetTrigger("Bomb");
        // Instantiate Particle Effect and Destroy
        Destroy(Instantiate(FX_Bomb_Explosion, transform.position, Quaternion.identity), 0.5f);
        //Destroy this game object
        Destroy(gameObject, 0.33f);
    }

    // Change Color of this obstacle when hit the ball 
    public void ChangeColor()
    {
        StartCoroutine(ChangeColorCoroutine());
    }


    IEnumerator ChangeColorCoroutine()
    {
        // Change color to black
        gameObject.GetComponent<Renderer>().material.color = Color.black;
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().color = Color.white;

        // Wait...
        yield return new WaitForSecondsRealtime(0.05f);

        // Change color to original color
        gameObject.GetComponent<Renderer>().material.color = originalObstacleColor;
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().color = originalTextColor;

        yield break;
    }



    private void OnCollisionEnter(Collision other)
    {
        // When hit by a bomb, obstacle take damage as much as remaining life. (=  die immediately)
        if (other.gameObject.tag == "Bomb") Damage(life);
    }


}
