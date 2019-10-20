using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

public class PlayerController : MonoBehaviour
{

    public float speed;
    public float tilt;
    private Rigidbody2D playerRb;

    public GameObject walls;
    private GameObject nextWall = null;
    private Rigidbody2D nextWallRb;
    private Vector3 nextWallInitialPosition;
    private float wallsMargin;

    float screenHeight;
    float screenWidth;

    private AudioSource pointSound;
    private AudioSource hitSound;

    private float wallHeight;
    private float playerHeight;
    public Text scoreText;
    public Text startText;
    public Canvas restartScreen;
    private bool isStarted = false;
    private int preMotionDirection = 1;

    // Use this for initialization
    void Start()
    {
        Screen.fullScreen = true;
        Screen.SetResolution(Screen.width, Screen.height, false);

        wallHeight = walls.GetComponentInChildren<EdgeCollider2D>().bounds.extents.y;
        playerHeight = GetComponent<MeshRenderer>().bounds.extents.y;

        screenWidth = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f)).x;
        screenHeight = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f)).y;
        wallsMargin = (screenHeight - screenWidth) - playerHeight - wallHeight - 1.5f;

        playerRb = GetComponent<Rigidbody2D>();
        GameController.controller.score = 0;
        scoreText.text = "0";

        var sounds = GetComponents<AudioSource>();
        pointSound = sounds[0];
        hitSound = sounds[1];
    }

    void AfterStart()
    {
        float xPosition = (UnityEngine.Random.value * 3.76f) - 1.88f;
        nextWallInitialPosition = new Vector3(0.0f, -screenHeight, 0.0f);
        nextWall = Instantiate(walls, (nextWallInitialPosition + new Vector3(xPosition, 0.0f, -0.1f)), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
        nextWallRb = nextWall.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted && nextWall != null &&
            nextWallRb.position.y >= -wallsMargin)
        {
            float xPosition = (UnityEngine.Random.value * 3.76f) - 1.88f;
            nextWall = Instantiate(walls, (nextWallInitialPosition + new Vector3(xPosition, 0.0f, -0.1f)), Quaternion.Euler(0.0f, 0.0f, 0.0f)) as GameObject;
            nextWallRb = nextWall.GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PushGameToStack();
        }

        if (isStarted)
        {

            float moveHorizontal = 0;

            if (Input.touchCount > 0)
            {
                if (Input.touches[0].position.x > Screen.width/2)
                {
                    moveHorizontal = 1;
                }
                else
                {
                    moveHorizontal = -1;
                }
                playerRb.velocity = new Vector3(moveHorizontal * speed, 0.0f, 0.0f);
                playerRb.rotation = moveHorizontal * tilt;
            }
            else
            {
                playerRb.velocity = new Vector2(0, 0);
                playerRb.rotation = 0;
            }
            /*moveHorizontal = Input.GetAxis("Horizontal");
            playerRb.velocity = new Vector3(moveHorizontal * speed, 0.0f, 0.0f);
            playerRb.rotation = moveHorizontal * tilt;*/
        }
        else
        {
            PreGameMotion();
            if (Input.touchCount > 0)
            {
                isStarted = true;
                startText.text = "";
                AfterStart();
            }

            /*if (Input.GetMouseButtonDown(0))
            {
                isStarted = true;
                startText.text = "";
                AfterStart();
            }*/
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

    void PreGameMotion()
    {
        if (playerRb.position.x > 1.5)
        {
            preMotionDirection = -1;
        }
        else if (playerRb.position.x < -1.5)
        {
            preMotionDirection = 1;
        }
        playerRb.velocity = new Vector3(preMotionDirection * 0.5f * speed, 0.0f, 0.0f);
        playerRb.rotation = preMotionDirection * 0.5f * tilt;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "SCORE")
        {
            GameController.controller.score++;
            scoreText.text = GameController.controller.score + "";
            Destroy(other.gameObject);
            pointSound.Play();
        }
        else
        {
            Time.timeScale = 0;
            hitSound.Play();
            Instantiate(restartScreen, new Vector3(0, 0, 0), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        }
    }

    void OnEnable()
    {
        GameController.controller.Load();
    }

    void OnDisable()
    {
        GameController.controller.Save();
    }
}
