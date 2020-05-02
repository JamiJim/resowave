using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
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

    public float SinModifier = 2f;
    private float modifier;
    private bool isSelecting = false;
    private bool changeInSelection = false;

    private int position = 1;
    private int maxPosition;

    public bool Selectable = true;
    public float MovementTimer = 1.0f;
    private float MoveTime;
    public float SelectTimer = 0;
    public GameObject ConfirmText;
    private float transformAmount = 191;
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
    public static float MasterVolume = 100;
    public static float MusicVolume = 100;
    public static float SoundEffectVolume = 100;
    private Vector3[] SettingsLastPositions;
    public GameObject[] SettingsFirstPositions;
    public bool AdjustingVolume = false;
    public Slider VolumeSlider;
    public Slider MusicSlider;
    public Slider SoundEffectSlider;
    public float SelectionTime;

    private VZController Controller;

    public AudioSource audioSource;
    public AudioClip SoundEffectTest;
    private float SoundEffectTimer = 0f;
    private CreditsScroll CreditsText;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        maxPosition = MenuItems.Length - 1;
        VisualButton = ButtonDisplay.GetComponent<Image>();
        SelectionSlider = VisualSelection.GetComponentInChildren<Slider>();
        Controller = GameObject.FindGameObjectWithTag("VZController").GetComponent<VZController>();
        CreditsText = CreditsItems[0].GetComponentInChildren<CreditsScroll>();
        modifier = (Mathf.Sin(SinModifier) * Time.deltaTime);
        int i = 0;
        int j = 0;
        VolumeSlider.value = MasterVolume;
        MusicSlider.value = MusicVolume;
        SoundEffectSlider.value = SoundEffectVolume;
        lastPositions = new Vector3[MenuItems.Length];
        SettingsLastPositions = new Vector3[SettingsItems.Length];
        foreach (GameObject item in MenuItems)
        {
            lastPositions[i] = MenuItems[i].transform.position;
            i += 1;
        }
        foreach (GameObject settings in SettingsItems)
        {
            SettingsLastPositions[j] = SettingsItems[j].transform.position;
            j += 1;
        }
    }

    bool CrossInput(string direction)
    {
        if (direction == "up")
        {
            if (Input.GetKey(KeyCode.UpArrow) || Controller.DpadUp.Down)
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
            if (Input.GetKey(KeyCode.DownArrow) || Controller.DpadDown.Down)
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
            if (Input.GetKey(KeyCode.LeftArrow) || Controller.DpadLeft.Down)
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
            if (Input.GetKey(KeyCode.RightArrow) || Controller.DpadRight.Down)
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
        if (MenuState == "Main" || (MenuState == "Settings" && position == 3))
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

        if (MenuState == "MainToCredits" || MenuState == "Credits" || MenuState == "CreditsToMain" || MenuState == "MainToSettings" || MenuState == "SettingsToMain")
        {

            if (CrossInput("up") && !CrossInput("down"))
            {
                VisualButton.sprite = ButtonCreditsUp;
            }

            if (CrossInput("down") && !CrossInput("up"))
            {
                VisualButton.sprite = ButtonCreditsDown;
            }

            if (CrossInput("right") && !CrossInput("left") && (isSelecting == true || ChangingMenus == false))
            {
                VisualButton.sprite = ButtonCreditsRight;
            }

            if (CrossInput("right") && !CrossInput("left") && (isSelecting == false || ChangingMenus == true))
            {
                VisualButton.sprite = ButtonCreditsRightInactive;
            }

            if (CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonCreditsLeft;
            }

            if (!CrossInput("up") && !CrossInput("down") && !CrossInput("right") && !CrossInput("left") && !CrossInput("right"))
            {
                VisualButton.sprite = ButtonCreditsNoInput;
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
                isSelecting = false;
            }

            if (ChangingMenus == true)
            {
                isSelecting = false;
            }

            if (isSelecting == false)
            {
                SelectionSlider.value = 0;
                ConfirmText.SetActive(false);
            }

            if (CrossInput("right") && ChangingMenus == false)
            {
                if (AdjustingVolume == false)
                {
                    isSelecting = true;
                }
            }

            if (isSelecting == true && ChangingMenus == false)
            {
                SelectionSlider.value += ((SelectionSlider.maxValue / SelectionTime) * Time.deltaTime);
                ConfirmText.SetActive(true);
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
                if (MoveDirection == "up" && MoveTime > 0)
                {
                    foreach (GameObject item in MenuItems)
                    {
                        item.transform.Translate(Vector3.down * Time.deltaTime * SlideAmount);
                        MoveTime -= Time.deltaTime;
                    }
                    if (MoveTime <= 0)
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
                }
                if (MoveDirection == "down" && MoveTime > 0)
                {
                    foreach (GameObject item in MenuItems)
                    {
                        item.transform.Translate(Vector3.up * Time.deltaTime * SlideAmount);
                        MoveTime -= Time.deltaTime;
                    }
                    if (MoveTime <= 0)
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
                if (MoveDirection == "up" && MoveTime > 0)
                {
                    foreach (GameObject settings in SettingsItems)
                    {
                        settings.transform.Translate(Vector3.down * Time.deltaTime * SlideAmount);
                        MoveTime -= Time.deltaTime;
                    }
                    if (MoveTime <= 0)
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
                }
                if (MoveDirection == "down" && MoveTime > 0)
                {
                    foreach (GameObject settings in SettingsItems)
                    {
                        settings.transform.Translate(Vector3.up * Time.deltaTime * SlideAmount);
                        MoveTime -= Time.deltaTime;
                    }
                    if (MoveTime <= 0)
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
        }
    }

    void CreditsMenu()
    {
        if (ChangingMenus == true && MenuState == "MainToCredits" && MoveTime > 0)
        {
            VisualSelection.transform.Translate(Vector3.down * Time.deltaTime * SelectionMoveAmount);
            foreach (GameObject credits in CreditsItems)
            {
                credits.transform.Translate(Vector3.left * Time.deltaTime * SlideAmount * 3);
            }
            MoveTime -= Time.deltaTime;

            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.left * Time.deltaTime * SlideAmount * 3);
            }

            if (MoveTime <= 0)
            {
                int i = 0;

                CreditsItems[0].transform.position = CenterTarget.transform.position;
                CreditsItems[1].transform.position = BackCenterTarget.transform.position;
                VisualSelection.transform.position = BackCenterTarget.transform.position;
                ChangingMenus = false;
                MenuState = "Credits";
            }
        }

        if (ChangingMenus == true && MenuState == "CreditsToMain" && MoveTime > 0)
        {
            VisualSelection.transform.Translate(Vector3.up * Time.deltaTime * SelectionMoveAmount);
            foreach (GameObject credits in CreditsItems)
            {
                credits.transform.Translate(Vector3.right * Time.deltaTime * SlideAmount * 3);
            }
            MoveTime -= Time.deltaTime;

            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.right * Time.deltaTime * SlideAmount * 3);
            }

            if (MoveTime <= 0)
            {
                int i = 0;
                foreach (GameObject item in MenuItems)
                {
                    item.transform.position = lastPositions[i];
                    i += 1;
                }

                VisualSelection.transform.position = MenuItems[2].transform.position;

                foreach (GameObject credits in CreditsItems)
                {
                    CreditsText.ResetCredits();
                    credits.SetActive(false);
                }
                ChangingMenus = false;
                MenuState = "Main";
            }
        }
    }

    void SettingsMenu()
    {
        if (ChangingMenus == true && MenuState == "MainToSettings" && MoveTime > 0)
        {
            foreach (GameObject settings in SettingsItems)
            {
                settings.transform.Translate(Vector3.down * Time.deltaTime * SlideAmount * 3);
            }
            MoveTime -= Time.deltaTime;

            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.down * Time.deltaTime * SlideAmount * 3);
            }

            if (MoveTime <= 0)
            {
                int i = 0;
                SettingsItems[0].transform.position = MasterCenterTarget.transform.position;
                SettingsItems[1].transform.position = MusicCenterTarget.transform.position;
                SettingsItems[2].transform.position = SoundCenterTarget.transform.position;
                SettingsItems[3].transform.position = SettingsBackCenterTarget.transform.position;
                ChangingMenus = false;
                maxPosition = (SettingsItems.Length - 1);
                MenuState = "Settings";
            }
        }

        if (ChangingMenus == true && MenuState == "SettingsToMain" && MoveTime > 0)
        {
            foreach (GameObject settings in SettingsItems)
            {
                settings.transform.Translate(Vector3.up * Time.deltaTime * SlideAmount * 3);
            }
            MoveTime -= Time.deltaTime;

            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.up * Time.deltaTime * SlideAmount * 3);
            }

            if (MoveTime <= 0)
            {
                int i = 0;
                foreach (GameObject item in MenuItems)
                {
                    item.transform.position = lastPositions[i];
                    SettingsItems[i].transform.position = SettingsFirstPositions[i].transform.position;
                    i += 1;
                }

                foreach (GameObject settings in SettingsItems)
                {
                    settings.SetActive(false);
                }
                ChangingMenus = false;
                MenuState = "Main";
            }
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
                    MasterVolume += Time.deltaTime * 30;
                    if (MasterVolume > 100)
                    {
                        MasterVolume = 100;
                    }
                    VolumeSlider.value = MasterVolume;
                    audioSource.volume = (MasterVolume * 0.01f);
                }
                if (position == 1)
                {
                    MusicVolume += Time.deltaTime * 30;
                    if (MusicVolume > 100)
                    {
                        MusicVolume = 100;
                    }
                    MusicSlider.value = MusicVolume;
                }
                if (position == 2)
                {
                    SoundEffectVolume += Time.deltaTime * 30;
                    if (SoundEffectVolume > 100)
                    {
                        SoundEffectVolume = 100;
                    }
                    SoundEffectTimer -= Time.deltaTime;
                    if (SoundEffectTimer <= 0)
                    {
                        audioSource.PlayOneShot(SoundEffectTest, (MenuControl.SoundEffectVolume * 0.01f));
                        SoundEffectTimer = 0.5f;
                    }
                    SoundEffectSlider.value = SoundEffectVolume;
                }
            }

            if (CrossInput("left") && ChangingMenus == false && AdjustingVolume == true)
            {
                if (position == 0)
                {
                    MasterVolume -= Time.deltaTime * 30;
                    if (MasterVolume < 0)
                    {
                        MasterVolume = 0;
                    }
                    VolumeSlider.value = MasterVolume;
                    audioSource.volume = (MasterVolume * 0.01f);
                }
                if (position == 1)
                {
                    MusicVolume -= Time.deltaTime * 30;
                    if (MusicVolume < 0)
                    {
                        MusicVolume = 0;
                    }
                    MusicSlider.value = MusicVolume;
                }
                if (position == 2)
                {
                    SoundEffectVolume -= Time.deltaTime * 30;
                    if (SoundEffectVolume < 0)
                    {
                        SoundEffectVolume = 0;
                    }
                    SoundEffectSlider.value = SoundEffectVolume;
                    SoundEffectTimer -= Time.deltaTime;
                    if (SoundEffectTimer <= 0)
                    {
                        audioSource.PlayOneShot(SoundEffectTest, (MenuControl.SoundEffectVolume * 0.01f));
                        SoundEffectTimer = 0.5f;
                    }
                }
            }
        }
        if (!CrossInput("left") && (!CrossInput("right")))
        {
            SoundEffectTimer = 0;
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
        if (SelectionSlider.value >= SelectionSlider.maxValue)
        {
            if (MenuState == "Main")
            {
                if (position == 1)
                {
                    SceneManager.LoadScene("Test+Deletion");
                }

                if (position == 2)
                {
                    MenuState = "MainToCredits";
                    ChangingMenus = true;
                    MoveTime = MovementTimer;

                    foreach (GameObject credits in CreditsItems)
                    {
                        credits.SetActive(true);
                    }

                    int i = 0;
                    foreach (GameObject item in MenuItems)
                    {
                        lastPositions[i] = MenuItems[i].transform.position;
                        i += 1;
                    }
                }

                if (position == 0)
                {
                    MenuState = "MainToSettings";
                    ChangingMenus = true;
                    MoveTime = MovementTimer;
                    foreach (GameObject settings in SettingsItems)
                    {
                        settings.SetActive(true);
                    }

                    int i = 0;
                    foreach (GameObject item in MenuItems)
                    {
                        lastPositions[i] = MenuItems[i].transform.position;
                        i += 1;
                    }
                }

                if (position == 3)
                {
                    Application.Quit();
                }
            }

            if (MenuState == "Credits")
            {
                ChangingMenus = true;
                MenuState = "CreditsToMain";
                MoveTime = MovementTimer;
            }

            if (MenuState == "Settings")
            {
                if (position == 3)
                {
                    ChangingMenus = true;
                    MenuState = "SettingsToMain";
                    MoveTime = MovementTimer;
                    position = 0;
                    maxPosition = (MenuItems.Length - 1);
                }
            }
        }

        
    }
}
