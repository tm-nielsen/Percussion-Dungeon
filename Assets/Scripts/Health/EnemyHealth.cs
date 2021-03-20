﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : Health
{
    public GameObject expOrbPrefab;
    public int experience, group;

    // Start is called before the first frame update
    void Start()
    {

        amount = max;

        base.FindDisplay();
    }

    public override void OnDeath()
    {
        for (int i = 0; i < experience; i++)
            Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
    }
}