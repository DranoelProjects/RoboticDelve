using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] float speed = 7f;
    [SerializeField] bool isAbleToAttack = false;
    Animator animator;
    bool lookRight = true;
    public bool OnAttack = false;


    [SerializeField] AudioClip sndAttack;
    AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

        if (Input.GetKeyDown(KeyCode.Mouse0) && !OnAttack && isAbleToAttack)
        {
            OnAttack = true;
            animator.SetTrigger("Attack");
            audioSource.PlayOneShot(sndAttack);
        }
    }

    void Flip()
    {
        lookRight = !lookRight;
        Vector2 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public void AttackBool()
    {
        OnAttack = false;
    }
}

