﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private ControlKey input;

    public Vector2 moveForce;
    public LayerMask isGround;
    public float maxFallSpeed, throwForce, xFriction, coyoteTime = 0.2f;

    private GameObject thrownDrum;
    private Rigidbody2D rb;
    private SpriteRenderer rend;
    private Animator anim;
    private BoxCollider2D hitbox;
    private float cTimer = 0;

    public bool canJump, canDodge;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BoxCollider2D>();
        canDodge = true;

        input = GameObject.Find("Player Control Key").GetComponent<ControlKey>();

        Music.onBeat.Add(OnBeat);
        Music.offBeat.Add(OffBeat);
    }

    private void Update()
    {
        UpdateAnimator();
    }

    // is caled at a fixed rate
    void FixedUpdate()
    {
        if (canJump)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("run") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("land"))
                ApplyRunForce();
            else if (!anim.GetCurrentAnimatorStateInfo(0).IsName("fall"))
                rb.velocity *= Vector2.up;
        }
        else
        {
                AirControl();
            if (rb.velocity.y > maxFallSpeed)
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("jump") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("fall"))
            {
                TurnTowardsInput();
            }
        }

    }

    private void UpdateAnimator()
    {
        Vector3 groundCheckOffset = hitbox.size / 2 * Vector2.right;
        if(Physics2D.Raycast(transform.position + groundCheckOffset, Vector2.down, 
            hitbox.size.y / 2 + 0.05f, isGround)
        || Physics2D.Raycast(transform.position - groundCheckOffset, Vector2.down, 
        hitbox.size.y / 2 + 0.05f, isGround))
        {
            canJump = true;
            cTimer = 0;
        }
        else
        {
            canJump = cTimer < coyoteTime;
            cTimer += Time.deltaTime;
        }

        anim.SetBool("jump", input["jump"] && canJump);

        if (input["attack"])
            anim.SetTrigger("attack");
        if (input["alt attack"])
            anim.SetTrigger("alt attack");

        if (input["dodge"] && canJump && canDodge)
        {
            canDodge = false;
            anim.SetTrigger("dodge");
        }

        anim.SetFloat("vy", rb.velocity.y);
        anim.SetFloat("vx", Mathf.Abs(rb.velocity.x));
        anim.SetBool("ground", canJump);
        anim.SetBool("down", input["down"]);

        anim.SetBool("run", input["left"] || input["right"]);
    }

    private void ApplyRunForce()
    {
        TurnTowardsInput();
        if (input["right"])
            rb.AddForce(moveForce.x * Vector2.right);
        else if (input["left"])
            rb.AddForce(moveForce.x * Vector2.left);
        rb.velocity = new Vector2(rb.velocity.x * xFriction, rb.velocity.y);
    }

    private void AirControl()
    {
        if (input["right"])
            rb.AddForce(moveForce.x * Vector2.right);
        else if (input["left"])
            rb.AddForce(moveForce.x * Vector2.left);

        rb.velocity = new Vector2(rb.velocity.x * Mathf.Pow(xFriction, .6f), rb.velocity.y);
    }

    [HideInInspector]
    private void OnBeat()
    {
        if(anim)
            anim.SetBool("beat", true);
    }
    [HideInInspector]
    public void OffBeat()
    {
        if(anim)
            anim.SetBool("beat", false);
    }

    // --------------------------------------------------------------------------------

    private void SpawnObject(GameObject g)
    {
        Instantiate(g, transform.position, Quaternion.identity);
    }

    private void Jump()
    {
        rb.velocity *= Vector2.right;
        Vector2 dir = moveForce.y * Vector2.up;
        //if (input["right"])
        //    dir += moveForce.x * Vector2.right;
        //if (input["left"])
        //    dir += moveForce.x * Vector2.left;

        //dir.x /= 4;
        rb.AddForce(dir, ForceMode2D.Impulse);
        canJump = false;
        anim.SetBool("ground", false);
        cTimer = coyoteTime;
    }

    private void ThrowDrum(GameObject prefab)
    {
        thrownDrum = Instantiate(prefab, transform.position, Quaternion.identity);
        thrownDrum.GetComponent<SpriteRenderer>().flipX = rend.flipX;
        Rigidbody2D r = thrownDrum.GetComponent<Rigidbody2D>();
        thrownDrum.GetComponentInChildren<DamageDealer>().
            SetSelfReceiver(GetComponent<DamageReceiver>());

        if (r)
            r.velocity = rb.velocity + throwForce * Vector2.down;
    }

    private void RetrieveDrum()
    {
        Destroy(thrownDrum);
    }

    private void FinishDodge()
    {
        canDodge = true;
    }

    private void TurnTowardsInput()
    {
        if (input["right"])
        {
            rend.flipX = false;
        }
        else if (input["left"])
        {
            rend.flipX = true;
        }
    }
}