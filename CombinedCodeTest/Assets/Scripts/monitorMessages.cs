using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class monitorMessages : MonoBehaviour
{
    public Text message;
    string messagechoice;
    int choiceCount = 1;
    void Start()
    {
        InvokeRepeating("messageCycle", 0.0f, 30.0f);

    }
    void Update()
    {
        message.text = messagechoice;
    }
    void messageCycle()
    {
        switch (choiceCount)
        {
            case 1:
                messagechoice = "Let's ride!";
                choiceCount++;
                break;

            case 2:
                messagechoice = "You're doing great!";
                choiceCount++;
                break;
            case 3:
                messagechoice = "~You're a resonation sensation~";
                choiceCount++;
                break;

            case 4:
                messagechoice = "Keep it up Zoombie!";
                choiceCount++;
                break;
            case 5:
                messagechoice = "Vaporizing Speeds";
                choiceCount = 1;
                break;
        }

    }
}