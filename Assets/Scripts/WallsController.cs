using UnityEngine;
using System.Collections;

public class WallsController : MonoBehaviour
{

    public float speed;
    private Rigidbody2D wallsRb;
    public GameObject walls;
    private const int LEFT = 1;
    private const int RIGHT = 2;
    public int direction = 0;


    void Start()
    {
        wallsRb = GetComponent<Rigidbody2D>();
        /* There is a 0.25 that this wall is moving */
        if (Random.value >= 0.5 && Random.value >= 0.5)
        {
            if (Random.value >= 0.5)
            {
                direction = LEFT;
            }
            else
            {
                direction = RIGHT;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (direction == LEFT)
        {
            wallsRb.velocity = new Vector3(speed/2, speed, 0.0f);
        }
        else if (direction == RIGHT)
        {
            wallsRb.velocity = new Vector3(-speed/2, speed, 0.0f);
        }
        else {
            wallsRb.velocity = new Vector3(0.0f, speed, 0.0f);
        }
    }

    public void ReverseDirection()
    {
        if(direction == LEFT)
        {
            direction = RIGHT;
        } else if(direction == RIGHT)
        {
            direction = LEFT;
        }
    }
}
