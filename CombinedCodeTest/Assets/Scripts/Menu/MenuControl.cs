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

    public GameObject VisualSelection;
    private Image SelectionEffect;
    private Color SelectionColor;

    public float SinModifier = 2f;
    private float modifier;
    private bool isSelecting = false;
    private bool changeInSelection = false;

    private int position = 1;
    private int maxPosition;

    private bool Selectable = true;
    public float MovementTimer = 1.0f;
    private float MovementAmount;
    public float SelectTimer = 0;
    public GameObject ConfirmText;
    public float transformAmount = 362;

    // Start is called before the first frame update
    void Start()
    {
        maxPosition = MenuItems.Length;
        VisualButton = ButtonDisplay.GetComponent<Image>();
        SelectionEffect = VisualSelection.GetComponent<Image>();
        SelectionColor = SelectionEffect.material.color;
        modifier = (Mathf.Sin(SinModifier) * Time.deltaTime);
        MovementAmount = MenuItems[0].transform.position.y;
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

        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            VisualButton.sprite = ButtonRight;
        }

        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            VisualButton.sprite = ButtonNoInput;
        }
    }

    void VisualSelectionControls()
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
                SelectionColor.a += Time.deltaTime;
                SelectionColor.r = 0;
                SelectionColor.g = 1;
                SelectionColor.b = 0;
                ConfirmText.SetActive(true);
            }
            SelectionEffect.color = SelectionColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        VisualButtonControls();
        VisualSelectionControls();

        if (Input.GetKeyDown(KeyCode.UpArrow) && Selectable && (position < maxPosition))
        {
            //Selectable = false; //191
            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.down * transformAmount);
                position += 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && Selectable && (position >= 0))
        {
            //Selectable = false; //191
            foreach (GameObject item in MenuItems)
            {
                item.transform.Translate(Vector3.up * transformAmount);
                position -= 1;
            }
        }

        if (position == 1 && SelectionColor.a >= 1)
        {
            SceneManager.LoadScene("Test+Deletion");
        }

    }
}
