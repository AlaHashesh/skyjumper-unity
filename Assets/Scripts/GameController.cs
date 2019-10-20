using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using GoogleMobileAds.Api;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public int score;
    public int maxScore;
    private BannerView bannerView = null;
    public static GameController controller;
    private RestartController restartController;

    void Awake () {
        if (controller == null)
        {
            controller = this;
            DontDestroyOnLoad(gameObject);
        } else if(controller != this)
        {
            Destroy(gameObject);
        }
	}

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");

        PlayerData data = new PlayerData();
        data.maxScore = maxScore;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat",
                                        FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            maxScore = data.maxScore;
        }
    }

    public void ShowAd()
    {
        #if UNITY_ANDROID
                string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
                                string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
        #else
                                string adUnitId = "unexpected_platform";
        #endif

        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);

    }

    public void HideAd()
    {
        if(bannerView != null)
        {
            bannerView.Hide();
        }
    }

    public void setRestartController(RestartController restartController)
    {
        this.restartController = restartController;
    }

    public RestartController getRestartController()
    {
        return restartController;
    }
}

[Serializable]
class PlayerData
{
    public int maxScore;
}
