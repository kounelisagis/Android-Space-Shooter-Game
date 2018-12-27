using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultSystem : MonoBehaviour {

    public GameManager myManager;

    public Text resultText;

    public Button backButton;
    public Button repeatButton;
    public Button nextButton;

    private const string WIN_TEXT = "YOU WON!!!";
    private const string LOSE_TEXT = "YOU LOST";


    void Start()
    {
        backButton.onClick.AddListener(loadLevels);
        repeatButton.onClick.AddListener(reloadScene);
        nextButton.onClick.AddListener(nextLevel);
    }

    public void activate(bool win)
    {
        int maxLevel = PlayerPrefs.GetInt("maxLevel", 1);
        int currLevel = PlayerPrefs.GetInt("currLevel", 1);

        if (win)
        {
            setResultText(WIN_TEXT);

            if (currLevel == maxLevel)
            {
                maxLevel++;
                PlayerPrefs.SetInt("maxLevel", maxLevel);
            }
        }

        else
            setResultText(LOSE_TEXT);

        if(currLevel < maxLevel)
            activateNext(currLevel);

        this.gameObject.SetActive(true);
    }

    public void setResultText(string txt)
    {
        resultText.text = txt;
    }

    public void activateNext(int currLevel)
    {
        if (currLevel + 1 < myManager.levels.GetLength(0))  // check for out of bounds
            nextButton.interactable = true;
    }

    private void loadLevels()
    {
        SceneManager.LoadScene("Levels");
    }

    private void reloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void nextLevel()
    {
        int currLevel = PlayerPrefs.GetInt("currLevel", 1);

        PlayerPrefs.SetInt("currLevel", currLevel + 1);

        SceneManager.LoadScene("Main");
    }

}
