﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AnimationMovement))]
[RequireComponent(typeof(DamageReceiver))]
[RequireComponent(typeof(BossHealth))]
[RequireComponent(typeof(AudioClipPlayer))]
public class BossBehavior : MonoBehaviour
{
    public GameObject childPrefab;
    public float birthingVelocity, adaptiveGravity;
    public LayerMask isGround;

    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private CircleCollider2D circCol;
    private Transform target;

    private float circumference, prevX;
    private bool ground, rolling, turnToParent;
    private static bool preggers = true;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        circCol = GetComponent<CircleCollider2D>();

        circumference = circCol.radius * 2 * Mathf.PI;
        AquireTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!target)
            AquireTarget();
        if (!target)
            return;

        rb.AddForce(Mathf.Abs(rb.velocity.y) * adaptiveGravity * Vector2.down);
        CheckGround();
        UpdateAnimator();

        if (rolling)
        {
            float deg = 360 * (transform.position.x - prevX) / circumference;
            rb.rotation -= deg;
            SetChildRotation(deg);

            Vector2 castDir = new Vector2(transform.localScale.x, 0);
            if (Physics2D.Raycast((Vector2)transform.position + circCol.offset,
                castDir, circCol.radius * 3, isGround))
                rolling = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            SetChildRotation(0);
        }

        prevX = transform.position.x;
    }

    private void Update()
    {
        SetChildRotation(0);
    }

    private void UpdateAnimator()
    {
        anim.SetBool("ground", ground);
        anim.SetBool("rolling", rolling);
        anim.SetBool("beat", Music.beat);

        if (turnToParent)
        {
            float xDiff = transform.parent.position.x - transform.position.x;
            xDiff *= transform.localScale.x;
            anim.SetBool("turn", xDiff > 0);
            anim.SetBool("back kick", false);
        }
        else
        {
            float xDiff = target.position.x - transform.position.x;
            xDiff *= transform.localScale.x;
            anim.SetBool("turn", xDiff < 0);
        }

        anim.SetFloat("vx", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("vy", rb.velocity.y);
    }

    private void CheckGround()
    {
        Vector3 groundCheckOffset = col.size / 2 * Vector2.right, cOff = col.offset;
        cOff.x *= transform.localScale.x;
        ground = Physics2D.Raycast(transform.position + groundCheckOffset + cOff, Vector2.down,
            col.size.y / 2 + 0.05f, isGround)
        || Physics2D.Raycast(transform.position - groundCheckOffset + cOff, Vector2.down,
            col.size.y / 2 + 0.05f, isGround);
    }

    private void AquireTarget()
    {
        PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
        if (pCon)
            target = pCon.transform;
    }

    public void OnHalfHealth()
    {
        if (!preggers)
            return;
        GetComponent<DamageReceiver>().invulnerable = true;

        float xDiff = target.position.x - transform.position.x;
        xDiff *= transform.localScale.x;
        anim.SetBool("turn", xDiff < 0);
        turnToParent = true;

        anim.SetTrigger("half health");
        anim.SetBool("phase 2", true);
    }

    private void OnDisable()
    {
        Attack(0);
    }

    private void SetChildRotation(float r)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).eulerAngles = Vector3.forward * r;
        }
    }
    // Animation events ---------------------------------------------------------
    // --------------------------------------------------------------------------
    private void EnterRoll()
    {
        Attack(0);
        Attack(1);
        rolling = true;
        col.enabled = false;
        circCol.enabled = true;

        float x = col.offset.x - circCol.offset.x;
        float y = col.offset.y - col.size.y / 2 - circCol.offset.y + circCol.radius;

        transform.Translate(new Vector2(x, y));
    }

    private void ExitRoll()
    {
        col.enabled = true;
        circCol.enabled = false;

        Vector2 castDir = new Vector2(transform.localScale.x, 0);
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, castDir, 10000, isGround);
        float x = rayHit.point.x;
        rayHit = Physics2D.Raycast(transform.position, Vector2.down, 10000, isGround);
        float y = rayHit.point.y;

        Vector2 newPos = new Vector2(x, y);
        Vector2 off = -col.offset + col.size / 2;
        off.x *= -transform.localScale.x;
        transform.position = (newPos + off);

        Attack(0);
    }

    private void SpawnChild()
    {
        turnToParent = false;
        GetComponent<DamageReceiver>().invulnerable = false;
        preggers = false;
        if (!childPrefab)
            return;

        GameObject child = Instantiate(childPrefab, transform.position, Quaternion.identity);
        child.GetComponent<Animator>().Play("roll");
        Vector2 velocity = new Vector2(-birthingVelocity * transform.localScale.x, 0);
        child.GetComponent<Rigidbody2D>().velocity = velocity;
        child.transform.localScale = new Vector2(-transform.localScale.x, 1);

        GetComponentInChildren<DamageDealer>().
            AddException(child.GetComponent<BoxCollider2D>());
        GetComponentInChildren<DamageDealer>().
            AddException(child.GetComponent<CircleCollider2D>());
        child.GetComponentInChildren<DamageDealer>().
            AddException(GetComponent<BoxCollider2D>());
        child.GetComponentInChildren<DamageDealer>().
            AddException(GetComponent<CircleCollider2D>());

        //sets child to also be in phase 2, comment out if too hard
        child.GetComponent<Animator>().SetBool("phase 2", true);
        FindObjectOfType<BossHUD>().Transition(child);
    }

    private void MidRollBite()
    {
        rolling = false;
        col.enabled = true;
        circCol.enabled = false;
        rb.velocity = Vector2.zero;
        transform.eulerAngles = new Vector3(0, 0, 0);

        Attack(0);
    }

    private void Attack(int i)
    {
        if (!enabled)
            return;
        DamageDealer dealer = GetComponentInChildren<DamageDealer>(true);
        dealer.enabled = i > 0;
        if (i == 0)
            dealer.GetComponent<BoxCollider2D>().size = Vector2.zero;
    }
}
