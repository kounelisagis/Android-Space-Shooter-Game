using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Line : MonoBehaviour {

    public Canvas myCanvas;

    public FirstThrowBubble myFirstBubble;

    public GameObject lineBubble;

    private GameObject[] lineBubbles;


    private float R;
    private float r;
    private float distance = 0;
    private const float maxDistance = 30f;
    private const int SPEED = 3;
    private const int lineBubblesNum = 15;
    private const int BOUNDS = 350;

    // Use this for initialization
    void Start() {

        lineBubbles = new GameObject[lineBubblesNum];
        for (int i = 0; i < lineBubblesNum; i++)
            lineBubbles[i] = Instantiate(lineBubble, myCanvas.transform);

        disableLine();

        R = myFirstBubble.GetComponent<CircleCollider2D>().radius; //35f;     //GetComponent<CircleCollider2D>().radius;
        r = 5f;    //lineBubble.GetComponent<CircleCollider2D>().radius;
    }

    public void disableLine()
    {
        for (int i = 0; i < lineBubblesNum; i++)
            lineBubbles[i].SetActive(false);

    }

    public void placeLine(Vector2 directionVector)
    {
        distance += SPEED;
        if (distance > maxDistance + 2*r)
            distance = 0;

        Vector2 startPos = myFirstBubble.getPosition();

        lineBubbles[0].transform.localPosition = startPos + directionVector * (R + r + distance);
        lineBubbles[0].SetActive(true);
        lineBubbles[0].GetComponent<Image>().color = myFirstBubble.getColor();

        for (int i = 1; i < lineBubblesNum; i++)
        {
            Vector2 target = startPos + directionVector * (R + r + i*maxDistance + i*2*r + distance);           //(Vector2)lineBubbles[i - 1].transform.localPosition + directionVector * (2 * r + distance);

            if (Mathf.Abs(target.x) + R > BOUNDS)
            {
                float edgeX = target.x > 0 ? BOUNDS - R : -BOUNDS + R;
                float remainingXDistanse = Mathf.Abs(target.x - edgeX);

                float x = target.x > 0 ? edgeX - remainingXDistanse : edgeX + remainingXDistanse;
                float y = target.y;

                target = new Vector2(x, y);
            }

            lineBubbles[i].transform.localPosition = target;
            if (lineBubbles[i].transform.localPosition.y > 0 - R - r)
                lineBubbles[i].SetActive(false);
            else
                lineBubbles[i].SetActive(true);

            lineBubbles[i].GetComponent<Image>().color = myFirstBubble.getColor();

        }
    }

}
