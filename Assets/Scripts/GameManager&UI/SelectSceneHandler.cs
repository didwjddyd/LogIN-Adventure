using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneVariable
{
#if UNITY_EDITOR
    public static int clearState = 0;

#else
    private static int _clearState = -1;

    public static int clearState
    {
        get
        {
            if (_clearState == -1)
            {
                _clearState = PlayerPrefs.GetInt("clearState", 0);
            }

            return _clearState;
        }
        set
        {
            _clearState = value;
            PlayerPrefs.SetInt("clearState", _clearState);
            PlayerPrefs.Save();
        }
    }
#endif
}

public class SelectSceneHandler : MonoBehaviour
{
    [Header("Image Sprite")]
    public Sprite disableMark;
    public Sprite enableMark;
    public Sprite offStar;
    public Sprite onStar;
    public Sprite stage1Image;
    public Sprite stage2Image;
    public Sprite stage3Image;
    public Sprite disableMarkItem;
    public Sprite enableMarkItem;

    [Header("Bookmark")]
    public GameObject stage1Button;
    public GameObject stage2Button;
    public GameObject stage3Button;
    public GameObject itemButton;
    public GameObject startButton;
    public GameObject startImage;

    [Header("Timer")]
    public Text time;
    public GameObject timer;

    [Header("Enemy")]
    public GameObject stage1Enemy;
    public GameObject stage2Enemy;
    public GameObject stage3Enemy;

    [Header("Stage Clear Recognition")]
    public GameObject stamp;
    public GameObject locker;

    [Header("Stage Infomation")]
    public Text subText;
    public Text numText;
    public Text infoText;
    public GameObject allTexts;
    public List<GameObject> stars;
    public GameObject stageImage;
    public GameObject allStars;

    [Header("Transition")]
    public Slider transition;

    [Header("Menu")]
    public GameObject menuUI;

    [Header("Audio")]
    public AudioSource stageAudio;
    public AudioSource lockedStageAudio;
    public AudioSource itemPageAudio;

    [Header("Item Page")]
    public GameObject allItemPage;
    public GameObject itemPage1;
    public GameObject page1right;
    public GameObject itemPage2;
    public GameObject page2left;
    public GameObject page2right;
    public GameObject itemPage3;
    public GameObject page3left;

    string selectedStage;
    string itemPageNum;

    void Start()
    {
        OnStage1Button(false);
    }

    void Update()
    {
#if UNITY_ANDROID
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
#endif
    }

