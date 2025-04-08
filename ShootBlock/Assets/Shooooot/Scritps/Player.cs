using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;

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
    public GameObject PopPanel;
    public string clickid;
    private StarkAdManager starkAdManager;

    void Start()
    {
        isDragging = false;
        isShooting = false;
        ballCounterText.text = countBalls.ToString();

        ballCountComeBack = 0;

        GO_rotator.SetActive(true);
        PopPanel.SetActive(true);
    }

    public void AddBallByAD()
    {
        ShowVideoAd("h3a23goj35prbfd6np",
            (bol) => {
                if (bol)
                {

                    countBalls += 2;
                    ballCounterText.text = countBalls.ToString();
                    PopPanel.SetActive(false);


                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
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

    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }
    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }

}