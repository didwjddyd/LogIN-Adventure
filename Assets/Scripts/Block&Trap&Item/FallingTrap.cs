using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

/*
 * 연필, 지우개 제어
 * Trigger Collider에 플레이어 진입 시 떨어짐
 * Non-Trigger Collider에 플레이어 충돌 시 데미지 주고 사라짐
 * Non-Trigger Collider에 바닥, 가시 충돌 시 사라짐
 * 사라진 후 2초 뒤에 재생성
 */

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class FallingTrap : MonoBehaviour
{   
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    AudioSource fallingAudio;

    Vector2 spawnPoint;

    [Range(1, 4)]
    public int fallingSpeed = 1;

    public float damage = 20f;

    public AudioClip fallingSound;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        fallingAudio = GetComponent<AudioSource>();
        
        fallingAudio.clip = fallingSound;

        spawnPoint = transform.position;

        rigid.gravityScale = fallingSpeed;
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
            rigid.isKinematic = false;
            fallingAudio.Play();
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
        transform.position = spawnPoint;
        gameObject.SetActive(true);
        
        StartCoroutine("FadeIn");
        rigid.isKinematic = true;
    }
}
