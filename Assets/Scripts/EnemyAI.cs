using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAI : MonoBehaviour
{
    public Transform sprite;
    private Transform target;
    private GameObject[] player;
    public float speed = 200f;
    public float newWaypointDistance = .5f;
    public float minDistance = 2f;
    public float detRange = 10f;
    [SerializeField] public float healthpoints = 10;
    [SerializeField] public float healthpointsMax = 10;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false, pDetected = false, targetReachable = false;

    Seeker seeker;
    Rigidbody2D rb;
     
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectsWithTag("Player");
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            //a modifier pour target le joueur ou robot le plus proche
            target = player[0].GetComponent<Transform>();
            seeker.StartPath(rb.position, target.position, onPathComplete);
            float length = path.GetTotalLength();
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
                pDetected = true;
            else
                pDetected = false;
            if (!pDetected)
            {
                //select a random point within a range that is not a wall (ispathpossible) patrol logic
                //while (!targetReachable)
                //{

                //}
            }
        }
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
    void FixedUpdate()
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
        if (Vector2.Distance(rb.position, target.position) < minDistance)
        {
            rb.velocity = new Vector2(0, 0);
            return;
        }
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 newVelocity = direction * speed * Time.deltaTime;

        rb.velocity = newVelocity;

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < newWaypointDistance)
            currentWaypoint++;

        if (direction.x > 0 && sprite.localScale.x < 0)
            Flip();
        if (direction.x < 0 && sprite.localScale.x > 0)
            Flip();
    }

    void Flip()
    {
        Vector3 theLocalScale = sprite.localScale;
        theLocalScale.x *= -1;
        sprite.localScale = theLocalScale;
    }
}
