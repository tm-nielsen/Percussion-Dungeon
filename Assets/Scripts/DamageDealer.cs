﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public DamageType dType = DamageType.light;
    public float movementValue = 1, vampMultiplier = 0;


    private List<Collider2D> ignore = new List<Collider2D>();
    private Health health;
    private float damageMultiplier = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (ignore.Contains(collision))
            return;

        ignore.Add(collision);
        DamageReceiver[] rec = collision.GetComponents<DamageReceiver>();

        Vector2 dir = GetComponent<BoxCollider2D>().offset;
        dir -= (Vector2)transform.parent.position;
        dir += ((Vector2)transform.localScale - Vector2.up) * 1f;

        if (rec == null)
            return;

        foreach(DamageReceiver r in rec)
            r.TakeDamage(dType, movementValue * damageMultiplier, dir);

        health.Heal(movementValue * damageMultiplier * vampMultiplier);
    }

    private void StartSwing()
    {
        ignore.Clear();
    }

    private void OnEnable()
    {
        ignore.Clear();
        SpriteRenderer parentSprite = GetComponentInParent<SpriteRenderer>();
        if (parentSprite)
            if (parentSprite.flipX)
                transform.localScale = new Vector2(-1, 1);
            else
                transform.localScale = new Vector2(1, 1);

        health = GetComponentInParent<Health>();
    }
}
