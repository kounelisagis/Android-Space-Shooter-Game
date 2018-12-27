using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {

    public GameObject LevelsLayer;
    public Button[] levelButtons;

    public Button ResetButton;

    private Vector3 targetPosition = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    public GameObject ResetScreen;
    public Button yesButton, noButton;

    private Vector3 prevPos;

    private bool canMove = true;
    private const int upBorder = -7300;
    private const int downBorder = 0;
    private const int moveTime = 5;


    void Awake() {
        float posY = PlayerPrefs.GetFloat("LevelsPosY", 0f);                                // get previously screen pos
        targetPosition = new Vector3(0, posY, 0);
        LevelsLayer.transform.localPosition = new Vector3(0, posY, 0);

        int maxLevel = PlayerPrefs.GetInt("maxLevel", 1);

        for (int i = 0; i < maxLevel; i++) {
            int memory_allocator = i + 1;                                                       // need allocation
            levelButtons[i].onClick.AddListener(delegate { loadLevel(memory_allocator); });
        }
        for (int i = maxLevel; i < levelButtons.GetLength(0); i++)
            levelButtons[i].interactable = false;

        ResetButton.onClick.AddListener(() => resetScreenActivate(true));
        noButton.onClick.AddListener(() => resetScreenActivate(false));
        yesButton.onClick.AddListener(() => resetLevels());
    }

    void Update () {

        if(canMove)
        {
            LevelsLayer.transform.localPosition = Vector3.SmoothDamp(LevelsLayer.transform.localPosition, targetPosition, ref velocity, moveTime * Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
                prevPos = Input.mousePosition;

            if (Input.GetMouseButton(0))
            {
                Vector3 move = Input.mousePosition - prevPos;


                targetPosition = new Vector3(0, move.y * 3.5f + LevelsLayer.transform.localPosition.y, 0);

                if (targetPosition.y > downBorder)
                    targetPosition = Vector3.zero;
                else if (targetPosition.y < upBorder)
                    targetPosition = new Vector3(0, upBorder, 0);
                else
                    prevPos = Input.mousePosition;
            }
        }

    }

    public void resetScreenActivate(bool a)
    {
        ResetScreen.SetActive(a);
        canMove = !a;
    }

    public void resetLevels()
    {
        resetScreenActivate(false);
        targetPosition = Vector3.zero;
        PlayerPrefs.DeleteAll();
        levelButtons[0].interactable = true;

        for (int i = 1; i < levelButtons.GetLength(0); i++)
            levelButtons[i].interactable = false;
    }

    void loadLevel(int i)
    {
        PlayerPrefs.SetFloat("LevelsPosY", LevelsLayer.transform.localPosition.y);

        PlayerPrefs.SetInt("currLevel", i);
        SceneManager.LoadScene("Main");
    }

}
