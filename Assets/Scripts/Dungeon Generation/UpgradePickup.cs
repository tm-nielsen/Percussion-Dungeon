﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePickup : MonoBehaviour
{
    public enum UpgradeType { health = 0, bux = 1}
    public UpgradeType type;
    public GameObject pickupEffect;
    public AudioClip noise;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.GetComponent<PlayerController>())
            return;

        if (pickupEffect)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        switch (type)
        {
            case UpgradeType.health:
                GameObject.FindObjectOfType<PlayerHealth>().UpgradeMax();
                GameData.healthUpgrades++;
                GameController.SaveGameData();
                break;
            case UpgradeType.bux:
                //Debug.Log("OMG!! VBUX!!!!!11!!");
                GameData.castas++;
                GameController.SaveGameData();
                break;
        }
        Destroy(gameObject);
        if (noise)
            AudioClipPlayer.Play(noise);
        this.enabled = false;
    }
}
