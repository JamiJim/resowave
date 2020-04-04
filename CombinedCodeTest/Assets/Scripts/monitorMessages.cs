using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class monitorMessages : MonoBehaviour
{
    public Text message;
    string[] messagechoice = new string[9] { "Let's ride!", "You're doing great!", "~You're a resonation sensation~", "Keep it up Zoombie!", "Vaporizing Speeds", "Resonating!", "Straight Vibin'", "That's the spirit!", "~Ok Zoomer~" };
    int choiceCount = 1;
    void Start()
    {
        InvokeRepeating("messageCycle", 30.0f, 30.0f);

    }
    void Update()
    {
        message.text = messagechoice[choiceCount];
    }
    void messageCycle()
    {
        choiceCount = Random.Range(1, 9);

    }
}