using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAnim : MonoBehaviour
{
    public float DelayTimer = 3f;
    // Start is called before the first frame update
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        GoUp();

    }

  void GoUp()
  { 
      for (int i = 0; i < 10; i++)
      {
            if (i == 5)
            {
                GoDown();

            }
            DelayTimer -= Time.deltaTime;
            this.gameObject.transform.position = transform.position + new Vector3(0f, 0.01f, 0f);

      }


    }

    void GoDown()
    {
        for (int i = 0; i < 10; i++)
        {
            DelayTimer -= Time.deltaTime;
            this.gameObject.transform.position = transform.position + new Vector3(0f, -0.01f, 0f);
        }


    }
}
