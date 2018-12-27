using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class SecondThrowBubble : MonoBehaviour {

    private Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void setColor(Color newColor)
    {
        img.color = newColor;
    }

    public Color getColor()
    {
        return img.color;
    }
    
    public void showBubble(bool a)
    {
        img.enabled = a;
    }
    
}
