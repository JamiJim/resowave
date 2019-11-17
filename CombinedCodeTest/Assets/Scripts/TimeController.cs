using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    #region Monobehavior API
    //Sets the paused state to false immediately
    private bool paused = false;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (paused)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;

            paused = !paused;
        }
    }

    #endregion
}
