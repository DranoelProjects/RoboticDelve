using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    float speed = 10f;
    public Vector3 TargetPos;
    public float Damage;
    Vector3 startPos;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Robot"))
        {
            collision.gameObject.GetComponent<PlayerScript>().Hurt(Damage, new Vector2(1f,-0.3f));
        }
        if (!collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startPos = transform.position;
        Destroy(gameObject, 4f);
    }

    private void Update()
    {
        Vector3 nextPos = Vector3.MoveTowards(transform.position, TargetPos, speed * Time.deltaTime);

        // Rotate to face the next position, and then move there
        transform.rotation = LookAt2D(nextPos - transform.position);
        transform.position = nextPos;

        // Do something when we reach the target
        if (nextPos == TargetPos) Destroy(gameObject);
    }

    static Quaternion LookAt2D(Vector2 forward)
    {
        return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
    }
}
