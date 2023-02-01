using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerTipsAnimation : MonoBehaviour
{
    private RectTransform Back;

    private bool IsDown = true;
    // Start is called before the first frame update
    void Start()
    {
        Back = transform.GetChild(0).GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDown)
        {
            var pos = Back.anchoredPosition;
            pos = new Vector2(0,pos.y-Time.deltaTime * 8);
            if (pos.y <= -4)
                IsDown = false;
            Back.anchoredPosition = pos;
        }
        else
        {
            var pos = Back.anchoredPosition;
            pos = new Vector2(0,pos.y+Time.deltaTime * 8);
            if (pos.y >= 0)
                IsDown = true;
            Back.anchoredPosition = pos;
        }
    }
}
