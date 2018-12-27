using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingBubble : MonoBehaviour {

    private Image myImage;
    private Rigidbody2D myRB;

    // Use this for initialization
    void Awake () {
        myImage = GetComponent<Image>();
        myRB = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y < -Screen.height - 100)
            Destroy(this.gameObject);
    }


    public void setVelocity(Vector2 myVec) {
        myRB.velocity = myVec;
    }

    public void setColor(Color myColor) {
        myImage.color = myColor;
    }

}
