using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class FirstThrowBubble : MonoBehaviour {

    public GameManager myManager;
    public Canvas myCanvas;
    public Board myBoard;

    private Rigidbody2D throwRigidBody;
    private Image img;

    private Vector3 startPosition;

    private bool collisionEnabled = false;


    private const int BUBBLE_SPEED = 19;


    public int getRay()
    {
        return (int)this.GetComponent<RectTransform>().rect.height / 2;
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
    void Awake()
    {
        img = GetComponent<Image>();
        throwRigidBody = GetComponent<Rigidbody2D>();
        startPosition = this.transform.localPosition;
    }

    public void setVelocity(Vector3 myVec)
    {
        throwRigidBody.velocity = myVec * BUBBLE_SPEED;
    }

    public void setStartPosition()
    {
        transform.localPosition = startPosition;
    }
    
    public Vector2 getPosition()
    {
        return this.transform.parent.localPosition + this.transform.localPosition;
    }

    public void setCollider(bool a)
    {
        collisionEnabled = a;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (transform.localPosition !=  startPosition && collisionEnabled)
        {
            if (collision.gameObject.tag == "bubble" || collision.gameObject.tag == "upperΒound")
            {
                setVelocity(Vector3.zero);
                setCollider(false);
                showBubble(false);

                Transform prevParent = this.transform.parent;
                this.transform.SetParent(myBoard.transform);
                myBoard.placeNearest(this.gameObject);  //pass vector 3 and move to start pos
                this.transform.SetParent(prevParent);

                StartCoroutine(myManager.makeDeletes());
            }

            else if (collision.gameObject.tag == "wall")
                throwRigidBody.velocity = new Vector3(-throwRigidBody.velocity.x, throwRigidBody.velocity.y);

        }
    }

}
