using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float speed = 7f;
    Animator animator;
    bool lookRight = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        transform.Translate(Vector2.right * moveX * speed * Time.deltaTime);
        transform.Translate(Vector2.up * moveY * speed * Time.deltaTime);
        animator.SetFloat("SpeedX", Mathf.Abs(moveX));
        animator.SetFloat("SpeedY", moveY);

        if (moveX > 0 && !lookRight)
        {
            Flip();
        }
        else if (moveX < 0 && lookRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        lookRight = !lookRight;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}

