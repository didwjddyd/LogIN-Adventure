using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public event EventHandler OnInit;

    [Header("State")]
    public float maxSpeed;
    public float jumpPower;
    public float maxHealth;
    public float curHealth;
    public Vector2 spawnPoint;
    public bool isLive;
    public int coin;
    public int bottle;
    public GameObject[] dressState;

    bool isJumping = false;
    int jumpCount = 2;

    Vector3 boxSize;
    Vector2 inputVector;
    bool inputJump;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;

    // non-roof sound
    [Header("Non-Roof Sound")]
    public AudioClip walkSound;
    public AudioClip jumpSoundStart;
    public AudioClip jumpSoundLand;

    // roof sound
    [Header("Roof Sound")]
    public AudioClip walkSoundRoof;
    public AudioClip jumpSoundStartRoof;
    public AudioClip jumpSoundLandRoof;

    [Header("Player Sound")]
    public AudioClip damageSound;
    public AudioClip itemSound;
    public AudioClip deadSound;

    // current AudioClip
    AudioClip currentWalkSound;
    AudioClip currentJumpSoundStart;
    AudioClip currentJumpSoundLand;

    public AudioSource walkAudio;
    public AudioSource jumpAudio;
    public AudioSource otherAudio;

    public void Awake()
    {
        jumpPower = 12f;
        maxSpeed = 5f;
        maxHealth = 120f;
        curHealth = maxHealth;
        isLive = true;
        rigid = GetComponent<Rigidbody2D>();

        // set default sound: non-roof
        currentWalkSound = walkSound;
        walkAudio.clip = currentWalkSound;
        walkAudio.loop = true;

        currentJumpSoundStart = jumpSoundStart;
        currentJumpSoundLand = jumpSoundLand;
    }

    #region Input System
    private void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    void OnJump()
    {
        inputJump = true;
        anim.SetBool("isJump", true);
    }
    #endregion

    void Update()
    {
        if (isLive)
        {
            // Jump
            if (inputJump && jumpCount > 0)
            {
                inputJump = false;
                jumpCount--;

                jumpAudio.clip = currentJumpSoundStart;
                jumpAudio.Play();

                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                Vector2 jumpVelocity = Vector2.up * jumpPower;
                rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
                isJumping = true;

                gameObject.transform.parent = null;
            }

            // Move
            if (inputVector.x == 0)
            {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0f, rigid.velocity.y);

                RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 1, LayerMask.GetMask("Platform"));

                if (hit.collider != null && hit.collider.gameObject.tag == "MovingPlatform" && !isJumping)
                {
                    gameObject.transform.parent = hit.collider.transform;
                }

            }
            else
            {
                // Sprite Flip by Move Direction
                spriteRenderer.flipX = inputVector.x == -1;

                // set maxspeed
                if (rigid.velocity.x > maxSpeed) // Right Speed
                    rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
                else if (rigid.velocity.x < maxSpeed * -1) // Left Speed
                    rigid.velocity = new Vector2(maxSpeed * -1, rigid.velocity.y);

                gameObject.transform.parent = null;
            }

            if (curHealth <= 0 || rigid.position.y <= -10)
            {
                Dead();
            }

            if (curHealth >= maxHealth)
            {
                curHealth = maxHealth;
            }
        }
    }

    void FixedUpdate()
    {
        // Draw BoxCast Gizmo
        Debug.DrawRay(rigid.position + new Vector2(0f, 0f), new Vector3(0f, -1.2f * transform.localScale.x, 0f), Color.red);
        Debug.DrawRay(rigid.position + new Vector2(0.45f * transform.localScale.x, 0f), new Vector3(0f, -1.2f * transform.localScale.x, 0f), Color.yellow);
        Debug.DrawRay(rigid.position + new Vector2(-0.45f * transform.localScale.x, 0f), new Vector3(0f, -1.2f * transform.localScale.x, 0f), Color.yellow);

        DressState();

        if (isLive)
        {
            //Move By Key Control
            float hor = inputVector.x;
            rigid.AddForce(Vector2.right * hor, ForceMode2D.Impulse);

            //walking animation
            if (inputVector != new Vector2(0, 0))
            {
                anim.SetBool("isWalk", true);

                // paly walking sound if not jumping
                if (!isJumping) walkAudio.enabled = true;
                else walkAudio.enabled = false;
            }
            else
            {
                anim.SetBool("isWalk", false);

                // turn off walking sound
                walkAudio.enabled = false;
            }
        }

        // Landing Platform using BoxCast
        if (rigid.velocity.y < -0.5f)
        {
            // set box size
            boxSize = new Vector3(0.9f, 0.6f, 1f) * transform.localScale.x;

            // BoxCast
            RaycastHit2D boxHit = Physics2D.BoxCast(rigid.position, boxSize, 0f,
                Vector2.down, boxSize.y, LayerMask.GetMask("Platform"));

            if (boxHit.collider != null)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);

                if(isJumping)
                {
                    jumpAudio.clip = currentJumpSoundLand;
                    jumpAudio.Play();
                }

                isJumping = false;
                inputJump = false;
                jumpCount = 2;

                anim.SetBool("isJump", false);

                if (boxHit.collider.gameObject.tag == "MovingPlatform")
                {
                    gameObject.transform.parent = boxHit.collider.transform;
                }
                else
                    gameObject.transform.parent = null;
            }
            else
            {
                inputJump = false;
            }
        }
    }

    void DressState()
    {
        if(coin > 2)
            coin = 2;

        //character dress state
        for (int i = 0; i < dressState.Length; i++)
        {
            if (i == coin)
            {
                dressState[i].SetActive(true);
                spriteRenderer = dressState[i].GetComponent<SpriteRenderer>();
                anim = dressState[i].GetComponent<Animator>();
            }
            else
                dressState[i].SetActive(false);
        }
    }

    public void Dead()
    {
        print("Player Dead");

        otherAudio.clip = deadSound;
        otherAudio.Play();
        isLive = false;
        transform.position = spawnPoint;
        Init();
    }

    private void Init()
    {
        curHealth = maxHealth;
        isLive = true;
        isJumping = false;
        coin = 0;
        transform.localScale = new Vector3(1f, 1f, 1f);
        jumpPower = 12f;
        maxSpeed = 5f;

        if (OnInit != null) OnInit(this, EventArgs.Empty);
    }
    
    public void GetDamage(float damage)
    {
        if (bottle > coin)
        {
            bottle = 0;

            transform.localScale = new Vector3(1f, 1f, 1f);
            jumpPower = 12f;
            maxSpeed = 5f;
        }
        else if (coin > 0)
        {
            coin--;
        }
        else
            curHealth -= damage;

        otherAudio.clip = damageSound;
        otherAudio.Play();
    }

    public bool isPlayerMoving()
    {
        if (inputVector.x == 0) return false;
        else return true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // set roof sound
        if(collision.gameObject.layer == 9) // roof camera confiner layer
        {
            currentWalkSound = walkSoundRoof;
            currentJumpSoundStart = jumpSoundStartRoof;
            currentJumpSoundLand = jumpSoundLandRoof;


            //print(currentWalkSound);
            //print(currentJumpSoundStart);
            //print(currentJumpSoundLand);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // set non-roof sound
        if(collision.gameObject.layer == 9) // roof camera confiner layer
        {
            currentWalkSound = walkSound;
            currentJumpSoundStart = jumpSoundStart;
            currentJumpSoundLand = jumpSoundLand;

            //print(currentWalkSound);
            //print(currentJumpSoundStart);
            //print(currentJumpSoundLand);
        }
    }
}
