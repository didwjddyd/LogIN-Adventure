using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    Player playerCom;
    CameraChanger changer;

    [Header("UI")]
    public GameObject gameoverUI;
    public GameObject pauseUI;
    public Image heart;
    public Text timer;

    [Header("Audio")]
    public AudioSource audioSource;

    float time;
    public static GameManager instance;    // singleton
    public bool pauseActive = false;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Start")
        {
            playerCom = player.GetComponent<Player>();
            changer = player.GetComponent<CameraChanger>();
        }

        if (SceneManager.GetActiveScene().name == "Stage1")
            time = 420;
        else if (SceneManager.GetActiveScene().name == "Stage2")
            time = 600;
        else
            time = 780;

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if(SceneManager.GetActiveScene().name != "Start")
        {
            if (!pauseActive)
            {
                time -= Time.deltaTime;

                if ((int)time == 11)
                {
                    changer.PlayHurryUpSound();
                }

                if (time < 0)
                {
                    time = 0;
                    StartCoroutine("GameOver");
                    pauseActive = true;
                }
            }

            //HealthUI
            heart.fillAmount = playerCom.curHealth / 120f;

            timer.text = ((int)time / 60).ToString() + ":" + ((int)time % 60).ToString("D2");

            if(time <= 120)
            {
                Color redColor;
                ColorUtility.TryParseHtmlString("#8A181A", out redColor);
                timer.GetComponent<Text>().color = redColor;
            }
            else
            {
                timer.color = Color.white;
            }
        }

#if UNITY_ANDROID
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
    }

    public void OnPlayerDead()
    {
        gameoverUI.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Pause()
    {
        PlayButtonSound();

        pauseActive = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        PlayButtonSound();

        pauseActive = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1;
    }

    public void ReturnToTitle()
    {
        PlayButtonSound();

        Time.timeScale = 1;
        SceneManager.LoadScene("Start");
    }

    public void Exit()
    {
        PlayButtonSound();

        Time.timeScale = 1;
        SceneManager.LoadScene("UI");
    }

    public void NextScene(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }

    IEnumerator GameOver()
    {
        changer.PlayGameOverSound();
        gameoverUI.GetComponent<CanvasGroup>().alpha = 0;
        gameoverUI.SetActive(true);
        while (gameoverUI.GetComponent<CanvasGroup>().alpha < 1)
        {
            gameoverUI.GetComponent<CanvasGroup>().alpha += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(2f);
        NextScene("UI");
    }

    public void PlayButtonSound()
    {
        audioSource.Play();
    }
}
