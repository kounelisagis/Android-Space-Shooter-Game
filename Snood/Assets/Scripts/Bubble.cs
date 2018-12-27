using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour{

    // Used for BFS
    public enum State { unvisited, visited };
    private State myState = State.unvisited;


    private int myI, myJ;
    private Color myColor;


    private Vector2 startPos;

    private CircleCollider2D myCollider;
    private Image myImage;


    void Awake()
    {
        myImage = GetComponent<Image>();
        myCollider = GetComponent<CircleCollider2D>();
    }

    public bool isBubble()
    {
        return myImage.enabled;
    }

    public void setBubble()
    {
        myImage.enabled = true;
        myCollider.enabled = true;
    }


    public bool isEmpty()
    {
        return !myImage.enabled;
    }

    public void setEmpty()
    {
        myImage.enabled = false;
        myCollider.enabled = false;
    }


    public void setColor(Color newColor)
    {
        this.gameObject.GetComponent<Image>().color = newColor;
        myColor = newColor;
    }

    public Color getColor()
    {
        return myColor;
    }


    public State getState()
    {
        return myState;
    }

    public void setState(State newState)
    {
        myState = newState;
    }


    public void setCoordinates(int i, int j)
    {
        myI = i;
        myJ = j;
    }

    public int[] getCoordinates()
    {
        return new int[] { myI, myJ };
    }


    public Vector2 getPosition()
    {
        return this.transform.localPosition;
    }

    public void setPosition(Vector2 myPos)
    {
        startPos = myPos;
        this.transform.localPosition = myPos;
    }
    

    public void setCollider(bool a)
    {
        myCollider.enabled = a;
    }


    //FOR RESET
    private void Update()
    {
        if (transform.localPosition.y < -Screen.height - 100)
        {
            setEmpty();
            gameObject.transform.localPosition = startPos;
        }

    }
}
