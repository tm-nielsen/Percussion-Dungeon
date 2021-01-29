﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public enum UpgradeType { health = 0, bux = 1}
    public UpgradeType type;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.GetComponent<PlayerController>())
            return;
        switch (type)
        {
            case UpgradeType.health:
                GameObject.FindObjectOfType<PlayerController>().GetComponent<Health>().UpgradeMax();
                break;
            case UpgradeType.bux:
                //increase bux by one
                break;
        }
        Destroy(gameObject);
        this.enabled = false;
    }
}
