using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleSystem : MonoBehaviour {

    public GameManager myManager;
    public PauseSystem myPause;
    public Board myBoard;
    public Button toggleButton;

    public FirstThrowBubble myFirstBubble;
    public SecondThrowBubble mySecondBubble;

    public Line myLine;

    public Text BubblesText;
    private int bubblesNum = 0;

    private bool canClick;
    private float downBound;
    private float upBound;

    void Start()
    {
        // SECOND BALL COLOR SET => FIRST = SECOND , SECOND = newColor
        Color secondColor = myBoard.getOneAvailableColor();
        mySecondBubble.setColor(secondColor);

        toggleButton.onClick.AddListener(toggleBubbles);

        BubblesText.text = bubblesNum + "";

        downBound = myFirstBubble.getPosition().y + myFirstBubble.getRay();
        upBound = myManager.getUpWallDownPosY();
    }

    private bool goodClick = false;

    void Update()
    {
        Vector2 click = myBoard.transform.parent.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));    // myBoard.transform = canvas
        Vector2 forceVector = (click - myFirstBubble.getPosition()).normalized;

        if (canClick && !myPause.getPaused())
        {
            if (Input.GetMouseButtonDown(0) && click.y <= upBound)    // DOWN
                goodClick = true;
            else if (goodClick)
            {
                if (Input.GetMouseButton(0))   // PRESS
                {
                    if (click.y >= downBound)
                        myLine.placeLine(forceVector);
                    else
                        myLine.disableLine();
                }

                else if (Input.GetMouseButtonUp(0)) // UP
                {
                    myLine.disableLine();
                    goodClick = false;

                    if (click.y >= downBound)
                    {
                        canClick = false;
                        myFirstBubble.setVelocity(forceVector);
                        printRemainingAfterShoot();
                    }
                }
            }
        }
    }


    void toggleBubbles()
    {
        if (canClick && bubblesNum > 1)
        {
            Color temp = myFirstBubble.getColor();
            myFirstBubble.setColor(mySecondBubble.getColor());
            mySecondBubble.setColor(temp);
        }
    }

    public void setInitialConditions()
    {
        myFirstBubble.setStartPosition();

        Color firstColor = mySecondBubble.getColor();
        if (!myBoard.isThisColorAvailable(firstColor))
            firstColor = myBoard.getOneAvailableColor();
        myFirstBubble.setColor(firstColor);

        Color secondColor = myBoard.getOneAvailableColor();
        mySecondBubble.setColor(secondColor);


        myFirstBubble.setCollider(true);

        if (bubblesNum >= 1)
        {
            myFirstBubble.showBubble(true);
            canClick = true;
        }
        else
            canClick = false;
        if (bubblesNum > 1)
            mySecondBubble.showBubble(true);
        else
            mySecondBubble.showBubble(false);

    }
    

    void printRemainingAfterShoot()
    {
        BubblesText.text = --bubblesNum + "";
    }

    public int getRemainingBubbles()
    {
        return bubblesNum;
    }

    public void setBubblesNum(int x)
    {
        bubblesNum = x;
    }

}
