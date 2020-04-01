using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepUpConverter : MonoBehaviour
{
    private VZPlayer Player;
    public GameObject[] Converters;
    public float Lifetime = 6;
    private float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<VZPlayer>();
        Converters = GameObject.FindGameObjectsWithTag("StepUpConverter"); //This is done as a check for multiples.

        if (Converters.Length > 1) //If a Step-Up Converter has been collected at the same time one is already active...
        {
            Destroy(Converters[0].gameObject); //The old one is removed.
        }

        lifetime = Lifetime;
        Player.ScoreMultiplier = 2; //The point values of every coin collected are doubled while this powerup is active.
    }

    // Update is called once per frame
    void Update()
    {
        lifetime -= Time.deltaTime; //The life of the object ticks down.
        this.gameObject.transform.position = Player.transform.position; //This powerup has a visual effect that follows the player.

        if (lifetime <= 0)
        {
            Converters = GameObject.FindGameObjectsWithTag("StepUpConverter"); //Multiples are checked for one last time...
            if (Converters.Length <= 1) //...though in this situation, there shouldn't ever be more than 1 active. Just in case though.
            {
                Player.ScoreMultiplier = 1;
            }
            Destroy(this.gameObject);
        }
    }
}
