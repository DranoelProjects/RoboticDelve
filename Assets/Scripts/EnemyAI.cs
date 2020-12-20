﻿using System.Collections;
using UnityEngine;
using Pathfinding;
using System.Linq;
using System;

public class EnemyAI : MonoBehaviour
{
    public Transform sprite;
    private Transform target;
    private GameObject[] player;
    GameObject nearestPlayer;
    PlayerScript nearestPlayerScript;
    public float speed = 200f;
    public float newWaypointDistance = .5f;
    public float minDistance = 1f;
    public float detRange = 5f, attackRange=1f, AttackCouldown=0.5f;
    [SerializeField] public float healthpoints = 10f, healthpointsMax = 10f, Damage = 1f;
    [SerializeField] float attackColliderRadius = 0.7f;
    [SerializeField] bool isBoss = false, needToFlipOtherSide = false, needToDropItem = false;
    [SerializeField] string bossName = "Demon Boss";
    [SerializeField] GameObject itemToDrop;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false, pDetected = false, targetReachable = false, isDead = false;
    public bool ShouldUpdatePlayerArray = false, OnAttack = false, IsRanged = false;

    Seeker seeker;
    Rigidbody2D rb;
    Animator animator;
    CircleCollider2D circleCollider2D;
    Rigidbody2D rigidbody2D;
    float colliderRadius;

    //Sounds
    [SerializeField] AudioClip sndAttack, sndDead, sndBoss;
    AudioSource audioSource;
    bool isBossMusicPlaying;

    //UI
    UIScript uiScript;

    [Header("Ranged Settings")]
    [SerializeField] LayerMask layer;
    [SerializeField] GameObject arrow;
    [SerializeField] float shootForce = 250f, timeShooting = 0.4f; 

    private void Awake()
    {
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        GameObject[] tag1 = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] tag2 = GameObject.FindGameObjectsWithTag("Robot");
        player = tag1.Concat(tag2).ToArray();
        nearestPlayer = player[0];
    }

    // Start is called before the first frame update
    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone() && !isDead)
        {
            if (ShouldUpdatePlayerArray)
            {
                GameObject[] tag1 = GameObject.FindGameObjectsWithTag("Player");
                GameObject[] tag2 = GameObject.FindGameObjectsWithTag("Robot");
                player = tag1.Concat(tag2).ToArray();
                ShouldUpdatePlayerArray = false;
            }
            //checking nearest target
            float lastDistance = 10000f;
            foreach (GameObject currentPlayer in player)
            {
                float currentDistance = Vector2.Distance(currentPlayer.transform.position, gameObject.transform.position);
                if (lastDistance > currentDistance)
                {
                    lastDistance = currentDistance;
                    target = currentPlayer.transform;
                    nearestPlayer = currentPlayer;
                    nearestPlayerScript = nearestPlayer.GetComponent<PlayerScript>();
                }
            }

            seeker.StartPath(rb.position, target.position, onPathComplete);
            float length = 10f;
            if (path != null)
            {
                length = path.GetTotalLength();
            }

            for (int i = 1; i < player.Length; i++)
            {
                Transform temp = player[i].GetComponent<Transform>();
                seeker.StartPath(rb.position, target.position, onPathComplete);
                float tempLength = path.GetTotalLength();
                if (tempLength < length)
                {
                    target = temp;
                    length = tempLength;
                }
            }
            if (length < detRange)
            {
                pDetected = true;
                if (isBoss)
                {
                    detRange = 10000f;
                    if (!isBossMusicPlaying)
                    {
                        isBossMusicPlaying = true;
                        uiScript.PlayBossMusic(sndBoss);
                        uiScript.ActiveFigthingBossUI(gameObject.GetComponentInChildren<EnemyHealthBarScript>(), bossName);
                    }
                }
                if(!OnAttack && length < attackRange && nearestPlayerScript.healthpoints > 0)
                {
                    OnAttack = true;
                    audioSource.PlayOneShot(sndAttack);
                    animator.SetTrigger("Attack");
                    if (IsRanged)
                    {
                        //IA Fire
                        Vector2 rayDir = target.position;
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDir, attackRange, layer);
                        StartCoroutine(Arrow());
                    } else
                    {
                        colliderRadius = circleCollider2D.radius;
                        circleCollider2D.radius = attackColliderRadius;
                    }
                    StartCoroutine(AttackBool());  
                }
            }
            else
            {
                pDetected = false;
            }
            if (!pDetected)
            {
                //select a random point within a range that is not a wall (ispathpossible) patrol logic
                //while (!targetReachable)
                //{

                //}
            }
        }
    }

    IEnumerator AttackBool()
    {
        yield return new WaitForSeconds(AttackCouldown);
        OnAttack = false;
        circleCollider2D.radius = colliderRadius;
    }

    IEnumerator Arrow()
    {
        yield return new WaitForSeconds(timeShooting);
        shootArrow();
    }

    void onPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            if (path == null)
                return;
            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
                reachedEndOfPath = false;
            if (Vector2.Distance(rb.position, target.position) < minDistance || !pDetected || nearestPlayerScript.healthpoints < 0)
            {
                rb.velocity = new Vector2(0, 0);
                animator.SetFloat("SpeedX", 0f);
                return;
            }
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 newVelocity = direction * speed * Time.deltaTime;

            rb.velocity = newVelocity;
            animator.SetFloat("SpeedX", 1f);
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < newWaypointDistance)
                currentWaypoint++;
            if (!needToFlipOtherSide)
            {
                if (direction.x > 0 && sprite.localScale.x < 0)
                    Flip();
                if (direction.x < 0 && sprite.localScale.x > 0)
                    Flip();
            } else
            {
                if (direction.x > 0 && sprite.localScale.x > 0)
                    Flip();
                if (direction.x < 0 && sprite.localScale.x < 0)
                    Flip();
            }

        }
    }

    void Flip()
    {
        Vector3 theLocalScale = sprite.localScale;
        theLocalScale.x *= -1;
        sprite.localScale = theLocalScale;
    }

    public void Hurt()
    {
        animator.SetTrigger("Hurt");
        Vector2 move = gameObject.transform.position - transform.position;
        rigidbody2D.AddForce(move.normalized * -200);
        if (healthpoints <= 0 && !isDead)
        {
            audioSource.PlayOneShot(sndDead);
            isDead = true;
            animator.SetTrigger("Dead");
            animator.SetBool("CanMoove", false);
            StartCoroutine(Dead());
        }
    }
    IEnumerator Dead()
    {
        yield return new WaitForSeconds(2f);
        if (needToDropItem)
        {
            GameObject item = Instantiate(itemToDrop, transform.position, Quaternion.identity);
            item.transform.parent = GameObject.Find("Ressources").transform;
        }
        Destroy(gameObject);
        if (isBoss)
        {
            uiScript.StopBossMusic();
            uiScript.PanelBoss.SetActive(false);
            PlayerPrefs.SetInt("BossKilled", PlayerPrefs.GetInt("BossKilled") + 1);
        } else
        {
            PlayerPrefs.SetInt("MonstersKilled", PlayerPrefs.GetInt("MonstersKilled") + 1);
        }
    }

    private void shootArrow()
    {
        GameObject arrowInstance = Instantiate(arrow, transform.position, Quaternion.identity);
        arrowInstance.transform.parent = gameObject.transform;
        ArrowScript arrowScript = arrowInstance.GetComponent<ArrowScript>();
        arrowScript.TargetPos = nearestPlayer.transform.position;
        arrowScript.Damage = Damage;
    }
}
