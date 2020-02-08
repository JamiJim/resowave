using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    #region Monobehavior API
    //Sets the paused state to false immediately
    private bool paused = false;
    public float TimeStop = 30;


    // Update is called once per frame
    void Update()
    {
        TimeStop -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (paused)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;

            paused = !paused;
        }

        if (TimeStop <= 0)
        {
            Time.timeScale = 0;
            StartCoroutine("GameReset", 10f);
        }
    }

    IEnumerator GameReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return new WaitForEndOfFrame();
    }

    #endregion
}
