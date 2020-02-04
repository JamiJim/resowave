using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    // Start is called before the first frame update
    public VZPlayer gm;
    public Text scoreText;
    void Start()
    {
           gm = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = gm.Score.ToString();
    }
}
