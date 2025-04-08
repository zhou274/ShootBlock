using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;
using UnityEngine.Analytics;



public class GameManager : MonoBehaviour
{
    static int COUNT_GAMEPLAY = 1;

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public Generator obstacleGenerator;
    public Text gameOverPanel_text_score;
    public Text gameOverPanel_text_BestScore;
    public Text text_score;

    public Text pausePanel_Score;
    public Text pausePanel_Best;

    public Toggle audioOnOff;

    private Player player;
    private int currentScore = 0;
    private int ball = 0;

    public string clickid;
    private StarkAdManager starkAdManager;

    private void Start()
    {

        // Set Frame Rate
        Application.targetFrameRate = 60;
        // Set Screen Resolution to half
        Screen.SetResolution((int)(Display.main.systemWidth * 0.5f), (int)(Display.main.systemHeight * 0.5f), true);

        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        Time.timeScale = 1;
        text_score.text = "0";

        // Show Unity Ads
        if (COUNT_GAMEPLAY > 1) GetComponent<UnityAdsManager>().ShowAd();
        COUNT_GAMEPLAY++;

        audioOnOff.onValueChanged.AddListener(delegate
        {
            ToggleValueChanged(audioOnOff);
        });
        if (PlayerPrefs.GetString("AudioSetting") == "On") audioOnOff.isOn = true;
        else audioOnOff.isOn = false;

    }


    public void OpenMenuPanel()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
        pausePanel_Score.text = currentScore.ToString();
        pausePanel_Best.text = PlayerPrefs.GetInt("BestScore", 0).ToString();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
        
    }



    public void GameOver()
    {
        // Stop Game
        Time.timeScale = 0;

        // Open Game Over Panel
        gameOverPanel.SetActive(true);

        // Set Score Text
        gameOverPanel_text_score.text = currentScore.ToString();

        //Judgment - best score or not
        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (currentScore > bestScore)
        {
            gameOverPanel_text_BestScore.text = currentScore.ToString();
            PlayerPrefs.SetInt("BestScore", currentScore);
        }
        else
        {
            gameOverPanel_text_BestScore.text = bestScore.ToString();
        }
        //播放插屏广告
        ShowInterstitialAd("1d1b217a9hrn3ep439",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
    }
    public void ContinueGame()
    {
        ShowVideoAd("h3a23goj35prbfd6np",
            (bol) => {
                if (bol)
                {

                    Time.timeScale = 1;
                    gameOverPanel.SetActive(false);
                    GameObject.Find("Generator").GetComponent<Generator>().Clear();


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



    public void AddScore(int n)
    {
        currentScore += n;
        text_score.text = currentScore.ToString();
    }


    public void IncreaseBallCounter()
    {
        ball++;
    }


    public void DecreaseBallCounter()
    {
        ball--;
        // When all the balls are back to starting point
        if (ball <= 0 && player.isShootAllBalls == true)
        {
            player.ResetBallCounter();
            // make new of obstacles
            StartCoroutine(obstacleGenerator.MoveObstaclesToNextPosition());
        }

    }


    void ToggleValueChanged(Toggle change)
    {
        if (audioOnOff.isOn)
        {
            PlayerPrefs.SetString("AudioSetting", "On");
            Debug.Log(PlayerPrefs.GetString("AudioSetting", "-"));
        }
        else
        {
            PlayerPrefs.SetString("AudioSetting", "Off");
            Debug.Log(PlayerPrefs.GetString("AudioSetting", "-"));
        }
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
