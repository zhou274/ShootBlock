using System.Collections;
using UnityEngine;


public class Generator : MonoBehaviour
{

    public GameObject PF_obstacle_Rectangle;
    public GameObject PF_obstacle_Diamond;
    public GameObject PF_obstacle_Bomb;
    public GameObject PF_item_PlusOneBall;
    public GameObject obstacleParent;

    private int obsLife = 1;
    private bool[] oldObjects;
    private bool[] newObjects;
    private Vector3[] generatePosition;


    void Start()
    {
        SetGeneratePosition();
        InitArray();
        StartCoroutine(MoveObstaclesToNextPosition());
    }
    public void init()
    {
        SetGeneratePosition();
        InitArray();
        StartCoroutine(MoveObstaclesToNextPosition());
    }
    // Add generate positions to array  
    void SetGeneratePosition()
    {
        generatePosition = new Vector3[7];
        generatePosition[0] = new Vector3(-4.2f, 0, 9.5f);
        generatePosition[1] = new Vector3(-2.8f, 0, 9.5f);
        generatePosition[2] = new Vector3(-1.4f, 0, 9.5f);
        generatePosition[3] = new Vector3(-0f, 0, 9.5f);
        generatePosition[4] = new Vector3(1.4f, 0, 9.5f);
        generatePosition[5] = new Vector3(2.8f, 0, 9.5f);
        generatePosition[6] = new Vector3(4.2f, 0, 9.5f);
    }


    void InitArray()
    {
        // Set all container to false. 
        oldObjects = new bool[7];
        for (int i = 0; i < oldObjects.Length; i++) oldObjects[i] = false;

        newObjects = new bool[7];
        for (int i = 0; i < newObjects.Length; i++) newObjects[i] = false;
    }

    // Move down all obstacles
    public IEnumerator MoveObstaclesToNextPosition()
    {
        // Distance to next position
        float distanceToNextPosition = 1.4f;
        // Save original(before  move) position to calculate distance 
        float oldZ = obstacleParent.transform.localPosition.z;

        // Move little by little
        while (oldZ - obstacleParent.transform.localPosition.z <= distanceToNextPosition)
        {
            float speed = 0.1f;
            float posX = obstacleParent.transform.localPosition.x;
            float posY = obstacleParent.transform.localPosition.y;
            float posZ = obstacleParent.transform.localPosition.z;
            obstacleParent.transform.localPosition = new Vector3(posX, posY, posZ - speed);
            yield return 0;
        }

        // Put obstacles in the correct position
        obstacleParent.transform.localPosition = new Vector3(obstacleParent.transform.localPosition.x, obstacleParent.transform.localPosition.y, oldZ - distanceToNextPosition);

        // After the move, create new obstacles
        GenerateNewObstacle();

        yield break;
    }

    public void Clear()
    {
        for(int i=0;i< obstacleParent.transform.childCount;i++)
        {
            Destroy(obstacleParent.transform.GetChild(i).gameObject);
        }
    }
    void GenerateNewObstacle()
    {
        // Set all container for new obstacle object to false. 
        newObjects = new bool[7];
        for (int i = 0; i < newObjects.Length; i++) newObjects[i] = false;


        for (int i = 0; i < generatePosition.Length; i++)
        {
            // i ->  0 1 2 3 4 5 6
            // if 'i' is 1 or 4, check if the obstacle in the second line(oldObjects[]) is empty or not.
            // if oldObjects[i] and oldObjects[i+1] is empty(false) make Diamond obstacle.

            // - example -
            // new : 0 1 2 3 4 5 6
            // old : T F F T T T T    ---->    make Diamond obstacle 
            // or
            // new : 0 1 2 3 4 5 6
            // old : T T T T F F T    ---->    make Diamond obstacle

            if ((i == 1 || i == 4) && (oldObjects[i] == false && oldObjects[i + 1] == false))
            {
                //set position of diamond obstacle
                Vector3 pos;
                if (i == 1)
                    pos = new Vector3(-2.1f, 0, 8.8f);
                else
                    pos = new Vector3(2.1f, 0, 8.8f);

                GenerateDiamondObstacle(pos);

                newObjects[i] = true;
                newObjects[i + 1] = true;
            }

            //  60% chance : create an obstacle or '+1 ball' item.  
            if (Random.Range(0, 100) < 60 && newObjects[i] == false)
            {
                GameObject newObj;

                int randomInt = Random.Range(0, 100);
                //  90% chance : create an normal rectangle obstacle.  
                if (randomInt < 90)
                {
                    newObj = Instantiate(PF_obstacle_Rectangle, generatePosition[i], Quaternion.identity);
                    int newObsLife = Mathf.Clamp(Random.Range((int)obsLife / 2, obsLife), 1, obsLife);
                    newObj.GetComponent<Obstacle>().life = newObsLife;
                }
                //  8% chance : create an Bomb.  
                else if (randomInt >= 85 && randomInt < 93)
                {
                    newObj = Instantiate(PF_obstacle_Bomb, generatePosition[i], Quaternion.identity);
                }
                //  7% chance : create an '+1 ball' item.  
                else
                {
                    newObj = Instantiate(PF_item_PlusOneBall, generatePosition[i], Quaternion.identity);
                }

                newObjects[i] = true;
                newObj.transform.SetParent(obstacleParent.transform);
            }
        }

        oldObjects = newObjects;

        // 50% chance : increase obstacle life
        if (Random.Range(0, 100f) < 45f) obsLife++;
    }


    void GenerateDiamondObstacle(Vector3 pos)
    {
        GameObject newDiamond;
        newDiamond = Instantiate(PF_obstacle_Diamond, pos, Quaternion.identity);
        newDiamond.transform.SetParent(obstacleParent.transform);
        int newObsLife = Random.Range(obsLife * 1, obsLife * 2);
        newDiamond.transform.GetChild(0).GetComponent<Obstacle>().life = newObsLife;
    }



}
