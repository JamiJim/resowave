using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject[] MenuItems;

    public GameObject ButtonDisplay;
    private Image VisualButton;
    public Sprite ButtonNoInput;
    public Sprite ButtonUp;
    public Sprite ButtonDown;
    public Sprite ButtonRight;
    public Sprite ButtonRightInactive;
    public Sprite ButtonLeft;
    public Sprite ButtonCreditsNoInput;
    public Sprite ButtonCreditsUp;
    public Sprite ButtonCreditsDown;
    public Sprite ButtonCreditsRight;
    public Sprite ButtonCreditsRightInactive;
    public Sprite ButtonCreditsLeft;
    public Sprite ButtonSettingsNoInput;
    public Sprite ButtonSettingsUp;
    public Sprite ButtonSettingsDown;
    public Sprite ButtonSettingsRight;
    public Sprite ButtonSettingsLeft;

    public GameObject VisualSelection;
    private Slider SelectionSlider;

    private bool isSelecting = false;
    private bool changeInSelection = false;

    public int position = 1;
    private int maxPosition;

    public bool Selectable = true;
    public float MovementTimer = 1.0f;
    private float MoveTime;
    public float SelectTimer = 0;
    public GameObject ConfirmText;
    public float transformAmount = 191;
    private string MoveDirection = "";
    private Vector3[] lastPositions;
    public float SlideAmount;
    public string MenuState = "Main";
    public bool ChangingMenus = false;
    public GameObject[] CreditsItems;
    public GameObject CenterTarget;
    public GameObject BackCenterTarget;
    public float SelectionMoveAmount;
    public GameObject[] SettingsItems;
    public GameObject MasterCenterTarget;
    public GameObject MusicCenterTarget;
    public GameObject SoundCenterTarget;
    public GameObject SettingsBackCenterTarget;
    private Vector3[] SettingsLastPositions;
    public GameObject[] SettingsFirstPositions;
    public bool AdjustingVolume = false;
    public Slider VolumeSlider;
    public Slider MusicSlider;
    public Slider SoundEffectSlider;
    public float SelectionTime;
    public float SettingsSlideAmount;
    public bool Confirmed = true;
    public GameObject Warning;

    private VZController Controller;
    private TimeController Pause;

    private AudioSource audioSource;
    public AudioClip SoundEffectTest;


    // Start is called before the first frame update
    void Start()
    {
        maxPosition = MenuItems.Length - 1;
        VisualButton = ButtonDisplay.GetComponent<Image>();
        SelectionSlider = VisualSelection.GetComponentInChildren<Slider>();
        Controller = GameObject.FindGameObjectWithTag("VZController").GetComponent<VZController>();
        Pause = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<TimeController>();
        audioSource = GameObject.FindGameObjectWithTag("BikeInputCont").GetComponent<AudioSource>();
        int i = 0;
        int j = 0;
        VolumeSlider.value = MenuControl.MasterVolume;
        MusicSlider.value = MenuControl.MusicVolume;
        SoundEffectSlider.value = MenuControl.SoundEffectVolume;
        lastPositions = new Vector3[MenuItems.Length];
        SettingsLastPositions = new Vector3[SettingsItems.Length];
        Debug.Log(SettingsLastPositions.Length);
        foreach (GameObject item in MenuItems)
        {
            lastPositions[i] = MenuItems[i].transform.position;
            i += 1;
            item.SetActive(true);
        }
        foreach (GameObject settings in SettingsItems)
        {
            SettingsLastPositions[j] = SettingsItems[j].transform.position;
            j += 1;
            settings.SetActive(false);
        }
        Warning.SetActive(false);
        foreach (GameObject credits in CreditsItems)
        {
            credits.SetActive(false);
        }
    }

    bool CrossInput(string direction)
    {
        if (direction == "up")
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Controller.DpadUp.Down)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        if (direction == "down")
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Controller.DpadDown.Down)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        if (direction == "left")
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Controller.DpadLeft.Down)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        if (direction == "right")
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Controller.DpadRight.Down)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void VisualButtonControls()
    {
        if (MenuState == "Main" || MenuState == "Credits" || (MenuState == "Settings" && position == 3))
        {

            if (CrossInput("up") && !CrossInput("down"))
            {
                VisualButton.sprite = ButtonUp;
            }

            if (CrossInput("down") && !CrossInput("up"))
            {
                VisualButton.sprite = ButtonDown;
            }

            if (CrossInput("right") && !CrossInput("left") && (isSelecting == true || ChangingMenus == false))
            {
                VisualButton.sprite = ButtonRight;
            }

            if (CrossInput("right") && !CrossInput("left") && (isSelecting == false || ChangingMenus == true))
            {
                VisualButton.sprite = ButtonRightInactive;
            }

            if (CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonLeft;
            }

            if (!CrossInput("up") && !CrossInput("down") && !CrossInput("right") && !CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonNoInput;
            }
        }

        if (MenuState == "Settings" && position < 3)
        {

            if (CrossInput("up") && !CrossInput("down"))
            {
                VisualButton.sprite = ButtonSettingsUp;
            }

            if (CrossInput("down") && !CrossInput("up"))
            {
                VisualButton.sprite = ButtonSettingsDown;
            }

            if (CrossInput("right") && !CrossInput("left") && (isSelecting == true || ChangingMenus == false))
            {
                VisualButton.sprite = ButtonSettingsRight;
            }

            if (CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonSettingsLeft;
            }

            if (!CrossInput("up") && !CrossInput("down") && !CrossInput("right") && !CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonSettingsNoInput;
            }
        }
    }

    void VisualHighlightControls()
    {
        if (Selectable == true)
        {

            if (!CrossInput("right"))
            {
                Confirmed = false;
            }

            if (ChangingMenus == true)
            {
                isSelecting = false;
            }

            if (isSelecting == false)
            {
                SelectionSlider.value = 0;
            }

            if (CrossInput("right") && ChangingMenus == false)
            {
                if (AdjustingVolume == false)
                {
                    Confirmed = true;
                }
            }
            //SelectionEffect.color = SelectionColor;
        }
    }

    void VisualMenuControls()
    {
        if (MenuState != "Credits" && MenuState != "MainToCredits" && MenuState != "CreditsToMain" && MenuState != "MainToSettings" && MenuState != "SettingsToMain" && MenuState != "Settings")
        {
            if (CrossInput("up") && Selectable && (position > 0) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                foreach (GameObject item in MenuItems)
                {
                    lastPositions[i] = MenuItems[i].transform.position;
                    i += 1;
                }
                position -= 1;
                MoveDirection = "up";
                MoveTime = MovementTimer;
            }

            if (CrossInput("down") && Selectable && (position < maxPosition) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                foreach (GameObject item in MenuItems)
                {
                    lastPositions[i] = MenuItems[i].transform.position;
                    i += 1;
                }
                position += 1;
                MoveDirection = "down";
                MoveTime = MovementTimer;
            }

            if (Selectable == false)
            {
                if (MoveDirection == "up")
                {
                        int i = 0;
                        foreach (GameObject item in MenuItems)
                        {
                            item.transform.position = lastPositions[i];
                            item.transform.Translate(Vector3.down * transformAmount);
                            i += 1;
                        }
                    Selectable = true;
                }
                if (MoveDirection == "down")
                {
                        int i = 0;
                        foreach (GameObject item in MenuItems)
                        {
                            item.transform.position = lastPositions[i];
                            item.transform.Translate(Vector3.up * transformAmount);
                            i += 1;
                        }
                    Selectable = true;
                }

            }
        }
        if (MenuState == "Settings")
        {
            if (position < 3)
            {
                AdjustingVolume = true;
            }
            else
            {
                AdjustingVolume = false;
            }

            if (CrossInput("up") && Selectable && (position > 0) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                foreach (GameObject settings in SettingsItems)
                {
                    SettingsLastPositions[i] = SettingsItems[i].transform.position;
                    i += 1;
                }
                position -= 1;
                MoveDirection = "up";
                MoveTime = MovementTimer;
            }

            if (CrossInput("down") && Selectable && (position < maxPosition) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                foreach (GameObject settings in SettingsItems)
                {
                    SettingsLastPositions[i] = SettingsItems[i].transform.position;
                    i += 1;
                }
                position += 1;
                MoveDirection = "down";
                MoveTime = MovementTimer;
            }

            if (Selectable == false)
            {
                if (MoveDirection == "up")
                {
                        int i = 0;
                        foreach (GameObject settings in SettingsItems)
                        {
                            settings.transform.position = SettingsLastPositions[i];
                            settings.transform.Translate(Vector3.down * transformAmount);
                            i += 1;
                        }
                        Selectable = true;
                    
                }
                if (MoveDirection == "down")
                {
                        int i = 0;
                        foreach (GameObject settings in SettingsItems)
                        {
                            settings.transform.position = SettingsLastPositions[i];
                            settings.transform.Translate(Vector3.up * transformAmount);
                            i += 1;
                        }
                        Selectable = true;
                }

            }
        }

        if (MenuState == "Credits")
        {
            if (CrossInput("up") && Selectable && (position > 0) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                /*foreach (GameObject credits in CreditsItems)
                {
                    lastPositions[i] = MenuItems[i].transform.position;
                    i += 1;
                }*/
                position -= 1;
                MoveDirection = "up";
                MoveTime = MovementTimer;
            }

            if (CrossInput("down") && Selectable && (position < maxPosition) && Selectable == true && isSelecting == false)
            {
                Selectable = false;
                int i = 0;
                /*foreach (GameObject item in MenuItems)
                {
                    lastPositions[i] = MenuItems[i].transform.position;
                    i += 1;
                }*/
                position += 1;
                MoveDirection = "down";
                MoveTime = MovementTimer;
            }

            if (Selectable == false)
            {
                if (MoveDirection == "up")
                {
                    int i = 0;
                    foreach (GameObject credits in CreditsItems)
                    {
                        //credits.transform.position = lastPositions[i];
                        credits.transform.Translate(Vector3.down * transformAmount);
                        i += 1;
                    }
                    Selectable = true;
                }
                if (MoveDirection == "down")
                {
                    int i = 0;
                    foreach (GameObject credits in CreditsItems)
                    {
                        //credits.transform.position = lastPositions[i];
                        credits.transform.Translate(Vector3.up * transformAmount);
                        i += 1;
                    }
                    Selectable = true;
                }

            }
        }
    }

    void CreditsMenu()
    {
        if (ChangingMenus == true && MenuState == "MainToCredits")
        {
            position = 1;
            foreach (GameObject credits in CreditsItems)
            {
                credits.SetActive(true);
            }

            foreach (GameObject item in MenuItems)
            {
                item.SetActive(false);
            }
  
            ChangingMenus = false;
            MenuState = "Credits";

        }

        if (ChangingMenus == true && MenuState == "CreditsToMain")
        {
            Warning.SetActive(false);
            foreach (GameObject credits in CreditsItems)
            {
                credits.SetActive(false);
            }

            foreach (GameObject item in MenuItems)
            {
                item.SetActive(true);
            }
            maxPosition = (MenuItems.Length - 1);
            ChangingMenus = false;
            MenuState = "Main";
        }
    }

    void SettingsMenu()
    {
        if (ChangingMenus == true && MenuState == "MainToSettings")
        {
            foreach (GameObject item in MenuItems)
            {
                item.SetActive(false);
            }

            foreach (GameObject settings in SettingsItems)
            {
                settings.SetActive(true);
            }

                ChangingMenus = false;
                maxPosition = (SettingsItems.Length - 1);
                MenuState = "Settings";
        }


        if (ChangingMenus == true && MenuState == "SettingsToMain")
        {
            foreach (GameObject item in MenuItems)
            {
                item.SetActive(true);
            }

            foreach (GameObject settings in SettingsItems)
            {
                settings.SetActive(false);
            }

            ChangingMenus = false;
            maxPosition = (MenuItems.Length - 1);
            MenuState = "Main";
        }
    }


    void SettingsVolume()
    {
        if (Selectable == true)
        {

            if (CrossInput("right") && ChangingMenus == false && AdjustingVolume == true)
            {
                if (position == 0)
                {
                    MenuControl.MasterVolume += 10;
                    if (MenuControl.MasterVolume > 100)
                    {
                        MenuControl.MasterVolume = 100;
                    }
                    VolumeSlider.value = MenuControl.MasterVolume;
                    audioSource.volume = (MenuControl.MasterVolume * 0.01f);
                }
                if (position == 1)
                {
                    MenuControl.MusicVolume += 10;
                    if (MenuControl.MusicVolume > 100)
                    {
                        MenuControl.MusicVolume = 100;
                    }
                    MusicSlider.value = MenuControl.MusicVolume;
                }
                if (position == 2)
                {
                    MenuControl.SoundEffectVolume += 10;
                    if (MenuControl.SoundEffectVolume > 100)
                    {
                        MenuControl.SoundEffectVolume = 100;
                    }
                    SoundEffectSlider.value = MenuControl.SoundEffectVolume;
                    audioSource.PlayOneShot(SoundEffectTest, (MenuControl.SoundEffectVolume * 0.01f));
                }
            }

            if (CrossInput("left") && ChangingMenus == false && AdjustingVolume == true)
            {
                if (position == 0)
                {
                    MenuControl.MasterVolume -= 10;
                    if (MenuControl.MasterVolume < 0)
                    {
                        MenuControl.MasterVolume = 0;
                    }
                    VolumeSlider.value = MenuControl.MasterVolume;
                    audioSource.volume = (MenuControl.MasterVolume * 0.01f);
                }
                if (position == 1)
                {
                    MenuControl.MusicVolume -= 10;
                    if (MenuControl.MusicVolume < 0)
                    {
                        MenuControl.MusicVolume = 0;
                    }
                    MusicSlider.value = MenuControl.MusicVolume;
                }
                if (position == 2)
                {
                    MenuControl.SoundEffectVolume -= 10;
                    if (MenuControl.SoundEffectVolume < 0)
                    {
                        MenuControl.SoundEffectVolume = 0;
                    }
                    SoundEffectSlider.value = MenuControl.SoundEffectVolume;
                    audioSource.PlayOneShot(SoundEffectTest, (MenuControl.SoundEffectVolume * 0.01f));
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        VisualButtonControls();
        VisualHighlightControls();
        VisualMenuControls();
        CreditsMenu();
        SettingsMenu();
        SettingsVolume();

        //Controls what happens on each menu item.
        if (Confirmed)
        {
            if (MenuState == "Main")
            {
                if (position == 1)
                {
                    Pause.UnPauseGame();
                    Destroy(this.gameObject);
                }

                if (position == 2)
                {
                    MenuState = "MainToCredits";
                    ChangingMenus = true;
                    Warning.SetActive(true);
                    foreach (GameObject credits in CreditsItems)
                    {
                        credits.SetActive(true);
                    }
                    foreach (GameObject item in MenuItems)
                    {
                        item.SetActive(false);
                    }
                    maxPosition = (CreditsItems.Length - 1);
                }

                if (position == 0)
                {
                    MenuState = "MainToSettings";
                    ChangingMenus = true;
                    foreach (GameObject settings in SettingsItems)
                    {
                        settings.SetActive(true);
                    }
                    foreach (GameObject item in MenuItems)
                    {
                        item.SetActive(false);
                    }
                }
            }

            if (MenuState == "Credits")
            {
                if (position == 0)
                {
                    Application.Quit();
                }
                if (position == 1)
                {
                    ChangingMenus = true;
                    MenuState = "CreditsToMain";
                    position = 2;
                    maxPosition = (MenuItems.Length - 1);
                }
            }

            if (MenuState == "Settings")
            {
                if (position == 3)
                {
                    ChangingMenus = true;
                    MenuState = "SettingsToMain";
                    position = 0;
                    maxPosition = (MenuItems.Length - 1);
                    SettingsItems[0].transform.position = MasterCenterTarget.transform.position;
                    SettingsItems[1].transform.position = MusicCenterTarget.transform.position;
                    SettingsItems[2].transform.position = SoundCenterTarget.transform.position;
                    SettingsItems[3].transform.position = SettingsBackCenterTarget.transform.position;
                }
            }
        }

        Confirmed = false;
    }
}
