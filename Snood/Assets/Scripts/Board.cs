using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Board : MonoBehaviour {

    //BASICS
    private const int FULL_LINE = 10;
    private const int START_X_FULL = -315, START_X_NON_FULL = -280;
    private const int stepX = 70, stepY = 61;
    private int startY;


    //COLORS
    private Color[] colors = { Color.red, Color.green, Color.blue, Color.magenta, Color.yellow, Color.cyan };   // 0 - 5 inclusive
    private bool[] availableColors = new bool[6];
    

    //GAMEOBJECTS
    public GameManager gameManager;
    public Transform upCollider;

    //public GameObject EmptyObject;
    public GameObject BubblePrefab;
    public GameObject BubbleLine;


    private Bubble newBubble;   // reference to the last placed bubble

    //BOARD_MOVE
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private const int moveTime = 5;

    //DATA STORE
    private List<List<Bubble>> listObjects;


    public int createBoard(List<List<int>> listIntegers)
    {
        int bubbleCounter = 0;

        listObjects = new List<List<Bubble>>();

        for (int i = 0; i < listIntegers.Count; i++)
        {
            List<Bubble> line = new List<Bubble>();

            for (int j = 0; j < listIntegers[i].Count; j++)
            {
                line.Add(Instantiate(BubblePrefab, this.transform, false).GetComponent<Bubble>());

                //TYPE
                if (listIntegers[i][j] != 0)
                {
                    //COLOR
                    line[line.Count - 1].setColor(colors[listIntegers[i][j] - 1]);
                    availableColors[listIntegers[i][j] - 1] = true;

                    bubbleCounter++;
                }
                else
                    line[line.Count - 1].setEmpty();


                Vector2 myPos;

                //POSITION
                if (i % 2 == 0)
                    myPos = new Vector2(START_X_FULL + j * stepX, startY - i * stepY);
                else
                    myPos = new Vector2(START_X_NON_FULL + j * stepX, startY - i * stepY);

                line[line.Count - 1].setPosition(myPos);


                //COORDINATES
                line[line.Count - 1].setCoordinates(i, j);
            }

            listObjects.Add(line);

        }

        newLine();

        upCollider.transform.localPosition = new Vector3(0, listObjects[0][0].transform.localPosition.y + BubblePrefab.GetComponent<CircleCollider2D>().radius + upCollider.GetComponent<RectTransform>().rect.height / 2, 0);


        return bubbleCounter;
    }

    public int countBubbles()
    {
        int counter = 0;
        foreach (List<Bubble> list in listObjects)
            foreach (Bubble bubble in list)
                if (bubble.isBubble())
                    counter++;

        return counter;
    }

    void Update()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPosition, ref velocity, moveTime * Time.deltaTime);
    }

    public void setStartY(int x)
    {
        startY = x;
    }

    public bool noMoreBubbles()
    {
        for (int i = 0; i < listObjects[0].Count; i++)
            if (!listObjects[0][i].isEmpty())
                return false;   //no WIN if bubble is still available

        return true;            //WIN if no bubble is still available
    }


    //BOARD POSITION
    public int lowestBubbleY()
    {
        for (int i = listObjects.Count - 1; i >= 0; i--)
            for (int j = 0; j < listObjects[i].Count; j++)
                if (listObjects[i][j].isBubble())
                    return i;

        return -1;
    }

    public int lowestEmptyY()
    {
        for (int i = listObjects.Count - 1; i >= 0; i--)
            for (int j = 0; j < listObjects[i].Count; j++)
                if (listObjects[i][j].isEmpty())
                    return i;

        return -1;
    }

    public void autoMoveBoard()
    {
        int lowest_bubble_Y = lowestBubbleY();
        int lowest_empty_Y = lowestEmptyY();

        float lowestPoint = listObjects[lowest_bubble_Y][0].transform.localPosition.y;
        if ((int)lowestPoint > 0)
            lowestPoint = 0;

        targetPosition = new Vector2(0, -lowestPoint);

        if (lowest_bubble_Y == lowest_empty_Y)
            newLine();
    }


    public void newLine()
    {
        List<Bubble> lastline = new List<Bubble>();
        int myStartX;

        for (int i = 0; i < FULL_LINE - 1; i++)
        {
            Bubble tempBubble = Instantiate(BubblePrefab, this.transform).GetComponent<Bubble>();
            tempBubble.setEmpty();
            lastline.Add(tempBubble);
        }


        if (listObjects[listObjects.Count - 1].Count == FULL_LINE - 1)
        {
            Bubble tempBubble = Instantiate(BubblePrefab, this.transform).GetComponent<Bubble>();
            tempBubble.setEmpty();
            lastline.Add(tempBubble);
            myStartX = START_X_FULL;
        }
        else
            myStartX = START_X_NON_FULL;


        for (int i = 0; i < lastline.Count; i++)
        {
            Vector2 pos = new Vector2(myStartX + i * stepX, listObjects[listObjects.Count - 1][0].transform.localPosition.y - stepY);
            lastline[i].setPosition(pos);
            lastline[i].setCoordinates(listObjects.Count, i);
        }

        listObjects.Add(lastline);

    }

    public void placeNearest(GameObject throwBubble)
    {
        newBubble = getNearestEmpty(throwBubble);
        newBubble.setBubble();
        newBubble.setColor(throwBubble.GetComponent<Image>().color);
    }

    private Bubble getNearestEmpty(GameObject throwBubble)
    {
        Bubble toRet = null;
        float minDistance = float.MaxValue;

        foreach (List<Bubble> list in listObjects)
            foreach (Bubble bubble in list)
            {
                if (bubble.isEmpty())
                {
                    float distance = Vector3.Distance(bubble.getPosition(), throwBubble.transform.localPosition);
                    if (distance < minDistance) //shortest distance
                    {
                        minDistance = distance;
                        toRet = bubble;
                    }
                }
            }

        return toRet;
    }


    public void resetStates()
    {
        foreach (List<Bubble> list in listObjects)
            foreach (Bubble bubble in list)
                bubble.setState(global::Bubble.State.unvisited);
    }


    //GET BUBBLES
    public List<Bubble> getSameColorAdjacentBubbles()
    {
        Color targetColor = newBubble.GetComponent<Image>().color;
        List<Bubble> myList = new List<Bubble>();
        newBubble.setState(global::Bubble.State.visited);
        myList.Add(newBubble);
        int counter = 0;

        while (counter < myList.Count)
        {
            List<Bubble> adjacent = new List<Bubble>();
            adjacent.AddRange(getAdjacent(myList[counter]));

            foreach (Bubble bubble in adjacent)
                if (bubble.isBubble() &&
                    bubble.getState() == global::Bubble.State.unvisited &&
                    bubble.getColor() == targetColor)
                {
                    bubble.setState(global::Bubble.State.visited);
                    myList.Add(bubble);
                }

            counter++;

        }

        resetStates();

        return myList;
    }

    public List<Bubble> getNotSupportedBubbles()
    {
        List<Bubble> supported = new List<Bubble>();

        for (int i = 0; i < listObjects[0].Count; i++)
            if (listObjects[0][i].isBubble())
            {
                listObjects[0][i].setState(global::Bubble.State.visited);
                supported.Add(listObjects[0][i]);
            }

        int counter = 0;

        while (counter < supported.Count)
        {
            List<Bubble> adjacentBubbles = new List<Bubble>();
            adjacentBubbles.AddRange(getAdjacent(supported[counter]));

            foreach (Bubble bubble in adjacentBubbles)
                if (bubble.isBubble() &&
                    bubble.getState() == global::Bubble.State.unvisited)
                {
                    bubble.setState(global::Bubble.State.visited);
                    supported.Add(bubble);
                }

            counter++;
        }

        List<Bubble> diff = new List<Bubble>();

        foreach (List<Bubble> list in listObjects)
            foreach (Bubble bubble in list)
                if (bubble.isBubble() &&
                    bubble.getState() == global::Bubble.State.unvisited)
                    diff.Add(bubble);

        resetStates();

        return diff;
    }
    
    private List<Bubble> getAdjacent(Bubble myBubble)
    {
        int[] arr = myBubble.getCoordinates();
        int i = arr[0];
        int j = arr[1];

        List<Bubble> toRet = new List<Bubble>();

        List<Bubble> myLine = listObjects[i];
        List<Bubble> upLine = null, downLine = null;

        if (i-1>=0)
            upLine = listObjects[i-1];
        if (i+1<listObjects.Count)
            downLine = listObjects[i+1];

        //FULL LINE

        if(myLine.Count == FULL_LINE)
        {
            //myLine
            if (j - 1 >= 0) toRet.Add(myLine[j - 1]);

            if (j + 1 < myLine.Count) toRet.Add(myLine[j + 1]);

            //upLine
            if(upLine != null)
            {
                if (j - 1 >= 0) toRet.Add(upLine[j - 1]);
                if (j < upLine.Count) toRet.Add(upLine[j]); 
            }

            //downLine
            if (downLine != null)
            {
                if (j - 1 >= 0) toRet.Add(downLine[j - 1]);
                if (j < downLine.Count) toRet.Add(downLine[j]);
            }

        }

        else
        {
            //myLine
            if (j - 1 >= 0) toRet.Add(myLine[j - 1]);

            if (j + 1 < myLine.Count) toRet.Add(myLine[j + 1]);

            //upLine
            if (upLine != null)
            {
                toRet.Add(upLine[j]);
                if (j + 1 < upLine.Count) toRet.Add(upLine[j + 1]);
            }

            //downLine
            if (downLine != null)
            {
                toRet.Add(downLine[j]);
                if (j + 1 < downLine.Count) toRet.Add(downLine[j + 1]);
            }

        }

        return toRet;

    }

    
    //COLORS
    public Color getOneAvailableColor()
    {
        //0-3   =   GET FROM THE LAST THREE LINES
        //4     =   GET RANDOM

        Color myColor;

        int select = UnityEngine.Random.Range(0, 5);

        if(select <= 3) // FROM THE LAST THREE LINES - LOWER => HIGHEST PROBABILITY
        {
            List<Color> colorList = new List<Color>();

            int lastLine = lowestBubbleY();
            int firstLine = lastLine >= 3 ? lastLine - 3 : 0;

            for (int i = lastLine; i >= firstLine; i--)
                for (int j = 0; j < listObjects[i].Count; j++)
                    if (listObjects[i][j].isBubble())
                        for (int k = 0; k <= i - firstLine; k++)     // 
                            colorList.Add(listObjects[i][j].GetComponent<Image>().color);


            int num = UnityEngine.Random.Range(0, colorList.Count);

            myColor = colorList[num];
        }

        else // FROM THE REST LINES - MORE APPEARS => HIGHEST PROBABILITY
        {
            List<Color> colorList = new List<Color>();

            int lastLine = lowestBubbleY();

            for(int i = 0; i <= lastLine; i++)
                for (int j = 0; j < listObjects[i].Count; j++)
                    if (listObjects[i][j].isBubble())
                        colorList.Add(listObjects[i][j].GetComponent<Image>().color);

            int num = UnityEngine.Random.Range(0, colorList.Count);

            myColor = colorList[num];
        }

        return myColor;
    }

    public void changeAvailableColors()
    {
        for (int i = 0; i < colors.Length; i++)
            if (!isThisColorAvailable(colors[i]))
                availableColors[i] = false;
    }

    public bool isThisColorAvailable(Color myColor)
    {
        foreach (List<Bubble> myList in listObjects)
            foreach (Bubble myGO in myList)
                if (myGO.isBubble() && myGO.GetComponent<Image>().color == myColor)
                    return true;

        return false;
    }

}
