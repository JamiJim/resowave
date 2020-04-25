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
    public GameObject PauseMenu;

    private Rigidbody rb;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        speed = rb.velocity.magnitude;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (paused)
            {
                Time.timeScale = 1;
                paused = !paused;
            }

            else
            {
                Time.timeScale = 0;
                Instantiate(PauseMenu, this.gameObject.transform.position + new Vector3(517.2f, 289.3f, -505f), Quaternion.identity);
            }



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
            Instantiate(PauseMenu, this.gameObject.transform.position + new Vector3(517.2f, 289.3f, -505f), Quaternion.identity);
            PauseTimer = 5f;
        }

        /*if (Input.GetKeyDown(KeyCode.R))
        {
            UnPauseGame();
        }*/

    }

    public void UnPauseGame()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
            paused = !paused;
            Time.timeScale = 1;
        //}
    }





}
