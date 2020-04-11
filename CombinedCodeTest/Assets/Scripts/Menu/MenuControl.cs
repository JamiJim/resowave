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

    public GameObject VisualSelection;
    private Image SelectionEffect;
    private Color SelectionColor;

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
    

    // Start is called before the first frame update
    void Start()
    {
        maxPosition = MenuItems.Length - 1;
        VisualButton = ButtonDisplay.GetComponent<Image>();
        SelectionEffect = VisualSelection.GetComponent<Image>();
        SelectionColor = SelectionEffect.material.color;
        modifier = (Mathf.Sin(SinModifier) * Time.deltaTime);
        int i = 0;
        lastPositions = new Vector3[MenuItems.Length];
        foreach (GameObject item in MenuItems)
        {
            lastPositions[i] = MenuItems[i].transform.position;
            i += 1;
        }
    }

    void VisualButtonControls()
    {
        if (Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            VisualButton.sprite = ButtonUp;
        }

        if (Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.UpArrow))
        {
            VisualButton.sprite = ButtonDown;
        }

        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && isSelecting == true)
        {
            VisualButton.sprite = ButtonRight;
        }

        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && isSelecting == false)
        {
            VisualButton.sprite = ButtonRightInactive;
        }

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            VisualButton.sprite = ButtonLeft;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            VisualButton.sprite = ButtonNoInput;
        }
    }

    void VisualHighlightControls()
    {
        if (Selectable == true)
        {
            if (!Input.GetKey(KeyCode.RightArrow))
            {
                isSelecting = false;
            }

            if (isSelecting == false)
            {
                SelectionColor.r = 1;
                SelectionColor.g = 0.258f;
                SelectionColor.b = 0.862f;
                SelectionColor.a = 0.4f;
                ConfirmText.SetActive(false);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                isSelecting = true;
            }

            if (isSelecting == true)
            {
                SelectionColor.a += Time.deltaTime * 0.5f;
                SelectionColor.r = 0;
                SelectionColor.g = 1;
                SelectionColor.b = 0;
                ConfirmText.SetActive(true);
            }
            SelectionEffect.color = SelectionColor;
        }
    }

    void VisualMenuControls()
    {
        if (Input.GetKey(KeyCode.UpArrow) && Selectable && (position > 0) && Selectable == true && isSelecting == false)
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

        if (Input.GetKey(KeyCode.DownArrow) && Selectable && (position < maxPosition) && Selectable == true && isSelecting == false)
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

    // Update is called once per frame
    void Update()
    {
        VisualButtonControls();
        VisualHighlightControls();
        VisualMenuControls();

        //Controls what happens on each menu item.
        if (SelectionColor.a >= 1)
        {
            if (position == 1)
            {
                SceneManager.LoadScene("Test+Deletion");
            }

        }
    }
}
