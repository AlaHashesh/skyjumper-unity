using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreboardController : MonoBehaviour {

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BackPressed();
        }
    }

    public void BackPressed()
    {
        SceneManager.UnloadScene("scoreboard");
        GameController.controller.ShowAd();
    }

}
