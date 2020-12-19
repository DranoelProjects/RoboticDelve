using System.Collections;
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
    [SerializeField] bool isBoss = false;
    [SerializeField] string bossName = "Demon Boss";

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false, pDetected = false, targetReachable = false, isDead = false;
    public bool ShouldUpdatePlayerArray = true, OnAttack = false;

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

    private void Awake()
    {
        uiScript = GameObject.Find("MainCanvas").GetComponent<UIScript>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
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
                    colliderRadius = circleCollider2D.radius;
                    circleCollider2D.radius = attackColliderRadius;
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

            if (direction.x > 0 && sprite.localScale.x < 0)
                Flip();
            if (direction.x < 0 && sprite.localScale.x > 0)
                Flip();
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
        Vector2 move = gameObject.transform.position - transform.position;
        rigidbody2D.AddForce(move.normalized * -200);
        if (healthpoints <= 0)
        {
            Destroy(circleCollider2D);
            audioSource.PlayOneShot(sndDead);
            isDead = true;
            animator.SetTrigger("Fall");
            StartCoroutine(Dead());
        }
    }
    IEnumerator Dead()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        uiScript.StopBossMusic();
        uiScript.PanelBoss.SetActive(false);
    }

}
