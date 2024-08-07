using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Diagnostics.Tracing;

public class ElevatorHandler : MonoBehaviour
{
    public GameObject rightGate;
    public GameObject leftGate;
    public GameObject fadeOutPanel;
    public GameObject endTxt;

    AudioSource audioSource;

    private static ElevatorHandler instance = null;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ElevatorHandler Instance
    {
        get
        {
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.isLive = false;      //stop control
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

            fadeOutPanel = GameObject.Find("Canvas").transform.Find("FadeOut").gameObject;

            StartCoroutine(Close(player));
        }
    }

    IEnumerator Close(Player player)
    {
        //bgm off
        player.GetComponent<CameraChanger>().BGMAudio[player.GetComponent<CameraChanger>().currentFloor].Stop();

        player.GetComponent<Player>().curHealth = 500;
        player.GetComponent<Player>().otherAudio.volume = 0;

        //player move to elevator
        while (player.transform.position.x < transform.position.x)
        {
            player.transform.position += new Vector3(0.03f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }

        //arrived
        player.anim.SetBool("isWalk", false);
        player.walkAudio.enabled = false;

        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        yield return new WaitForSeconds(0.3f);

        //gate close
        if(rightGate != null)
        {
            rightGate.GetComponent<SpriteRenderer>().sortingOrder = 5;
            leftGate.GetComponent<SpriteRenderer>().sortingOrder = 5;

            float curPos = 2.9f;

            for (int i = 0; i < 20; i++)
            {
                curPos -= 1.3f / 20;
                rightGate.transform.localPosition = new Vector3(curPos, 0, 0);
                leftGate.transform.localPosition = new Vector3(-curPos, 0, 0);
                yield return new WaitForSeconds(0.02f);
            }
        }

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        fadeOutPanel.gameObject.SetActive(true);
        Image image = fadeOutPanel.GetComponent<Image>();

        float alpha = 0;

        for (int i = 0; i < 50; i++)
        {
            alpha += 0.02f;
            image.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(0.01f);
        }

        yield return new WaitForSeconds(0.1f); // 초기값 0.3

        if (endTxt != null)
        {
            endTxt.gameObject.SetActive(true);

            Text txt = endTxt.GetComponent<Text>();
            alpha = 0;

            for (int i = 0; i < 50; i++)
            {
                alpha += 0.02f;
                txt.color = new Color(255, 255, 255, alpha);
                yield return new WaitForSeconds(0.01f);
            }
        }

        yield return new WaitForSeconds(0.5f); // 초기값 1

        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.006f; // 초기값 0.003
            yield return new WaitForSeconds(0.005f); // 초기값 0.01
        }

        if (SceneManager.GetActiveScene().name == "Stage3")
        {
            ChangeScene();
        }
        else
        {
            // null 체크
            if (Admob.Instance == null)
            {
                Debug.LogError("Admob instance is null");
                ChangeScene();
            }
            else
            {
                GameObject.FindWithTag("Player").SetActive(false);
                Admob.Instance.ShowInterstitialAd();
            }

            //if (UnityAdsManager.Instance == null)
            //{
            //    Debug.LogError("UnityAdsManager instance is null");
            //    ChangeScene();
            //}
            //else
            //{
            //    GameObject.FindWithTag("Player").SetActive(false);
            //    UnityAdsManager.Instance.ShowAd(); 
            //}
        }
    }

    public void ChangeScene()
    {
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            if (SceneVariable.clearState < 1) SceneVariable.clearState = 1;
        }
        else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            if (SceneVariable.clearState < 2) SceneVariable.clearState = 2;
        }
        else
        {
            SceneVariable.clearState = 3;
            SceneManager.LoadScene("Ending");
            return;
        }

        SceneManager.LoadScene("UI");
    }
}
