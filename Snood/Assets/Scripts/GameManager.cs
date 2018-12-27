using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Canvas myCanvas;

    public Transform upWall;
    public Transform upCollider;


    public BubbleSystem myBubbleSystem;

    public Button pauseButton;
    public PauseSystem myPause;
    public Text levelText;
    public Text scoreText;

    public ResultSystem myResultSystem;

    public Board myBoard;


    public AudioSource bubbleSound;
    public AudioSource backgroundAudio;

    public GameObject Points;
    public GameObject FallingBubble;

    public TextAsset[] levels;

    private List<FallingBubble> fallingObjects;

    private float screenHeight;

    private float timeCounter = 0f;

    public Image Bar;   // for progress


    //------------------CONSTANTS---------------------
    private const float WAIT_TIME_TO_END_LEVEL = 1f;
    private const float WAIT_TIME_TO_COUNT_REMAINING = .5f;


    private const float VOLUME_DECREASE = .01f;

    private const float NORMAL_DELETES_TIME_DELAY = .08f;


    private const int NOT_SUPPORTED_TEXT_FONT_MULTIPLIER = 2;
    private const float NOT_SUPPORTED_DESTROY_TIME = .5f;

    private const int EMPTY_DOWN_AREA = 100;


    private const string LEVEL = "LEVEL ";
    //-----------------------------------------------

    private int startBubbles;


    public void loadPauseMenu()
    {
        myPause.activateMe(true);
    }

    public void pauseBackgroundSound()
    {
        backgroundAudio.Pause();
    }

    public void playBackgroundSound()
    {
        backgroundAudio.Play();
    }



    void Start()
    {
        fallingObjects = new List<FallingBubble>();

        int R = 35;     //GetComponent<CircleCollider2D>().radius;

        screenHeight = myCanvas.GetComponent<RectTransform>().rect.height;


        // WALL AND UP-COLLIDER POSITION
        float upWallPosition = screenHeight / 2 - upWall.GetComponent<RectTransform>().rect.height / 2;
        upWall.localPosition = new Vector2(0, upWallPosition);
        upCollider.localPosition = new Vector2(0, upWallPosition);

        // BUBBLE START POSITION
        int bubbleStartY = (int)(screenHeight / 2 - upWall.GetComponent<RectTransform>().rect.height - R);
        myBoard.setStartY(bubbleStartY);

        // BUBBLE-SYSTEM START POSITION
        Vector2 myPos = new Vector2(0, -screenHeight / 2 + EMPTY_DOWN_AREA);
        myBubbleSystem.transform.localPosition = myPos;


        // LEVEL CREATE - LOAD
        int levelToLoad = PlayerPrefs.GetInt("currLevel", 1) - 1;
        levelText.text = LEVEL + (levelToLoad + 1);

        List < List<int>> listIntegers = StringToInts(levelToLoad);
        startBubbles = myBoard.createBoard(listIntegers);


        myBubbleSystem.setInitialConditions();
        myBoard.autoMoveBoard();

        // BUTTON LISTENERS
        pauseButton.onClick.AddListener(loadPauseMenu);

    }

    void Update()
    {
        if (!myPause.getPaused() && !deleting && noFallingBubbles() && ( myBubbleSystem.getRemainingBubbles() == 0 || myBoard.noMoreBubbles() ) )
        {
            timeCounter += Time.deltaTime;
            backgroundAudio.volume -= VOLUME_DECREASE;

            if (myBubbleSystem.isActiveAndEnabled && timeCounter >= WAIT_TIME_TO_COUNT_REMAINING)
                myBubbleSystem.gameObject.SetActive(false);
                
            if (!myResultSystem.isActiveAndEnabled && timeCounter >= WAIT_TIME_TO_END_LEVEL)
            {
                bool win = myBoard.noMoreBubbles();
                myResultSystem.activate(win);
            }
        }


        if (Input.GetKey(KeyCode.Escape))
            loadLevels();
    }

    private void loadLevels()
    {
        SceneManager.LoadScene("Levels");
    }

    public List<List<int>> StringToInts(int levelToLoad)
    {
        List<List<int>> listIntegers = new List<List<int>>();

        string t = levels[levelToLoad].ToString();

        string[] words = t.Split('\n');

        myBubbleSystem.setBubblesNum(System.Int32.Parse(words[0]));

        for (int i = 2; i < words.Length; i += 2)
        {
            words[i] = words[i].Substring(1);
        }

        for (int i = 1; i < words.Length; i++)
        {
            string[] temp = words[i].Split(' ');
            List<int> tempList = new List<int>();

            for (int j = 0; j < temp.Length; j++)
                tempList.Add(System.Int32.Parse(temp[j]));

            listIntegers.Add(tempList);
        }

        return listIntegers;
    }


    public bool noFallingBubbles()
    {
        foreach (FallingBubble a in fallingObjects)
            if (a!=null)
                return false;

        return true;
    }

    public float getUpWallDownPosY()
    {
        return upWall.transform.localPosition.y - upWall.GetComponent<RectTransform>().rect.height / 2;
    }


    private bool deleting = false;

    public IEnumerator makeDeletes()
    {
        deleting = true;
        List<Bubble> sameColor = myBoard.getSameColorAdjacentBubbles();
        int counter = sameColor.Count;

        if (counter >= 3)
            for (int k = 0; k < counter; k++)
            {
                yield return new WaitForSeconds(NORMAL_DELETES_TIME_DELAY);

                sameColor[k].setEmpty();

                refreshProgress();

                bubbleSound.Stop();
                bubbleSound.Play();
            }


        List<Bubble> notSupported = myBoard.getNotSupportedBubbles();


        for (int k = 0; k < notSupported.Count; k++)
        {
            FallingBubble temp = Instantiate(FallingBubble, myBoard.transform, false).GetComponent<FallingBubble>();
            temp.transform.localPosition = notSupported[k].transform.localPosition;
            temp.setColor(notSupported[k].getColor());
            float random1 = Random.Range(-5f, 5f);
            float random2 = Random.Range(-2f, -5f);
            temp.setVelocity(new Vector2(random1, random2));

            fallingObjects.Add(temp);

            notSupported[k].setEmpty();
        }

        refreshProgress();


        if (!myBoard.noMoreBubbles())
        {
            myBoard.changeAvailableColors();
            myBoard.autoMoveBoard();
            myBubbleSystem.setInitialConditions();
        }

        deleting = false;

    }

    private void refreshProgress()
    {
        float progress = 1f - (float)myBoard.countBubbles() / startBubbles;
        Bar.fillAmount = progress;
    }

}
