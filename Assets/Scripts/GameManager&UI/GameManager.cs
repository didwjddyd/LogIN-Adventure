using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    Player playerCom;

    [Header("UI")]
    public GameObject gameoverUI;
    public GameObject pauseUI;
    public Image heart;
    public Text timer;

    public GameObject mainCamera;

    float time;
    static GameManager instance;    // singleton
    bool pauseActive = false;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Start")
            playerCom = player.GetComponent<Player>();

        if (SceneManager.GetActiveScene().name == "Stage1")
            time = 600;
        else if (SceneManager.GetActiveScene().name == "Stage2")
            time = 780;
        else
            time = 960;

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
        }
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
        pauseActive = true;
        pauseUI.SetActive(true);
        Time.timeScale = 0;
        mainCamera.GetComponent<UnityEngine.Rendering.Volume>().enabled = true;
    }

    public void Resume()
    {
        pauseActive = false;
        pauseUI.SetActive(false);
        Time.timeScale = 1;
        mainCamera.GetComponent<UnityEngine.Rendering.Volume>().enabled = false;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void NextScene(string stageName)
    {
        SceneManager.LoadScene(stageName);
    }

    IEnumerator GameOver()
    {
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
}
