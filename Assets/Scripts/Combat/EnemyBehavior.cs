﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
[RequireComponent(typeof(DamageReceiver))]
[RequireComponent(typeof(AudioClipPlayer))]
[RequireComponent(typeof(AnimationMovement))]
public class EnemyBehavior : MonoBehaviour
{
    public enum Type { stationary, roaming, chasing, ranged}
    public Type type;

    public float attackDistance;

    private Animator anim;
    private SpriteRenderer rend;
    private Transform target;
    private Rigidbody2D rb;
    private BoxCollider2D hitbox;

    private bool hasTurnAnimation = false, hasVYParam = false, hasGroundParam = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
        foreach (AnimatorControllerParameter p in anim.parameters)
            if (p.name == "turn")
                hasTurnAnimation = true;
            else if (p.name == "vy")
                hasVYParam = true;
            else if (p.name == "ground")
                hasGroundParam = true;
        AquireTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            AquireTarget();
            return;
        }

        switch (type)
        {
            case Type.stationary:
                StationaryBehavior();
                break;
            case Type.roaming:

                break;
            case Type.chasing:

                break;
            case Type.ranged:

                break;
        }

        if (hasVYParam && rb)
            anim.SetFloat("vy", rb.velocity.y);
        if (hasGroundParam && hitbox)
            anim.SetBool("ground", Physics2D.Raycast((Vector2)transform.position + hitbox.offset +
                Vector2.right * (hitbox.size.x / 2), Vector2.down,
                hitbox.size.y / 2 + 0.05f, LayerMask.GetMask("Ground")) ||
                Physics2D.Raycast((Vector2)transform.position + hitbox.offset +
                Vector2.left * (hitbox.size.x / 2), Vector2.down,
                hitbox.size.y / 2 + 0.05f, LayerMask.GetMask("Ground")));

    }

    private void AquireTarget()
    {
        PlayerController pCon = GameObject.FindObjectOfType<PlayerController>();
        if(pCon)
            target = pCon.transform;
    }

    private void StationaryBehavior()
    {
        bool turn = hasTurnAnimation &&
            (rend.flipX && target.transform.position.x > transform.position.x ||
            !rend.flipX && target.transform.position.x < transform.position.x);
        anim.SetBool("turn", turn);

        bool shouldAttack = TargetInSight();// Vector2.Distance(transform.position, target.position) < attackDistance;
        shouldAttack &= Music.beat & !turn;
        anim.SetBool("attack", shouldAttack);
    }

    private bool TargetInSight()
    {
        return (rend.flipX && target.transform.position.x + attackDistance > transform.position.x) ||
            (!rend.flipX && target.transform.position.x < attackDistance + transform.position.x);
    }

    private void Attack(int i)
    {
        bool b = i > 0;
        DamageDealer dealer = GetComponentInChildren<DamageDealer>();
        dealer.enabled = b;
        dealer.GetComponent<BoxCollider2D>().enabled = b;
    }
}
