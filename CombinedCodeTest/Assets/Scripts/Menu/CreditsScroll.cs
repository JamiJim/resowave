using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroll : MonoBehaviour
{
    private Color textColor;
    private Text[] CreditsItems;
    public float Lifetime;
    private float life;
    public Transform startPosition;
    public float ScrollSpeed;
    private bool fadedIn = false;
    private bool created = false;
    public float FadeDelay = 5f;
    private float fadeTime;

    // Start is called before the first frame update
    void Start()
    {
        CreditsItems = GetComponentsInChildren<Text>();
        textColor = GetComponentInChildren<Text>().color;
        textColor.a = 0;
        fadeTime = FadeDelay;
        foreach (Text item in CreditsItems)
        {
            item.color = textColor;
        }
        life = Lifetime;
    }

    public void ResetCredits()
    {
        life = Lifetime;
        textColor.a = 0;
        fadeTime = FadeDelay;
        foreach (Text item in CreditsItems)
        {
            item.color = textColor;
        }
        fadedIn = false;
        created = false;
        this.transform.position = startPosition.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (fadedIn == false)
        {
            fadeTime -= Time.deltaTime;
            if (fadeTime <= 0)
            {
                if (created == false)
                {
                    this.transform.position = startPosition.position;
                    created = true;
                }
                textColor.a += Time.deltaTime;
                foreach (Text item in CreditsItems)
                {
                    item.color = textColor;
                }
                if (textColor.a >= 1)
                {
                    textColor.a = 1;
                    foreach (Text item in CreditsItems)
                    {
                        item.color = textColor;
                    }
                    fadedIn = true;
                }
            }
        }

        life -= Time.deltaTime;

        this.transform.Translate(Vector3.up * ScrollSpeed);
        if (life <= 0)
        {
            textColor.a -= Time.deltaTime;
            foreach (Text item in CreditsItems)
            {
                item.color = textColor;
            }
            if (CreditsItems[0].color.a <= 0)
            {
                ResetCredits();
            }
        }
    }
}
