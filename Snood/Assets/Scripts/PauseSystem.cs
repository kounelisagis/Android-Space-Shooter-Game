using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseSystem : MonoBehaviour {

    public GameManager myManager;

    public GameObject myPanel;

    public Button soundButton;
    public Sprite mutted;
    public Sprite normal;

    public Button backButton;
    public Button repeatButton;
    public Button continueButton;


    private int sound = 1;


    void Start () {

        // 1 = YES, 0 = NO
        sound = PlayerPrefs.GetInt("sound", 1);
        if (sound == 0)
        {
            myManager.pauseBackgroundSound();
            soundButton.GetComponent<Image>().sprite = mutted;
        }
        else
            myManager.playBackgroundSound();


        soundButton.onClick.AddListener(() => toggleSound());

        backButton.onClick.AddListener(() => loadLevels());
        repeatButton.onClick.AddListener(() => reloadScene());
        continueButton.onClick.AddListener(() => activateMe(false));
    }


    public void toggleSound()
    {
        if(sound == 0)
        {
            sound = 1;
            myManager.playBackgroundSound();
            soundButton.GetComponent<Image>().sprite = normal;
        }
        else
        {
            sound = 0;
            myManager.pauseBackgroundSound();
            soundButton.GetComponent<Image>().sprite = mutted;
        }

        PlayerPrefs.SetInt("sound", sound);
    }

    private bool isPaused = false;

    public bool getPaused()
    {
        return isPaused;
    }

    private IEnumerator setPausedAfterFrame(bool a)
    {
        yield return new WaitForEndOfFrame();
        isPaused = a;
    }


    //BUTTON LEFT
    private void loadLevels()
    {
        SceneManager.LoadScene("Levels");
    }

    //BUTTON MIDDLE
    private void reloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    //BUTTON RIGHT
    public void activateMe(bool a)
    {
        myPanel.SetActive(a);
        StartCoroutine(setPausedAfterFrame(a));
    }
}