    void SetStar(int n)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i < n)
                stars[i].GetComponent<Image>().sprite = onStar;
            else
                stars[i].GetComponent<Image>().sprite = offStar;
        }
    }

    public void PlaySound()
    {
        stageAudio.Play();
    }

    public void PlayItemPageSound()
    {
        itemPageAudio.Play();
    }

    public void PlayLockedSound()
    {
        lockedStageAudio.Play();
    }

    public void OnStage1Button(bool playSound = true)
    {
        if (selectedStage != "Stage1")
        {
            selectedStage = "Stage1";

            if(playSound) PlaySound();

            stage1Button.GetComponent<Image>().sprite = enableMark;
            stage2Button.GetComponent<Image>().sprite = disableMark;
            stage3Button.GetComponent<Image>().sprite = disableMark;
            itemButton.GetComponent<Image>().sprite = disableMarkItem;

            timer.SetActive(true);
            time.text = "7:00";

            stage1Enemy.SetActive(true);
            stage2Enemy.SetActive(false);
            stage3Enemy.SetActive(false);

            if (SceneVariable.clearState > 0)
                stamp.SetActive(true);
            else
                stamp.SetActive(false);

            locker.SetActive(false);

            allTexts.SetActive(true);
            subText.text = "복도와 인문대를 지나";
            numText.text = "Stage 1";
            infoText.text = "대학교를 탈출하기 위해선 3개의 STAGE를 통과해야 한다. 우리에겐 시간이 촉박하다.\n" +
                "STAGE 1 - 복도와 인문대를 지나기 위해 드디어, 발걸음을 옯기게 되는데...";

            allStars.SetActive(true);
            SetStar(1);

            stageImage.SetActive(true);
            stageImage.GetComponent<Image>().sprite = stage1Image;

            startButton.SetActive(true);
            startImage.SetActive(true);

            allItemPage.SetActive(false);
        }
    }

    public void OnStage2Button()
    {
        if (selectedStage != "Stage2")
        {
            selectedStage = "Stage2";

            stage1Button.GetComponent<Image>().sprite = disableMark;
            stage2Button.GetComponent<Image>().sprite = enableMark;
            stage3Button.GetComponent<Image>().sprite = disableMark;
            itemButton.GetComponent<Image>().sprite = disableMarkItem;

            timer.SetActive(true);
            time.text = "10:00";

            stage1Enemy.SetActive(false);
            stage2Enemy.SetActive(true);
            stage3Enemy.SetActive(false);

            // Stamp
            if (SceneVariable.clearState < 2)
            {
                stamp.SetActive(false);
            }
            else
            {
                stamp.SetActive(true);
            }

            // Button Sound
            if (SceneVariable.clearState < 1)
            {
                //PlayLockedSound();
                PlaySound();
            }
            else
            {
                PlaySound();
            }

            // Start Button
            if (SceneVariable.clearState > 0)
                locker.SetActive(false);
            else
                locker.SetActive(true);

            allTexts.SetActive(true);
            subText.text = "자연대를 거쳐";
            numText.text = "Stage 2";
            infoText.text = "아직 긴장의 끈을 놓을 수 없다.\n" +
                "STAGE 2는 STAGE 1에 비해 난이도가 높다고 알려져 있다. 자연대가 이렇게 무서웠나... 분위기가 매우 스산하고 어둡다.";

            allStars.SetActive(true);
            SetStar(3);

            stageImage.SetActive(true);
            stageImage.GetComponent<Image>().sprite = stage2Image;

            startButton.SetActive(true);
            startImage.SetActive(true);

            allItemPage.SetActive(false);
        }
    }

    public void OnStage3Button()
    {
        if (selectedStage != "Stage3")
        {
            selectedStage = "Stage3";

            stage1Button.GetComponent<Image>().sprite = disableMark;
            stage2Button.GetComponent<Image>().sprite = disableMark;
            stage3Button.GetComponent<Image>().sprite = enableMark;
            itemButton.GetComponent<Image>().sprite = disableMarkItem;

            timer.SetActive(true);
            time.text = "13:00";

            stage1Enemy.SetActive(false);
            stage2Enemy.SetActive(false);
            stage3Enemy.SetActive(true);

            // Stamp
            if (SceneVariable.clearState < 3)
            {
                stamp.SetActive(false);
            }
            else
            {
                stamp.SetActive(true);
            }

            // Button Sound
            if (SceneVariable.clearState < 2)
            {
                //PlayLockedSound();
                PlaySound();
            }
            else
            {
                PlaySound();
            }

            if (SceneVariable.clearState > 1)
                locker.SetActive(false);
            else
                locker.SetActive(true);

            allTexts.SetActive(true);
            subText.text = "미대와 복도, 그리고 옥상";
            numText.text = "Stage 3";
            infoText.text = "마지막 STAGE까지 도달했다.\n여기서 멈출 수 없다.\n" +
                "미대와 복도를 거쳐 옥상 어딘가에 최종 보스가 우리를 기다리고 있다고 하는데...";

            allStars.SetActive(true);
            SetStar(5);

            stageImage.SetActive(true);
            stageImage.GetComponent<Image>().sprite = stage3Image;

            startButton.SetActive(true);
            startImage.SetActive(true);

            allItemPage.SetActive(false);
        }
    }

    public void StartGame()
    {
        if (locker.activeSelf == false)
        {
            PlaySound();
            StartCoroutine("StartFadeout");
        }
        else
        {
            PlayLockedSound();
            StartCoroutine("Lock");
        }
    }

    public void ExitGame()
    {
        PlaySound();

        SceneManager.LoadScene("Start");
    }

    public void MenuButton()
    {
        PlaySound();

        if (menuUI.activeSelf == false)
            menuUI.SetActive(true);
        else
            menuUI.SetActive(false);
    }

    IEnumerator StartFadeout()
    {
        transition.transform.localScale = new Vector3(1, 2, 1);
        while (transition.value < 1)
        {
            transition.value += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(selectedStage);
    }

    IEnumerator Lock()
    {
        for(int i = 0; i < 5; i++)
        {
            locker.transform.localPosition = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            yield return new WaitForSeconds(0.05f);
        }
        locker.transform.localPosition = Vector3.zero;
    }

    public void OnItemButton()
    {
        if (selectedStage != "Item")
        {
            selectedStage = "Item";

            Debug.Log("item");

            PlaySound();

            stage1Button.GetComponent<Image>().sprite = disableMark;
            stage2Button.GetComponent<Image>().sprite = disableMark;
            stage3Button.GetComponent<Image>().sprite = disableMark;
            itemButton.GetComponent<Image>().sprite = enableMarkItem;

            timer.SetActive(false);

            stage1Enemy.SetActive(false);
            stage2Enemy.SetActive(false);
            stage3Enemy.SetActive(false);

            stageImage.SetActive(false);

            stamp.SetActive(false);

            allStars.SetActive(false);

            allTexts.SetActive(false);

            startButton.SetActive(false);
            startImage.SetActive(false);

            allItemPage.SetActive(true);

            FirstItemPage(false);
        }
    }

    public void FirstItemPage(bool playSound = true)
    {
        itemPage1.SetActive(true);
        itemPage2.SetActive(false);
        itemPage3.SetActive(false);

        if(playSound) PlayItemPageSound();

        if (itemPageNum != "Page1")
        {
            itemPageNum = "Page1";
            Debug.Log("1");
        }
    }

    public void SecondItemPage()
    {
        itemPage1.SetActive(false);
        itemPage2.SetActive(true);
        itemPage3.SetActive(false);
        PlayItemPageSound();

        if (itemPageNum != "Page2")
        {
            itemPageNum = "Page2";
            Debug.Log("2");
        }
    }

    public void ThridItemPage()
    {
        itemPage1.SetActive(false);
        itemPage2.SetActive(false);
        itemPage3.SetActive(true);
        PlayItemPageSound();

        if (itemPageNum != "Page3")
        {
            itemPageNum = "Page3";
            Debug.Log("3");
        }
    }
}
