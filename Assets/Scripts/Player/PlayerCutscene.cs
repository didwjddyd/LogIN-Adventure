using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCutscene : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public GameObject gameManager;
    GameManager gameManagerCom;

    public GameObject mainUI;

    public Slider transition;

    public AudioSource audioSource;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        gameManagerCom = gameManager.GetComponent<GameManager>();

        StartCoroutine("Move");
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

    //default state coroutine
    IEnumerator Move()
    {
        anim.SetBool("isWalk", true);

        spriteRenderer.flipX = false;
        rigid.velocity = new Vector2(4, 0);
        yield return new WaitForSeconds(3f);

        spriteRenderer.flipX = true;
        rigid.velocity = new Vector2(-4, 0);
        yield return new WaitForSeconds(3f);

        StartCoroutine("Move");
    }

    //StartButton On Click, start cutscene
    public void StartButtonEvent()
    {
        StopCoroutine("Move");
        anim.SetBool("isWalk", false);

        rigid.velocity = new Vector2(0, 0);

        PlaySound();

        StartCoroutine("StartGame");
    }

    IEnumerator StartGame()
    {
        //MainUI fadeOut
        while (mainUI.GetComponent<CanvasGroup>().alpha > 0)
        {
            mainUI.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        mainUI.SetActive(false);

        yield return new WaitForSeconds(1f);

        anim.SetBool("isWalk", true);
        anim.speed = 2;

        spriteRenderer.flipX = false;
        rigid.velocity = new Vector2(8, 0);

        yield return new WaitForSeconds(1f);

        //Transition control
        while (transition.value < 1)
        {
            transition.value += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }

        gameManagerCom.NextScene("UI");
    }

    public void ExitButtonEvent()
    {
        PlaySound();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }    

    public void CreditButtonEvent()
    {
        StopCoroutine("Move");
        anim.SetBool("isWalk", false);

        rigid.velocity = new Vector2(0, 0);

        PlaySound();

        StartCoroutine("CreditFadeOut");
    }

    IEnumerator CreditFadeOut()
    {
        //MainUI fadeOut
        while (mainUI.GetComponent<CanvasGroup>().alpha > 0)
        {
            mainUI.GetComponent<CanvasGroup>().alpha -= 0.02f;
            yield return new WaitForSeconds(0.01f);
        }
        mainUI.SetActive(false);

        yield return new WaitForSeconds(1f);

        //Transition control
        while (transition.value < 1)
        {
            transition.value += 0.05f;
            yield return new WaitForSeconds(0.01f);
        }

        gameManagerCom.NextScene("Credit");
    }

    public void PlaySound()
    {
        audioSource.Play();
    }
}
