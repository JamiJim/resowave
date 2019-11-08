using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{

    //Initializing Variables
    private CharacterController Controller;
    [SerializeField]
    private float speed = 5.0f;
    private float startZ;
    private float startZT = 1475.0f;
  

    //Platform Prefabs
    public GameObject Platform1;
    public GameObject Platform2;
    public GameObject Platform3;
    public GameObject Platform4;

    //Terrain Prefabs
    public GameObject[] Terrains;
    public GameObject[] desertTrn;
    public GameObject[] cityTrrn;
    //Level timer
    public float targetTime = 120.0f;
    int levelNo = 0;

    //Transform of the Start Platform

    public Transform StartPlatform;

    //Coin Prefabs, note not all of these are used in the actual scene
    public GameObject CoinPattern1;
    public GameObject CoinPattern2;
    public GameObject CoinPattern3;
    public GameObject CoinPattern4;
    public GameObject CoinPattern5;
    public GameObject CoinPattern8;
    public GameObject CoinPattern9;
    public GameObject CoinPattern10;
    public GameObject CoinPattern11;

    public string StartingScene;
    public int LevelLength = 10;
    private int UntilNewLevel;
    public GameObject SkyboxFader; //The skybox transition object should be selected.
    public string SceneToLoad; //Specify the name of the scene you're going to load, and eventually activate once the the skybox begins to fade out;


    private void Awake()
    {
        SceneManager.LoadScene(StartingScene, LoadSceneMode.Additive); //"Additive" is required for loading scenes with other scenes.
    }

    void Start()
    {

        //What's important here is setting StartZ to the StartPlatform Position on the Z axis
        Terrains[0] = desertTrn[0];
        Terrains[1] = desertTrn[1];
        Terrains[2] = desertTrn[2];
        Terrains[3] = desertTrn[3];
        Controller = GetComponent<CharacterController>();
        startZ = StartPlatform.position.z;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(StartingScene));
        UntilNewLevel = LevelLength;

    }




    private void OnTriggerEnter(Collider other)
    {
        //Upon Entering a "Spawn Trigger" 
        //Add 12 to startZ
        //Set spawnLoc to be at the new startZ
        //The spawnLocC is for coins and set at 1.12 units above startZ
        //Instantiate 1 of the 4 possible Platforms at spawn Location
        

        
        if (other.gameObject.CompareTag("SpawnPlatform"))
        {
            startZ += 50f;
            Vector3 spawnLoc = new Vector3(0, 0, startZ);
            Vector3 spawnLocC = new Vector3(0, 1.12f, startZ);
            int random = Random.Range(1, 5);
            int randomC = Random.Range(1, 13);
            UntilNewLevel -= 1;
            
            Debug.Log(UntilNewLevel);
            switch (random)
            {
                case 1:
                    Instantiate(Platform1, spawnLoc, Quaternion.identity);
                   
                    break;
                case 2:
                    Instantiate(Platform2, spawnLoc, Quaternion.identity);

                    break;

                case 3:
                    Instantiate(Platform3, spawnLoc, Quaternion.identity);

                    break;
                case 4:
                    Instantiate(Platform4, spawnLoc, Quaternion.identity);

                    break;



            }

            //Instantiate 1 of the 12 possible coin patterns

            /*switch (randomC)
            {
                case 1:
                    Instantiate(CoinPattern1, spawnLocC, Quaternion.identity);
                    break;
                case 2:
                    Instantiate(CoinPattern2, spawnLocC, Quaternion.identity);
                    break;
                case 3:
                    Instantiate(CoinPattern3, spawnLocC, Quaternion.identity);
                    break;
                case 4:
                    Instantiate(CoinPattern4, spawnLocC, Quaternion.identity);
                    break;
                case 5:
                    Instantiate(CoinPattern5, spawnLocC, Quaternion.identity);
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    Instantiate(CoinPattern8, spawnLocC, Quaternion.identity);
                    break;
                case 9:
                    Instantiate(CoinPattern9, spawnLocC, Quaternion.identity);
                    break;
                case 10:
                    Instantiate(CoinPattern10, spawnLocC, Quaternion.identity);
                    break;
                case 11:
                    Instantiate(CoinPattern11, spawnLocC, Quaternion.identity);
                    break;
                case 12:
                    break;


            }*/
        }

    
       

        //Once you enter the "Delete Trigger" (that is at the end of each platform)
        //Destroy a gameobject with this name after 9 seconds.

        if(other.gameObject.CompareTag("DeletePlatformGreen"))
        {
            Destroy(GameObject.Find("Platform 1(Clone)"));

        }

        if (other.gameObject.CompareTag("DeletePlatformRed"))
        {
            Destroy(GameObject.Find("Platform 2(Clone)"));

        }

        if (other.gameObject.CompareTag("DeletePlatformBlue"))
        {

            Destroy(GameObject.Find("Platform 3(Clone)"));

        }

        if (other.gameObject.CompareTag("DeletePlatformPurple"))
        {

            Destroy(GameObject.Find("Platform 4(Clone)"));
        }

    }


    

    void Update()
        {
        targetTime -= Time.deltaTime;
        if (targetTime <= 0.0f)
        {
            if (levelNo == 1)
            {
                levelNo = 0;
            }
            else
            {
                levelNo++;
            }
            targetTime = 120.0f;
            loadNextLevelAssets();
        }
        if (Input.GetButtonDown("-"))
        {
            if (speed != 0)
            { speed = speed - 5; }
            Debug.Log("Decelerating");
        }
        if (Input.GetButtonDown("+"))
        {
            speed = speed + 5;
            Debug.Log("Accelerating");
        }
        //Debug.Log("Speed: " + speed);

        //Moving the character
        Vector3 direction = Vector3.forward;
            Vector3 velocity = direction * speed;
            Controller.Move(velocity * Time.deltaTime);
        if (UntilNewLevel <= 0)
        {
            Debug.Log("Entered");
           // Instantiate(SkyboxFader, this.transform.position, this.transform.rotation);
           // SkyboxFade.SceneToBeLoaded = SceneToLoad; //Let's the skybox object's script know that this is the scene that will be activated.
            UntilNewLevel = LevelLength;  //Resets the level length;
            startZT += 500;
            Vector3 spawnLoc = new Vector3(-250, 0, startZT);
            int randomT = Random.Range(0, 4);
            Instantiate(Terrains[randomT], spawnLoc, Quaternion.identity);
            /*switch (randomT)
            {
                case 1:
                    Instantiate(Terrains[1], spawnLoc, Quaternion.identity);

                    break;
                case 2:
                    Instantiate(Terrains[1], spawnLoc, Quaternion.identity);

                    break;

                case 3:
                    Instantiate(Terrains[1], spawnLoc, Quaternion.identity);

                    break;
                case 4:
                    Instantiate(Terrains[1], spawnLoc, Quaternion.identity);

                    break;



            }*/
        }
        }
    void loadNextLevelAssets()
    {
        Debug.Log("timer end");
        for(int i=0; i < 4; i++)
        {
            switch (levelNo)
            {
                case 1:
                    Terrains[i] = cityTrrn[i];
                    break;
                case 0:
                    Terrains[i] = desertTrn[i];
                    break;

            }

        }
    }

}

