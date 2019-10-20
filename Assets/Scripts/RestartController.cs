using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartController : MonoBehaviour
{
    private bool isReady;
    public Text score;
    public Text maxScore;

    void Start()
    {
        if (GameController.controller.score > GameController.controller.maxScore)
        {
            GameController.controller.maxScore = GameController.controller.score;
        }
        updateTextFields();
        GameController.controller.ShowAd();
        GameController.controller.setRestartController(this);
    }

    public void updateTextFields()
    {
        score.text = GameController.controller.score + "";
        maxScore.text = GameController.controller.maxScore + "";
    }

    public void RestartClicked()
    {
        GameController.controller.HideAd();
        SceneManager.LoadScene("main");
        Time.timeScale = 1;
    }

    public void ScoreBoardClicked()
    {
        GameController.controller.HideAd();
        SceneManager.LoadScene("scoreboard", LoadSceneMode.Additive);
    }

    public void Update()
    {

        if (!SceneManager.GetSceneByName("scoreboard").isLoaded && Input.GetKeyDown(KeyCode.Escape))
        {
            PushGameToStack();
        }

        Scene scene = SceneManager.GetSceneByName("scoreboard");
        if (scene != null && scene.isLoaded)
        {
            SceneManager.SetActiveScene(scene);
        }
    }

    private void PushGameToStack()
    {
        #if UNITY_ANDROID
                // Get the unity player activity
                AndroidJavaObject activity =
              new AndroidJavaClass("com.unity3d.player.UnityPlayer")
              .GetStatic<AndroidJavaObject>("currentActivity");

                activity.Call<bool>("moveTaskToBack", true);

        #endif
    }
}
