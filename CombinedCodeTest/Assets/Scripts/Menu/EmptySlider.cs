using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmptySlider : MonoBehaviour
{
    private Slider Amount;
    private GameObject Fill;
    public string ChildFillArea;

    private void Start()
    {
        Amount = this.gameObject.GetComponent<Slider>();
        Fill = GameObject.Find(ChildFillArea);
    }
    // Update is called once per frame
    void Update()
    {
        if (Amount.value <= 0)
        {
            Fill.SetActive(false);
        }
        else
        {
            Fill.SetActive(true);
        }
    }
}
