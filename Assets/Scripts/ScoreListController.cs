using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Facebook.Unity;
using System.Collections.Generic;
using System;

public class ScoreListController : MonoBehaviour {

    public GameObject Element;

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
            FB.LogInWithReadPermissions(
                new List<string>() { "user_friends" },
                LogInWithReadPermissionsCallBack
            );
        }
    }

    private void OnHideUnity(bool isUnityShown)
    {
        if (!isUnityShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            FB.LogInWithReadPermissions(
                new List<string>() { "user_friends" },
                LogInWithReadPermissionsCallBack
            );
        }
        else {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void LogInWithReadPermissionsCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            //var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User ID
            //Debug.Log(aToken.UserId);
            // Print current access token's granted permissions
            /*foreach (string perm in aToken.Permissions)
            {
                Debug.Log(perm);
            }*/

            FB.API("/474185026083094/scores", HttpMethod.GET, GetScoresCallback);
            
        }
        else {
            Debug.Log("User cancelled login");
        }
    }

    private void GetScoresCallback(IGraphResult result)
    {
        FacebookScoresAPIResponse facebookScoresAPIResponse = 
            JsonUtility.FromJson<FacebookScoresAPIResponse>(result.RawResult);
        ResponseData[] dataList = facebookScoresAPIResponse.data;
        var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
        // We don't need to update facebook score
        bool needsUpdate = false;
        for (int i=0; i< dataList.Length; i++)
        {
            GameObject temp = Instantiate(Element) as GameObject;
            temp.transform.SetParent(this.transform);
            RectTransform rect = temp.GetComponent<RectTransform>();
            rect.localScale = new Vector3(1, 1, 1);
            
            Text[] list = temp.GetComponentsInChildren<Text>();
            list[0].text = (i + 1) + "";
            list[1].text = dataList[i].user.name;
            if (aToken.UserId.Equals(dataList[i].user.id))
            {
                int x = 0;
                x = Int32.Parse(dataList[i].score);
                if(x >= GameController.controller.maxScore)
                {
                    GameController.controller.maxScore = x;
                    GameController.controller.getRestartController().updateTextFields();
                } else
                {
                    needsUpdate = true;
                    dataList[i].score = GameController.controller.maxScore + "";
                }
            }
            list[2].text = dataList[i].score;
        }

        if (needsUpdate)
        {
            //post to facebook
            FB.LogOut();
            FB.LogInWithPublishPermissions(
                new List<string>() { "publish_actions" },
                logInWithPublishPermissionsCallBack
            );
        }
    }

    private void logInWithPublishPermissionsCallBack(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            var scoreData =
            new Dictionary<string, string>() { { "score", GameController.controller.maxScore + "" } };

            FB.API("/me/scores", HttpMethod.POST, postScoreCallBack, scoreData);
        }
        else {
            Debug.Log("User cancelled login");
        }
    }

    private void postScoreCallBack(IGraphResult result)
    {

    }
}

[Serializable]
public class FacebookScoresAPIResponse
{
    public ResponseData[] data;
}

[Serializable]
public class UserInfo
{
    public string name;
    public string id;
}

[Serializable]
public class ResponseData
{
    public string score;
    public UserInfo user;
}