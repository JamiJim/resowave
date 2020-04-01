using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimeController : MonoBehaviour
{

    //Sets the paused state to false immediately
    private bool paused = false;

    private float speed;
    //private float StartTimer = 10f;
    private float PauseTimer = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity.magnitude;
        Debug.Log(speed);
        Debug.Log(paused);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (paused)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;

            paused = !paused;
        }

        if (speed <= 10.0f)
        {
            PauseGame();

        }

    }

    void PauseGame()
    {
        PauseTimer -= Time.deltaTime;
        if (PauseTimer <= 0)
        {
            Time.timeScale = 0;
            paused = true;

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UnPauseGame();
        }

    }

    void UnPauseGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            paused = !paused;
            Time.timeScale = 1;
            PauseTimer = 5f;

        }
    }
}
