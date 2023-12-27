using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

/*
 * 볼펜 제어
 * Trigger Collider에 플레이어 진입 시 왼쪽으로 delta만큼 움직임
 * Non-Trigger Collider에 플레이어 충돌 시 데미지 주고 사라짐
 * delta만큼 이동했을 경우 사라짐
 * 사라진 후 2초 뒤에 재생성
 */

[RequireComponent(typeof(Rigidbody2D), typeof(AudioSource))]
public class SideTrap : MonoBehaviour
{   
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    AudioSource sideAudio;

    Vector2 spawnPoint;

    [Range(300, 600)]
    public int speed = 300;

    public float delta = 10f;
    public float damage = 20f;

    public AudioClip sideSound;

    // Start is called before the first frame update
    void Start()
    {   
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        sideAudio = GetComponent<AudioSource>();

        sideAudio.clip = sideSound;

        spawnPoint = transform.position;
    }

    private void Update()
    {
        if (rigid.position.x <= spawnPoint.x - delta)
        {
            gameObject.SetActive(false);
            Invoke("Init", 2);
        }
    }

    IEnumerator FadeIn()
    {
        for (int i = 0; i <= 10; i++)
        {
            Color c = spriteRenderer.material.color;
            c.a = i / 10f;
            spriteRenderer.material.color = c;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            rigid.AddForce(Vector2.left * speed); // 수정 필요

            sideAudio.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            //player.curHealth -= damage;
            player.GetDamage(damage);

            gameObject.SetActive(false);
            Invoke("Init", 2);
        }

        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Trap")
        {
            gameObject.SetActive(false);
            Invoke("Init", 2);
        }
    }

    private void Init()
    {
        rigid.velocity = new Vector2(0, 0);
        gameObject.transform.position = spawnPoint;
        gameObject.SetActive(true);
        
        StartCoroutine("FadeIn");
    }
}
